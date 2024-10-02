using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;
using MinimalApi.DTOs.Dominios;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Dominio.Servicos;

public class VeiculoServico : IVeiculoServico
{
    private readonly DbContexto _contexto;
        public VeiculoServico(DbContexto contexto)
        {
            _contexto = contexto;
        }

    public void Apagar(Veiculo veiculo)
    {
        _contexto.Veiculos.Remove(veiculo);
        _contexto.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
        _contexto.Veiculos.Update(veiculo);
        _contexto.SaveChanges();
    }

    public Veiculo? BuscaPorId(int id)
    {
        return _contexto.Veiculos.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Incluir(Veiculo veiculo)
    {
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();
    }
    

    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {// realiza a consulta à tabela de Veículos com suporte para filtros e paginação

        var query = _contexto.Veiculos.AsQueryable();
        // Se o parâmetro nome não for nulo ou vazio, a consulta é filtrada para incluir apenas veículos cujo nome contenha a string fornecida (ignorando maiúsculas e minúsculas).
        if(!string.IsNullOrEmpty(nome))
        {
            query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome}%"));
        }

        // Ele retorna até 10 veículos por página, começando do item correspondente à página solicitada.
        int itemsnPorPagina = 10;

        if(pagina != null)
            query = query.Skip(((int)pagina - 1) * itemsnPorPagina).Take(itemsnPorPagina);
        

        return query.ToList();
    }
}

