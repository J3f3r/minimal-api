using MinimalApi.DTOs.Dominios;

namespace Test.Domain.Entidades;

[TestClass]
public class AdministradorTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        // arrange
        var adm = new Administrador();

        // act
        adm.Id = 1;// get
        adm.Email = "test@test.com";// set
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        // assert
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("test@test.com", adm.Email);
        Assert.AreEqual("teste", adm.Senha);
        Assert.AreEqual("Adm", adm.Perfil);
    }
}