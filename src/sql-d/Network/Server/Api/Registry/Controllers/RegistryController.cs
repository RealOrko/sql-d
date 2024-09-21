using Microsoft.AspNetCore.Mvc;
using SqlD.Logging;
using SqlD.Network.Server.Api.Registry.Model;

namespace SqlD.Network.Server.Api.Registry.Controllers;

[ApiController]
[Route("api/registry")]
public class RegistryController : Controller
{
    private readonly EndPoint authorityAddress;
    private readonly DbConnectionFactory dbConnectionFactory;

    public RegistryController(DbConnectionFactory dbConnectionFactory, EndPoint serverAddress)
    {
        this.dbConnectionFactory = dbConnectionFactory;
        using (var dbConnection = dbConnectionFactory.Connect())
        {
            dbConnection.CreateTable<RegistryEntry>();
        }
        authorityAddress = serverAddress;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return this.Intercept(() =>
        {
            using (var dbConnection = dbConnectionFactory.Connect())
            {
                var registrationResponse = new RegistrationResponse
                {
                    Authority = authorityAddress,
                    Registry = dbConnection.Query<RegistryEntry>().OrderBy(x => x.Id).ToList()
                };
                return Ok(registrationResponse);
            }
        });
    }

    [HttpPost]
    public IActionResult Register([FromBody] Registration registration)
    {
        return this.Intercept(() =>
        {
            RegisterOrUpdateEntry(registration);
            using (var dbConnection = dbConnectionFactory.Connect())
            {
                var registrationResponse = new RegistrationResponse
                {
                    Authority = authorityAddress,
                    Registration = registration,
                    Registry = dbConnection.Query<RegistryEntry>().OrderByDescending(x => x.Id).ToList()
                };
                return Ok(registrationResponse);
            }
        });
    }

    [HttpPost("unregister")]
    public IActionResult Unregister([FromBody] Registration registration)
    {
        return this.Intercept(() =>
        {
            UnregisterOrDeleteEntry(registration);
            using (var dbConnection = dbConnectionFactory.Connect())
            {
                var registrationResponse = new RegistrationResponse
                {
                    Authority = authorityAddress,
                    Registration = registration,
                    Registry = dbConnection.Query<RegistryEntry>().OrderByDescending(x => x.Id).ToList()
                };
                return Ok(registrationResponse);
            }
        });
    }

    private void RegisterOrUpdateEntry(Registration registration)
    {
        using (var dbConnection = dbConnectionFactory.Connect())
        {
            dbConnection.CreateTable<RegistryEntry>();
            UnregisterOrDeleteEntry(registration);
            dbConnection.Insert(new RegistryEntry
            {
                Name = registration.Name,
                Database = registration.Database,
                Uri = registration.Source.ToUrl(),
                Tags = string.Join(",", registration.Tags.Select(x => x.Trim())),
                LastUpdated = DateTime.UtcNow,
                AuthorityUri = authorityAddress.ToUrl()
            });
        }
    }

    private void UnregisterOrDeleteEntry(Registration registration)
    {
        using (var dbConnection = dbConnectionFactory.Connect())
        {
            dbConnection.CreateTable<RegistryEntry>();
            var entries = dbConnection.Query<RegistryEntry>($"WHERE Uri = '{registration.Source.ToUrl()}'").ToList();
            if (entries != null && entries.Any()) dbConnection.DeleteMany(entries);
        }
    }
}