using System.Text.Json.Serialization;

namespace LojaApi.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    // Relacionamento 1:N -> uma Categoria possui muitos Produtos
    [JsonIgnore]
    public List<Produto> Produtos { get; set; } = new();
}
