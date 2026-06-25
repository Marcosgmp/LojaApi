using System.Text.Json.Serialization;

namespace LojaApi.Models;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Estoque { get; set; }

    // Relacionamento N:1 -> muitos Produtos pertencem a uma Categoria
    public int CategoriaId { get; set; }

    [JsonIgnore]
    public Categoria? Categoria { get; set; }

    // Relacionamento N:M -> Produto <-> Tag via tabela associativa ProdutoTag
    [JsonIgnore]
    public List<ProdutoTag> ProdutoTags { get; set; } = new();
}
