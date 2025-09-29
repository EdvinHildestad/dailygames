namespace DailyGames.Models;

public class GameScore
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    public Profile Profile { get; set; } = null!;
    public GameType GameType { get; set; }
    public DateTime GameDate { get; set; }
    public int? Tries { get; set; } // null means failed/not completed
    public bool IsCompleted { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

public enum GameType
{
    Wordle,
    Wordle_Norwegian, // Wørdle
    Worldle,
    Travle,
    Bandle
}