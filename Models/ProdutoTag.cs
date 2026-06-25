using System.Text.Json.Serialization;

namespace LojaApi.Models;

// Tabela associativa explícita para o relacionamento N:M entre Produto e Tag
public class ProdutoTag
{
    public int ProdutoId { get; set; }
    [JsonIgnore]
    public Produto? Produto { get; set; }

    public int TagId { get; set; }
    [JsonIgnore]
    public Tag? Tag { get; set; }
}
