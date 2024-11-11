using System.Text.Json.Nodes;

namespace SkillQuest.API.Database;

public interface IDatabaseConnection{
    public Task<IEnumerable<Dictionary<string, object>>> Query(
        string query,
        IEnumerable<KeyValuePair<string, object>> parameters = null
    );

    public bool TableExists(string name);
}
