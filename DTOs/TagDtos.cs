namespace LojaApi.DTOs;

public class TagCreateDto
{
    public string Nome { get; set; } = string.Empty;
}

public class TagResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}
