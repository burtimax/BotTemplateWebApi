namespace MultipleTestBot.Db.AppDb.Entities;

public class PostEntity : BaseEntity<long>
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string? ImageFilename { get; set; }
    public bool IsHidden { get; set; }
    public int? Score { get; set; }
}