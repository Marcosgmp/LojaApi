using LojaApi.Data;
using LojaApi.DTOs;
using LojaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly LojaContext _context;

    public ProdutosController(LojaContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoResponseDto>>> GetAll()
    {
        var produtos = await _context.Produtos
            .Include(p => p.Categoria)
            .Include(p => p.ProdutoTags).ThenInclude(pt => pt.Tag)
            .ToListAsync();

        return Ok(produtos.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProdutoResponseDto>> GetById(int id)
    {
        var produto = await _context.Produtos
            .Include(p => p.Categoria)
            .Include(p => p.ProdutoTags).ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto is null) return NotFound(new { mensagem = $"Produto {id} não encontrado." });

        return Ok(ToDto(produto));
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoResponseDto>> Create(ProdutoCreateDto dto)
    {
        var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
        if (!categoriaExiste)
            return BadRequest(new { mensagem = $"Categoria {dto.CategoriaId} não existe." });

        var produto = new Produto
        {
            Nome = dto.Nome,
            Preco = dto.Preco,
            Estoque = dto.Estoque,
            CategoriaId = dto.CategoriaId
        };

        await AtualizarTags(produto, dto.TagIds, isNovo: true);

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        await _context.Entry(produto).Reference(p => p.Categoria).LoadAsync();
        await _context.Entry(produto).Collection(p => p.ProdutoTags).Query().Include(pt => pt.Tag).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = produto.Id }, ToDto(produto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProdutoCreateDto dto)
    {
        var produto = await _context.Produtos
            .Include(p => p.ProdutoTags)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto is null) return NotFound(new { mensagem = $"Produto {id} não encontrado." });

        var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
        if (!categoriaExiste)
            return BadRequest(new { mensagem = $"Categoria {dto.CategoriaId} não existe." });

        produto.Nome = dto.Nome;
        produto.Preco = dto.Preco;
        produto.Estoque = dto.Estoque;
        produto.CategoriaId = dto.CategoriaId;

        await AtualizarTags(produto, dto.TagIds, isNovo: false);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto is null) return NotFound(new { mensagem = $"Produto {id} não encontrado." });

        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task AtualizarTags(Produto produto, List<int> tagIds, bool isNovo)
    {
        if (!isNovo)
            produto.ProdutoTags.Clear();

        if (tagIds.Count == 0) return;

        var tagsValidas = await _context.Tags
            .Where(t => tagIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync();

        foreach (var tagId in tagsValidas)
        {
            produto.ProdutoTags.Add(new ProdutoTag { TagId = tagId, ProdutoId = produto.Id });
        }
    }

    private static ProdutoResponseDto ToDto(Produto p) => new()
    {
        Id = p.Id,
        Nome = p.Nome,
        Preco = p.Preco,
        Estoque = p.Estoque,
        CategoriaId = p.CategoriaId,
        CategoriaNome = p.Categoria?.Nome,
        Tags = p.ProdutoTags.Where(pt => pt.Tag != null).Select(pt => pt.Tag!.Nome).ToList()
    };
}
