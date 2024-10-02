namespace MinimalApi.DTOs;

// record é uma instancia menor que uma classe
public record LoginDTO
{
    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
}