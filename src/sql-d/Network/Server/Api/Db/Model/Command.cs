using SqlD.Extensions;

namespace SqlD.Network.Server.Api.Db.Model;

public class Command
{
    public List<string> Commands { get; } = new();

    public void AddInsert(object instance, bool withIdentity = false)
    {
        Commands.Add(instance.GetInsert(withIdentity));
    }

    public void AddManyInserts(IEnumerable<object> instances, bool withIdentity = false)
    {
        Commands.AddRange(instances.Select(x => x.GetInsert(withIdentity)));
    }

    public void AddUpdate(object instance)
    {
        Commands.Add(instance.GetUpdate());
    }

    public void AddManyUpdates(IEnumerable<object> instances)
    {
        Commands.AddRange(instances.Select(x => x.GetUpdate()));
    }

    public void AddDelete(object instance)
    {
        Commands.Add(instance.GetDelete());
    }

    public void AddManyDeletes(IEnumerable<object> instances)
    {
        Commands.AddRange(instances.Select(x => x.GetDelete()));
    }
}