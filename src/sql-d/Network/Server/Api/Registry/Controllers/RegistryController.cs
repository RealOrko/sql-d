﻿using Microsoft.AspNetCore.Mvc;
using SqlD.Extensions.Network.Server;
using SqlD.Logging;
using SqlD.Network.Server.Api.Registry.Model;

namespace SqlD.Network.Server.Api.Registry.Controllers;

[ApiController]
[Route("api/registry")]
public class RegistryController : Controller
{
    private readonly EndPoint _authorityAddress;
    private readonly DbConnectionFactory _dbConnectionFactory;

    public RegistryController(DbConnectionFactory dbConnectionFactory, EndPoint serverAddress)
    {
        _dbConnectionFactory = dbConnectionFactory;
        using (var dbConnection = dbConnectionFactory.Connect())
        {
            dbConnection.CreateTable<RegistryEntry>();
        }
        _authorityAddress = serverAddress;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return this.Intercept(() =>
        {
            using (var dbConnection = _dbConnectionFactory.Connect())
            {
                var registrationResponse = new RegistrationResponse
                {
                    Authority = _authorityAddress,
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
            using (var dbConnection = _dbConnectionFactory.Connect())
            {
                var registrationResponse = new RegistrationResponse
                {
                    Authority = _authorityAddress,
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
            using (var dbConnection = _dbConnectionFactory.Connect())
            {
                var registrationResponse = new RegistrationResponse
                {
                    Authority = _authorityAddress,
                    Registration = registration,
                    Registry = dbConnection.Query<RegistryEntry>().OrderByDescending(x => x.Id).ToList()
                };
                return Ok(registrationResponse);
            }
        });
    }

    private void RegisterOrUpdateEntry(Registration registration)
    {
        using (var dbConnection = _dbConnectionFactory.Connect())
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
                AuthorityUri = _authorityAddress.ToUrl()
            });
        }
    }

    private void UnregisterOrDeleteEntry(Registration registration)
    {
        using (var dbConnection = _dbConnectionFactory.Connect())
        {
            dbConnection.CreateTable<RegistryEntry>();
            var entries = dbConnection.Query<RegistryEntry>($"WHERE Uri = '{registration.Source.ToUrl()}'").ToList();
            if (entries != null && entries.Any()) dbConnection.DeleteMany(entries);
        }
    }
}