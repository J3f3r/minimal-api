using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Dominio.Enuns;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.DTOs;
using MinimalApi.DTOs.Dominios;
using MinimalApi.Infraestrutura.Db;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration?.GetSection("Jwt")?.ToString()?? "";
    }

    private string key = "";
    public IConfiguration Configuration {get; set;} = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option =>{
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>{
            option.TokenValidationParameters = new TokenValidationParameters{
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdministradorServico, AdministradorServico>();
        services.AddScoped<IVeiculoServico, VeiculoServico>();

        services.AddEndpointsApiExplorer();
        // configurado e adicionado swagger
        services.AddSwaggerGen(option =>{
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
                Name = "Autorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT aqui"
            });

            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string [] {}
                }
            });
        });

        services.AddDbContext<DbContexto>(options => {
            options.UseMySql(
                Configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("DefaultConnection"))
            );
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => {
            #region Home
            // em vez de retornar hello word na home, sera retornado novo modelo de visualização Home
            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");// este wifitags é para organizar no swagger
            #endregion

            #region Administardores
            string GererTokenJwt(Administrador administrador){
                if(string.IsNullOrEmpty(key)) return string.Empty;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);// este faz a criptografia

                var claims = new List<Claim>()
                {
                    new Claim("Email", administrador.Email),
                    new Claim("Pefil", administrador.Perfil),
                    new Claim(ClaimTypes.Role, administrador.Perfil),// necessário na autorizaçãp do .NET,para que seja definido o perfil 
                };

                var token = new JwtSecurityToken(
                    claims: claims,
                    // este token expira em 1 dia
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            endpoints.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {
                //if(loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456") era o teste que foi substituido

                var adm = administradorServico.Login(loginDTO);
                if(adm != null)
                {
                    string token = GererTokenJwt(adm);
                    return Results.Ok( new AdministradorLogado
                    {
                        Email = adm.Email,
                        Perfil = adm.Perfil,
                        Token = token
                    });
                }
                else
                    return Results.Unauthorized();
            }).AllowAnonymous().WithTags("Administradores");
            // essa rota fica sem restrição

            endpoints.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) => {
                // listar administradores
                var adms = new List<AdministradorModelViews>();

                var administradores = administradorServico.Todos(pagina);

                foreach(var adm in administradores)
                {
                    adms.Add(new AdministradorModelViews{
                        Id = adm.Id,
                        Email = adm.Email,
                        Perfil = adm.Perfil
                    });
                }

                return Results.Ok(adms);

            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
            .WithTags("Administradores");
            // agora essas rotas requrem autorização e com o perfil definido

            endpoints.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) => {
                var administrador = administradorServico.BuscarPorId(id);

                if(administrador == null) return Results.NotFound();// não tendo o veiculo cadastrado retorna não encontrado
                
                return Results.Ok(new AdministradorModelViews{
                        Id = administrador.Id,
                        Email = administrador.Email,
                        Perfil = administrador.Perfil
                });// retorna 200 no swagger

            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
            .WithTags("Administradores");

            endpoints.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) => {
                
                var validacao = new ErrosDeValidacao{
                    Mensagens = new List<string>()
                };
                
                if(string.IsNullOrEmpty(administradorDTO.Email))
                    validacao.Mensagens.Add("Email não pode ser vazio");

                if(string.IsNullOrEmpty(administradorDTO.Senha))
                    validacao.Mensagens.Add("Email não pode ser vazia");

                if(administradorDTO.Perfil == null)
                    validacao.Mensagens.Add("Email não pode ser vazio");

                
                if(validacao.Mensagens.Count > 0)
                    return Results.BadRequest(validacao);

                var administrador = new Administrador{
                    Senha = administradorDTO.Senha,
                    Email = administradorDTO.Email,
                    Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
                };

                administradorServico.Incluir(administrador);

                return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelViews{
                        Id = administrador.Id,
                        Email = administrador.Email,
                        Perfil = administrador.Perfil
                });

            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
            .WithTags("Administradores");
            #endregion
        });
    }
}