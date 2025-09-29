using Microsoft.AspNetCore.Mvc.RazorPages;
using DailyGames.Models;
using DailyGames.Services;

namespace DailyGames.Pages;

public class LeaderboardModel : PageModel
{
    private readonly IProfileService _profileService;
    private readonly IGameService _gameService;

    public LeaderboardModel(IProfileService profileService, IGameService gameService)
    {
        _profileService = profileService;
        _gameService = gameService;
    }

    public List<Profile> Profiles { get; set; } = new();
    public List<GameInfo> Games { get; set; } = new();
    public Dictionary<GameType, List<GameScore>> GameLeaderboards { get; set; } = new();
    public Dictionary<int, Profile> ProfileLookup { get; set; } = new();

    public async Task OnGetAsync()
    {
        Profiles = await _profileService.GetAllProfilesAsync();
        Games = _gameService.GetAllGames();
        
        ProfileLookup = Profiles.ToDictionary(p => p.Id, p => p);

        foreach (var game in Games)
        {
            var leaderboard = await _gameService.GetLeaderboardAsync(game.GameType);
            GameLeaderboards[game.GameType] = leaderboard;
        }
    }
}