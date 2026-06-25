using LojaApi.Data;
using LojaApi.DTOs;
using LojaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly LojaContext _context;

    public TagsController(LojaContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagResponseDto>>> GetAll()
    {
        var tags = await _context.Tags
            .Select(t => new TagResponseDto { Id = t.Id, Nome = t.Nome })
            .ToListAsync();

        return Ok(tags);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TagResponseDto>> GetById(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag is null) return NotFound(new { mensagem = $"Tag {id} não encontrada." });

        return Ok(new TagResponseDto { Id = tag.Id, Nome = tag.Nome });
    }

    [HttpPost]
    public async Task<ActionResult<TagResponseDto>> Create(TagCreateDto dto)
    {
        var tag = new Tag { Nome = dto.Nome };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = tag.Id }, new TagResponseDto { Id = tag.Id, Nome = tag.Nome });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TagCreateDto dto)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag is null) return NotFound(new { mensagem = $"Tag {id} não encontrada." });

        tag.Nome = dto.Nome;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag is null) return NotFound(new { mensagem = $"Tag {id} não encontrada." });

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
