using Microsoft.AspNetCore.Mvc;
using SqlD.Extensions;
using SqlD.Network.Server.Api.Db.Model;

namespace SqlD.Network.Server.Api.Db.Controllers;

[ApiController]
[Route("api/db")]
public class DbController : Controller
{
    private readonly EndPoint endPoint;
    private readonly DbConnection dbConnection;

    public DbController(EndPoint endPoint, DbConnection dbConnection)
    {
        this.endPoint = endPoint;
        this.dbConnection = dbConnection;
    }

    [HttpPost("describe")]
    public IActionResult Describe([FromBody] Describe describe)
    {
        return this.Intercept(() =>
        {
            var results = new List<List<object>>();

            try
            {
                using (var dbReader = dbConnection.ExecuteQuery($"pragma table_info({describe.TableName.TrimEnd(';')})"))
                {
                    var reader = dbReader.Reader;
                    while (reader.Read())
                    {
                        var rowResults = new List<object>();
                        foreach (var column in Model.Describe.DescribeColumns)
                        {
                            var columnOrdinal = reader.GetOrdinal(column);
                            rowResults.Add(reader.GetValue(columnOrdinal));
                        }

                        results.Add(rowResults);
                    }

                    return Ok(DescribeResponse.Ok(endPoint, describe, results));
                }
            }
            catch (Exception err)
            {
                return Ok(DescribeResponse.Failed(endPoint, err.Message));
            }
        });
    }

    [HttpPost("query")]
    public IActionResult Query([FromBody] Query query)
    {
        return this.Intercept(() =>
        {
            var results = new List<List<object>>();

            try
            {
                using (var dbReader = dbConnection.ExecuteQuery(query.Select))
                {
                    var reader = dbReader.Reader;
                    while (reader.Read())
                    {
                        var rowResults = new List<object>();
                        foreach (var column in query.Columns)
                        {
                            var columnOrdinal = reader.GetOrdinal(column);
                            rowResults.Add(reader.GetValue(columnOrdinal));
                        }

                        results.Add(rowResults);
                    }

                    return Ok(QueryResponse.Ok(endPoint, query, results));
                }
            }
            catch (Exception err)
            {
                return Ok(QueryResponse.Failed(endPoint, err.Message));
            }
        });
    }

    [HttpPost("scalar")]
    public IActionResult Scalar([FromBody] Command command)
    {
        return this.Intercept(() =>
        {
            try
            {
                var results = dbConnection.ExecuteScalars<long>(command.Commands);
                return Ok(CommandResponse.Ok(endPoint, results));
            }
            catch (Exception err)
            {
                return Ok(CommandResponse.Failed(endPoint, err.Message));
            }
        });
    }

    [HttpPost("command")]
    public IActionResult Command([FromBody] Command command)
    {
        return this.Intercept(() =>
        {
            try
            {
                dbConnection.ExecuteCommands(command.Commands);
                return Ok(CommandResponse.Ok(endPoint));
            }
            catch (Exception err)
            {
                return Ok(CommandResponse.Failed(endPoint, err.Message));
            }
        });
    }
}