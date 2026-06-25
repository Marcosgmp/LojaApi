using LojaApi.Data;
using LojaApi.DTOs;
using LojaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly LojaContext _context;

    public CategoriasController(LojaContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaResponseDto>>> GetAll()
    {
        var categorias = await _context.Categorias
            .Include(c => c.Produtos)
            .Select(c => new CategoriaResponseDto
            {
                Id = c.Id,
                Nome = c.Nome,
                Descricao = c.Descricao,
                QuantidadeProdutos = c.Produtos.Count
            })
            .ToListAsync();

        return Ok(categorias);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaResponseDto>> GetById(int id)
    {
        var categoria = await _context.Categorias
            .Include(c => c.Produtos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria is null) return NotFound(new { mensagem = $"Categoria {id} não encontrada." });

        return Ok(new CategoriaResponseDto
        {
            Id = categoria.Id,
            Nome = categoria.Nome,
            Descricao = categoria.Descricao,
            QuantidadeProdutos = categoria.Produtos.Count
        });
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaResponseDto>> Create(CategoriaCreateDto dto)
    {
        var categoria = new Categoria { Nome = dto.Nome, Descricao = dto.Descricao };
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, new CategoriaResponseDto
        {
            Id = categoria.Id,
            Nome = categoria.Nome,
            Descricao = categoria.Descricao,
            QuantidadeProdutos = 0
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CategoriaCreateDto dto)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria is null) return NotFound(new { mensagem = $"Categoria {id} não encontrada." });

        categoria.Nome = dto.Nome;
        categoria.Descricao = dto.Descricao;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var categoria = await _context.Categorias.Include(c => c.Produtos).FirstOrDefaultAsync(c => c.Id == id);
        if (categoria is null) return NotFound(new { mensagem = $"Categoria {id} não encontrada." });

        if (categoria.Produtos.Any())
            return BadRequest(new { mensagem = "Não é possível excluir uma categoria que possui produtos vinculados." });

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
