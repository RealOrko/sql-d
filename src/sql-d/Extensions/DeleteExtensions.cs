using System.Text;
using SqlD.Extensions.Discovery;

namespace SqlD.Extensions;

internal static class DeleteExtensions
{
    internal static string GetDelete(this object instance)
    {
        var result = new StringBuilder();

        var type = instance.GetType();
        var idProperty = PropertyDiscovery.GetProperty("Id", type);

        var tableName = AttributeDiscovery.GetTableName(type);
        result.AppendLine($"DELETE FROM {tableName} WHERE Id = {ValueDiscovery.GetValue(instance, idProperty)}");

        return result.ToString();
    }
}