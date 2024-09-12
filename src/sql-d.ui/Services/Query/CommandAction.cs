using System;
using System.Threading.Tasks;
using SqlD.Network.Client;
using SqlD.Network.Server.Api.Db.Model;
using SqlD.UI.Models.Query;

namespace SqlD.UI.Services.Query;

public class CommandAction : IQueryAction
{
    public async Task<object> Go(string query, ConnectionClient client)
    {
        var c = new Command();
        c.Commands.Add(query);
        try
        {
            var res = await client.PostCommandAsync(c);
            return new CommandResultViewModel(res);
        }
        catch (Exception err)
        {
            return new CommandResultViewModel(err.Message);
        }
    }
}