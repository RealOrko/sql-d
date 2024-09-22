using Microsoft.AspNetCore.Mvc;
using SqlD.Extensions;
using SqlD.Extensions.Network.Server;
using SqlD.Network.Server.Api.Db.Model;

namespace SqlD.Network.Server.Api.Db.Controllers;

[ApiController]
[Route("api/db")]
public class DbController : Controller
{
    private readonly EndPoint _endPoint;
    private readonly DbConnectionFactory _dbConnectionFactory;

    public DbController(EndPoint endPoint, DbConnectionFactory dbConnectionFactory)
    {
        _endPoint = endPoint;
        _dbConnectionFactory = dbConnectionFactory;
    }

    [HttpPost("describe")]
    public IActionResult Describe([FromBody] Describe describe)
    {
        return this.Intercept(() =>
        {
            var results = new List<List<object>>();

            try
            {
                using (var dbConnection = _dbConnectionFactory.Connect())
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

                    return Ok(DescribeResponse.Ok(_endPoint, describe, results));
                }
            }
            catch (Exception err)
            {
                return Ok(DescribeResponse.Failed(_endPoint, err.Message));
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
                using (var dbConnection = _dbConnectionFactory.Connect())
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

                    return Ok(QueryResponse.Ok(_endPoint, query, results));
                }
            }
            catch (Exception err)
            {
                return Ok(QueryResponse.Failed(_endPoint, err.Message));
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
                using (var dbConnection = _dbConnectionFactory.Connect())
                {
                    var results = dbConnection.ExecuteScalars<long>(command.Commands);
                    return Ok(CommandResponse.Ok(_endPoint, results));
                }
            }
            catch (Exception err)
            {
                return Ok(CommandResponse.Failed(_endPoint, err.Message));
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
                using (var dbConnection = _dbConnectionFactory.Connect())
                {
                    dbConnection.ExecuteCommands(command.Commands);
                    return Ok(CommandResponse.Ok(_endPoint));
                }
            }
            catch (Exception err)
            {
                return Ok(CommandResponse.Failed(_endPoint, err.Message));
            }
        });
    }

    [HttpGet("fs")]
    public IActionResult FileStream()
    {
        return this.Intercept(() =>
        {
            using (var dbConnection = _dbConnectionFactory.Connect())
            {
                var stream = dbConnection.GetDatabaseFileStream();
                return File(stream, "application/octet-stream", dbConnection.DatabaseName);
            }
        });
    }
}