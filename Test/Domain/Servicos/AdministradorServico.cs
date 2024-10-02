using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Servicos;
using MinimalApi.DTOs;
using MinimalApi.DTOs.Dominios;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Entidades;

// Serico é responsável pelos testes de persistência e não o Mock

[TestClass]
public class AdministradorServicoTest
{
    private DbContexto CriarContextoDeTeste()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DbContexto(configuration);
    }

    [TestMethod]
    public void TestandoSalvarAdministradores()
    {
        // arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Administrador();
        adm.Email = "test@test.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";   
        var administradorServico = new AdministradorServico(context);

        // act
        administradorServico.Incluir(adm);

        // assert
        Assert.AreEqual(1, administradorServico.Todos(1).Count());
    }

    [TestMethod]
    public void TestandoBuscaPorId()
    {
        // arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Administrador();
        adm.Email = "test@test.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";   
        var administradorServico = new AdministradorServico(context);

        // act
        administradorServico.Incluir(adm);
        var admDoBanco = administradorServico.BuscarPorId(adm.Id);

        // assert
        Assert.AreEqual(1, admDoBanco?.Id);
    }

    [TestMethod]
    public void TestandoLogin()
    {
        // arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Administrador();
        adm.Email = "test@test.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";   
        var administradorServico = new AdministradorServico(context);
        var login = new LoginDTO();
        // act
        
        administradorServico.Login(login);

        // assert
        Assert.IsNotNull(login);
        Assert.AreEqual(adm.Email, "test@test.com");
        Assert.AreEqual(adm.Senha, "teste");
    }

    [TestMethod]
    public void TestandoTodosAdmPaginados()// falhou
    {
        // arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        for (int i = 1; i <= 10; i++)
        {
            context.Administradores.Add(new Administrador { Email = $"admin{i}@teste.com", Senha = "senha", Perfil = "Adm" });
            // aqui teria que incluir cada adm individualmente
        }
        context.SaveChanges();

        /*var adm = new List<Administrador>{
            new Administrador{Email = "test@test.com", Senha = "teste", Perfil = "Adm"}
        };*/   
        var administradorServico = new AdministradorServico(context);
        
        
        // act
        
        var resultado = administradorServico.Todos(1);

        // assert
        Assert.AreEqual(10, resultado.Count);
    }

    [TestMethod]
    public void TestandoTodosAdmNaoPaginados()// Passou
    {
        // arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new List<Administrador>();   
        var administradorServico = new AdministradorServico(context);
        
        
        // act
        
        var resultado = administradorServico.Todos(null);

        // assert
        Assert.AreEqual(adm.Count, resultado.Count);
    }

    [TestMethod]
    public void TestandoMetodoTodos_ComPaginacao()// não tenho certeza se atende aos requsitos do metodo Todos()
    {
        // Arrange
        var contexto = CriarContextoDeTeste();
        var administradorServico = new AdministradorServico(contexto);

        // Inserindo 15 administradores no contexto
        for (int i = 1; i <= 15; i++)
        {
            contexto.Administradores.Add(new Administrador { Email = $"admin{i}@teste.com", Senha = "senha", Perfil = "Adm" });
        }
        contexto.SaveChanges();

        // Act
        var resultadoPagina1 = administradorServico.Todos(1);
        var resultadoPagina2 = administradorServico.Todos(2);

        // Assert
        Assert.AreEqual(10, resultadoPagina1.Count);  // Página 1 tem 10 itens
        Assert.AreEqual(5, resultadoPagina2.Count);   // Página 2 tem 5 itens restantes
    }

}