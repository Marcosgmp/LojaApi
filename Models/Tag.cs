using System.Text.Json.Serialization;

namespace LojaApi.Models;

public class Tag
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;

    [JsonIgnore]
    public List<ProdutoTag> ProdutoTags { get; set; } = new();
}
