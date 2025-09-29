namespace DailyGames.Models;

public class Profile
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public List<GameScore> Scores { get; set; } = new();
}