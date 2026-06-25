using LojaApi.Data;
using LojaApi.DTOs;
using LojaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PessoasController : ControllerBase
{
    private readonly LojaContext _context;

    public PessoasController(LojaContext context)
    {
        _context = context;
    }

    // GET: api/pessoas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PessoaResponseDto>>> GetAll()
    {
        var pessoas = await _context.Pessoas
            .Include(p => p.Endereco)
            .Select(p => ToDto(p))
            .ToListAsync();

        return Ok(pessoas);
    }

    // GET: api/pessoas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PessoaResponseDto>> GetById(int id)
    {
        var pessoa = await _context.Pessoas
            .Include(p => p.Endereco)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pessoa is null) return NotFound(new { mensagem = $"Pessoa {id} não encontrada." });

        return Ok(ToDto(pessoa));
    }

    // POST: api/pessoas
    [HttpPost]
    public async Task<ActionResult<PessoaResponseDto>> Create(PessoaCreateDto dto)
    {
        var pessoa = new Pessoa
        {
            Nome = dto.Nome,
            Email = dto.Email,
            DataNascimento = dto.DataNascimento,
            Endereco = dto.Endereco is null ? null : new Endereco
            {
                Logradouro = dto.Endereco.Logradouro,
                Numero = dto.Endereco.Numero,
                Cidade = dto.Endereco.Cidade,
                Estado = dto.Endereco.Estado,
                Cep = dto.Endereco.Cep
            }
        };

        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = pessoa.Id }, ToDto(pessoa));
    }

    // PUT: api/pessoas/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, PessoaCreateDto dto)
    {
        var pessoa = await _context.Pessoas
            .Include(p => p.Endereco)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pessoa is null) return NotFound(new { mensagem = $"Pessoa {id} não encontrada." });

        pessoa.Nome = dto.Nome;
        pessoa.Email = dto.Email;
        pessoa.DataNascimento = dto.DataNascimento;

        if (dto.Endereco is not null)
        {
            if (pessoa.Endereco is null)
            {
                pessoa.Endereco = new Endereco { PessoaId = pessoa.Id };
            }

            pessoa.Endereco.Logradouro = dto.Endereco.Logradouro;
            pessoa.Endereco.Numero = dto.Endereco.Numero;
            pessoa.Endereco.Cidade = dto.Endereco.Cidade;
            pessoa.Endereco.Estado = dto.Endereco.Estado;
            pessoa.Endereco.Cep = dto.Endereco.Cep;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/pessoas/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var pessoa = await _context.Pessoas.FindAsync(id);
        if (pessoa is null) return NotFound(new { mensagem = $"Pessoa {id} não encontrada." });

        _context.Pessoas.Remove(pessoa);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static PessoaResponseDto ToDto(Pessoa p) => new()
    {
        Id = p.Id,
        Nome = p.Nome,
        Email = p.Email,
        DataNascimento = p.DataNascimento,
        Endereco = p.Endereco is null ? null : new EnderecoDto
        {
            Logradouro = p.Endereco.Logradouro,
            Numero = p.Endereco.Numero,
            Cidade = p.Endereco.Cidade,
            Estado = p.Endereco.Estado,
            Cep = p.Endereco.Cep
        }
    };
}
