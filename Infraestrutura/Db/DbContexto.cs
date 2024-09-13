using Microsoft.EntityFrameworkCore;
using MinimalApi.DTOs.Dominios;

namespace MinimalApi.Infraestrutura.Db;

public class DbContexto : DbContext
{
    private IConfiguration _configuracaoAppSettings;
    public DbContexto(IConfiguration configuracaoAppSettings)
    {
        _configuracaoAppSettings = configuracaoAppSettings;
    }
    public DbSet<Administrador> Administradores {get; set;} = default!;
    public DbSet<Veiculo> Veiculos {get; set;} = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador {
                Id = 1,// Id só no Seed, nas outra migration não
                Email = "Administrador@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            }
        );
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // se optionsBuilder não foi configurado, então será aqui
        if(!optionsBuilder.IsConfigured)
        {
            var stringConexao = _configuracaoAppSettings.GetConnectionString("MySql")?.ToString();
            // se essa variavel não for nem nulo nem empty, vai para o auto detect
            if(!string.IsNullOrEmpty(stringConexao))
                {
                    optionsBuilder.UseMySql(
                    stringConexao,
                    ServerVersion.AutoDetect(stringConexao)
                );
            }
        }    
    }
}