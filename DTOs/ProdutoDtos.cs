namespace LojaApi.DTOs;

public class ProdutoCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public int CategoriaId { get; set; }
    public List<int> TagIds { get; set; } = new();
}

public class ProdutoResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public int CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
    public List<string> Tags { get; set; } = new();
}
