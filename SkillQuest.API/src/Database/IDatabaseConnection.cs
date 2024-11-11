using System.Text.Json.Nodes;

namespace SkillQuest.API.Database;

public interface IDatabaseConnection{
    public Task<IEnumerable<IEnumerable<KeyValuePair<string, object>>>> Query(
        string query,
        IEnumerable<KeyValuePair<string, object>> parameters = null
    );
}
