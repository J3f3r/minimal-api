using MinimalApi.DTOs.Dominios;

namespace Test.Domain.Entidades;

[TestClass]
public class VeiculoTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        // arrange
        var veiculo = new Veiculo();

        // act
        veiculo.Id = 1;// get
        veiculo.Nome = "Atestando";// set
        veiculo.Marca = "teste";
        veiculo.Ano = 1111;
        

        // assert
        Assert.AreEqual(1, veiculo.Id);
        Assert.AreEqual("Atestando", veiculo.Nome);
        Assert.AreEqual("teste", veiculo.Marca);
        Assert.AreEqual(1111, veiculo.Ano);
    }
}