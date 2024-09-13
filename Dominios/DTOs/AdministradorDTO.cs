using MinimalApi.Dominio.Enuns;

namespace MinimalApi.DTOs;

// record é uma instancia menor que uma classe
public record AdministradorDTO
{
    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
    public Perfil? Perfil { get; set; } = default!;
}