using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Servicos;
using MinimalApi.DTOs;
using MinimalApi.DTOs.Dominios;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Entidades;

// Serviço é responsável pelos testes de persistência e não o Mock

[TestClass]
public class VeiculoServicoTest
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
    public void TestandoSalvarVeiculos()
    {
        // arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo();
        veiculo.Nome = "Atestando";
        veiculo.Marca = "teste";
        veiculo.Ano = 1111;   
        var veiculoServico = new VeiculoServico(context);

        // act
        veiculoServico.Incluir(veiculo);

        // assert
        Assert.AreEqual(1, veiculoServico.Todos(1).Count());
    }

    [TestMethod]
    public void TestandoBuscaPorId()
    {
        // arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo();
        veiculo.Nome = "Atestando";
        veiculo.Marca = "teste";
        veiculo.Ano = 1111;   
        var veiculoServico = new VeiculoServico(context);

        // act
        veiculoServico.Incluir(veiculo);
        var veiculoDoBanco = veiculoServico.BuscaPorId(veiculo.Id);

        // assert
        Assert.AreEqual(1, veiculoDoBanco?.Id);
    }


    [TestMethod]
    public void TestandoTodosVeiculosPaginados()
    {
        // arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        for (int i = 1; i <= 10; i++)
        {
            context.Veiculos.Add(new Veiculo { Nome = $"Nome{i}", Marca = "marca"});
        }
        context.SaveChanges();
  
        var veiculoServico = new VeiculoServico(context);
        
        
        // act
        
        var resultado = veiculoServico.Todos(1);

        // assert
        Assert.AreEqual(10, resultado.Count);
    }

    [TestMethod]
    public void TestandoTodosAdmNaoPaginados()// Passou
    {
        // arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new List<Veiculo>();   
        var veiculoServico = new VeiculoServico(context);
        
        
        // act
        
        var resultado = veiculoServico.Todos(null);

        // assert
        Assert.AreEqual(veiculo.Count, resultado.Count);
    }

    [TestMethod]
    public void TestandoMetodoTodos_ComFiltrosEPaginacao()
    {
        // Arrange
        var contexto = CriarContextoDeTeste();
        var veiculoServico = new VeiculoServico(contexto);

        // Inserindo veículos no contexto
        contexto.Veiculos.AddRange(
            new Veiculo { Nome = "Carro A", Marca = "Marca1", Ano = 1112 },
            new Veiculo { Nome = "Carro B", Marca = "Marca2", Ano = 1113 },
            new Veiculo { Nome = "Carro C", Marca = "Marca1", Ano = 1114 },
            new Veiculo { Nome = "Carro C", Marca = "Marca1", Ano = 1115 }
        );
        contexto.SaveChanges();

        // Act
        var resultadoPagina1 = veiculoServico.Todos(1, nome: "Carro C");

        // Assert
        Assert.AreEqual(2, resultadoPagina1.Count);  // Deve encontrar 2 veículos com nome "Carro C"
        Assert.IsTrue(resultadoPagina1.All(v => v.Nome.Contains("Carro C")));
    }

}