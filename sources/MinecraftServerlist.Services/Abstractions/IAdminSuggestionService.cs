namespace MinecraftServerlist.Services.Abstractions;

public interface IAdminSuggestionService
{
    public IEnumerable<(string title, string description)> GetSuggestions();

    public void AddSuggestion(string title, string description);
}