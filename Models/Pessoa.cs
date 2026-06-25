namespace LojaApi.Models;

public class Pessoa
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }

    // Relacionamento 1:1 -> uma Pessoa possui um Endereco
    public Endereco? Endereco { get; set; }
}
