using System.Text.Json.Serialization;

namespace LojaApi.Models;

public class Endereco
{
    public int Id { get; set; }
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;

    // FK para o relacionamento 1:1 (Endereco é o lado dependente)
    public int PessoaId { get; set; }

    [JsonIgnore]
    public Pessoa? Pessoa { get; set; }
}
