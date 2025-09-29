using DailyGames.Models;

namespace DailyGames.Services;

public interface IProfileService
{
    Task<List<Profile>> GetAllProfilesAsync();
    Task<Profile?> GetProfileByIdAsync(int id);
    Task<Profile> CreateProfileAsync(string name, string? avatar = null);
    Task<bool> DeleteProfileAsync(int id);
    Task<Profile?> UpdateProfileAsync(int id, string name, string? avatar = null);
}

public class InMemoryProfileService : IProfileService
{
    private readonly List<Profile> _profiles = new();
    private int _nextId = 1;

    public Task<List<Profile>> GetAllProfilesAsync()
    {
        return Task.FromResult(_profiles.ToList());
    }

    public Task<Profile?> GetProfileByIdAsync(int id)
    {
        var profile = _profiles.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(profile);
    }

    public Task<Profile> CreateProfileAsync(string name, string? avatar = null)
    {
        var profile = new Profile
        {
            Id = _nextId++,
            Name = name,
            Avatar = avatar,
            CreatedDate = DateTime.UtcNow
        };
        
        _profiles.Add(profile);
        return Task.FromResult(profile);
    }

    public Task<bool> DeleteProfileAsync(int id)
    {
        var profile = _profiles.FirstOrDefault(p => p.Id == id);
        if (profile != null)
        {
            _profiles.Remove(profile);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<Profile?> UpdateProfileAsync(int id, string name, string? avatar = null)
    {
        var profile = _profiles.FirstOrDefault(p => p.Id == id);
        if (profile != null)
        {
            profile.Name = name;
            profile.Avatar = avatar;
            return Task.FromResult<Profile?>(profile);
        }
        return Task.FromResult<Profile?>(null);
    }
}