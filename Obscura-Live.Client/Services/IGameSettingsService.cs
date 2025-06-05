using Obscura_Live.Client.Components.UI;
using Obscura_Live.Client.Models;

namespace Obscura_Live.Client.Services
{
    public interface IGameSettingsService
    {
        Task<GameSettingsModel> GetGameSettings();
        Task SaveGameSettingsAsync(GameSettingsModel gameSettings);
        
    }
}
