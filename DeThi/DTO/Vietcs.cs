namespace DeThi.DTO;

public class Vietcs
{
    public string Topic { get; set; } = null!;

    public int Level { get; set; }

    public string Purpose { get; set; } = null!;

    public string JsonContent { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
