using Microsoft.JSInterop;
using Obscura_Live.Client.Components.UI;
using Obscura_Live.Client.Models;   
using System.Text.Json;

namespace Obscura_Live.Client.Services
{
    public class GameSettingsService(IJSRuntime jsRuntime) : IGameSettingsService
    {

        private readonly string _localStorageKey = "obscuraSettings";

        public async Task<GameSettingsModel> GetGameSettings()
        {
            GameSettingsModel gameSettings = new GameSettingsModel();

            try 
            { 
              var json = await jsRuntime.InvokeAsync<string>("localStorage.getItem", _localStorageKey);
               gameSettings = JsonSerializer.Deserialize<GameSettingsModel>(json) ?? new GameSettingsModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving game settings: {ex.Message}");
            }

            return gameSettings;

        }

        public async Task SaveGameSettingsAsync(GameSettingsModel gameSettings)
        {
            try
            {
                var json = JsonSerializer.Serialize(gameSettings);
                await jsRuntime.InvokeVoidAsync("localStorage.setItem", _localStorageKey, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving game settings: {ex.Message}");
            }
        }
    }
}
