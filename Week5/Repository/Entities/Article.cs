namespace Week5.Repository.Entities
{
public class Article
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; }
}
}