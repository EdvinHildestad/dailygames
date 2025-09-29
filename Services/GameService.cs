using DailyGames.Models;

namespace DailyGames.Services;

public interface IGameService
{
    Task<List<GameScore>> GetScoresByProfileAsync(int profileId);
    Task<List<GameScore>> GetScoresByGameTypeAsync(GameType gameType);
    Task<GameScore?> GetTodayScoreAsync(int profileId, GameType gameType);
    Task<GameScore> AddScoreAsync(int profileId, GameType gameType, int? tries, bool isCompleted, DateTime? gameDate = null);
    Task<List<GameScore>> GetLeaderboardAsync(GameType? gameType = null, DateTime? date = null);
    List<GameInfo> GetAllGames();
}

public class InMemoryGameService : IGameService
{
    private readonly List<GameScore> _scores = new();
    private int _nextId = 1;

    public Task<List<GameScore>> GetScoresByProfileAsync(int profileId)
    {
        var scores = _scores.Where(s => s.ProfileId == profileId).ToList();
        return Task.FromResult(scores);
    }

    public Task<List<GameScore>> GetScoresByGameTypeAsync(GameType gameType)
    {
        var scores = _scores.Where(s => s.GameType == gameType).ToList();
        return Task.FromResult(scores);
    }

    public Task<GameScore?> GetTodayScoreAsync(int profileId, GameType gameType)
    {
        var today = DateTime.UtcNow.Date;
        var score = _scores.FirstOrDefault(s => 
            s.ProfileId == profileId && 
            s.GameType == gameType && 
            s.GameDate.Date == today);
        return Task.FromResult(score);
    }

    public Task<GameScore> AddScoreAsync(int profileId, GameType gameType, int? tries, bool isCompleted, DateTime? gameDate = null)
    {
        var score = new GameScore
        {
            Id = _nextId++,
            ProfileId = profileId,
            GameType = gameType,
            GameDate = gameDate ?? DateTime.UtcNow.Date,
            Tries = tries,
            IsCompleted = isCompleted,
            CreatedDate = DateTime.UtcNow
        };

        _scores.Add(score);
        return Task.FromResult(score);
    }

    public Task<List<GameScore>> GetLeaderboardAsync(GameType? gameType = null, DateTime? date = null)
    {
        var query = _scores.AsQueryable();

        if (gameType.HasValue)
            query = query.Where(s => s.GameType == gameType.Value);

        if (date.HasValue)
            query = query.Where(s => s.GameDate.Date == date.Value.Date);

        var leaderboard = query
            .Where(s => s.IsCompleted)
            .OrderBy(s => s.Tries)
            .ThenBy(s => s.CreatedDate)
            .ToList();

        return Task.FromResult(leaderboard);
    }

    public List<GameInfo> GetAllGames()
    {
        return new List<GameInfo>
        {
            new GameInfo
            {
                GameType = GameType.Wordle,
                Name = "Wordle",
                Description = "Guess the 5-letter word in 6 tries",
                Url = "https://www.nytimes.com/games/wordle/index.html",
                Icon = "🔤",
                Color = "#6AAA64"
            },
            new GameInfo
            {
                GameType = GameType.Wordle_Norwegian,
                Name = "Wørdle",
                Description = "Norwegian version of Wordle",
                Url = "https://xn--wrdle-vua.dk/",
                Icon = "🇳🇴",
                Color = "#D92A3E"
            },
            new GameInfo
            {
                GameType = GameType.Worldle,
                Name = "Worldle",
                Description = "Guess the country by its outline",
                Url = "https://worldle.teuteuf.fr/",
                Icon = "🌍",
                Color = "#4285F4"
            },
            new GameInfo
            {
                GameType = GameType.Travle,
                Name = "Travle",
                Description = "Plan your trip between countries",
                Url = "https://travle.earth/",
                Icon = "✈️",
                Color = "#FF6B35"
            },
            new GameInfo
            {
                GameType = GameType.Bandle,
                Name = "Bandle",
                Description = "Guess the song from audio clues",
                Url = "https://bandle.app/daily",
                Icon = "🎵",
                Color = "#1DB954"
            }
        };
    }
}