using MinecraftServerlist.Services.Abstractions;

namespace MinecraftServerlist.Services.Implementation;

public sealed class AdminSuggestionService : IAdminSuggestionService
{
    private readonly IList<(string title, string description)> _suggestionList;

    public AdminSuggestionService()
    {
        _suggestionList = new List<(string title, string description)>();
    }

    public IEnumerable<(string title, string description)> GetSuggestions() => _suggestionList;

    public void AddSuggestion(string title, string description)
    {
        var tuple = (title, description);
        _suggestionList.Add(tuple);
    }
}