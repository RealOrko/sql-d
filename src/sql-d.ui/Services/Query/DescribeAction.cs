using System;
using System.Linq;
using System.Threading.Tasks;
using SqlD.Network;
using SqlD.Network.Client;
using SqlD.Network.Server.Api.Db.Model;
using SqlD.UI.Models.Query;

namespace SqlD.UI.Services.Query;

public class DescribeAction : IQueryAction
{
    public async Task<object> Go(string query, ConnectionClient client)
    {
        var describe = query.Split("?", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

        Describe describeRequest;
        if (describe.Any())
            describeRequest = new Describe { TableName = describe[0] };
        else
            describeRequest = new Describe { TableName = "sqlite_master" };

        try
        {
            var describeResponse = await client.DescribeCommandAsync(describeRequest);
            return new DescribeResultViewModel(describeResponse);
        }
        catch (Exception err)
        {
            return new DescribeResultViewModel(client.EndPoint, err.Message);
        }
    }
}