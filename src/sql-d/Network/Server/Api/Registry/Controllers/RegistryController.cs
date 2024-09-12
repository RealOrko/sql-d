using Microsoft.AspNetCore.Mvc;
using SqlD.Logging;
using SqlD.Network.Server.Api.Registry.Model;

namespace SqlD.Network.Server.Api.Registry.Controllers;

[ApiController]
[Route("api/registry")]
public class RegistryController : Controller
{
    private readonly EndPoint authorityAddress;
    private readonly DbConnection dbConnection;

    public RegistryController(DbConnection dbConnection, EndPoint serverAddress)
    {
        this.dbConnection = dbConnection;
        this.dbConnection.CreateTable<RegistryEntry>();
        authorityAddress = serverAddress;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return this.Intercept(() =>
        {
            var registrationResponse = new RegistrationResponse
            {
                Authority = authorityAddress,
                Registry = dbConnection.Query<RegistryEntry>().OrderBy(x => x.Id).ToList()
            };

            return Ok(registrationResponse);
        });
    }

    [HttpPost]
    public IActionResult Register([FromBody] Registration registration)
    {
        return this.Intercept(() =>
        {
            RegisterOrUpdateEntry(registration);
            var registrationResponse = new RegistrationResponse
            {
                Authority = authorityAddress,
                Registration = registration,
                Registry = dbConnection.Query<RegistryEntry>().OrderByDescending(x => x.Id).ToList()
            };

            return Ok(registrationResponse);
        });
    }

    [HttpPost("unregister")]
    public IActionResult Unregister([FromBody] Registration registration)
    {
        return this.Intercept(() =>
        {
            UnregisterOrDeleteEntry(registration);
            var registrationResponse = new RegistrationResponse
            {
                Authority = authorityAddress,
                Registration = registration,
                Registry = dbConnection.Query<RegistryEntry>().OrderByDescending(x => x.Id).ToList()
            };

            return Ok(registrationResponse);
        });
    }

    private void RegisterOrUpdateEntry(Registration registration)
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

    private void UnregisterOrDeleteEntry(Registration registration)
    {
        dbConnection.CreateTable<RegistryEntry>();
        var entries = dbConnection.Query<RegistryEntry>($"WHERE Uri = '{registration.Source.ToUrl()}'").ToList();
        if (entries != null && entries.Any()) dbConnection.DeleteMany(entries);
    }
}