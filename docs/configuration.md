﻿# SqlD Help - Configuration

<div align="right">
	<a href="https://github.com/RealOrko/sql-d/blob/master/docs/_.md#sqld-help---contents">[Back to Contents]</a>
</div>

  * [Defaults](#defaults)
  * [Registries](#registries)
  * [Services](#services)
    * [Example](#services--example)
    * [Database](#services--database)
    * [Host/Port](#services--host--port)
    * [Tags](#services--tags)
    * [Pragma](#services--pragma)
    * [Forwarding](#services--forwarding)

## Defaults

<div align="right">
	<a href="#sqld-help---configuration">[Back to Top]</a>
</div>
<br/>

### No `appsettings.json`

If you look at [appsettings.json](https://github.com/RealOrko/sql-d/blob/master/src/sql-d/appsettings.json), you will notice a registry and sql-d instance
that is defaulted to `localhost` using port `5000`. You do not have to have an `appsettings.json` and you can construct configuration objects manually like below, although it is highly recommended you add one. Highly likely that the sensible defaults for the construction of these objects wont meet your requirements for upgrades. However if you need something quick, then here it is ... 

```csharp
// Class level declaration
public static SqlDConfiguration Default { get; } = new SqlDConfiguration()
{
    Enabled = true,
    Registries = new List<SqlDRegistryModel>()
    {
        new SqlDRegistryModel()
        {
            Port = 5000,
            Host = "localhost"
        }
    },
    Services = new List<SqlDServiceModel>()
    {
        new SqlDServiceModel()
        {
            Database = "localhost.db",
            Port = 5000,
            Name = "localhost",
            Host = "localhost",
            Tags = new List<string>(){ "registry", "master", "localhost" }
        }
    }
};

// In main method somewhere
Interface.Start(Default);
```

### With `appsettings.json`

This is the default scaled out configuration of a sql-d cluster build/running `sql-d.ui`. You can add this json as an appsettings.json file. Please make sure it is always copied to the output directory. 
You can activate it using the `Interface.Setup(typeof(Program).Assembly, "appsettings.json"); Interface.Start();` public APIs from the [sql-d](https://www.nuget.org/packages?q=sql-d) NuGet.

```json
"SqlD": {
    "loglevel": "info",
    "enabled": true,
    "settings": {
      "connections": {
        "strategy": "singleton"
      },
      "forwarding": {
        "allowed": true,
        "strategy": "primary"
      },
      "replication": {
        "allowed": true,
        "interval": 5,
        "delay": 30
      }
    },
    "registries": [
      {
        "host": "localhost",
        "port": 50095
      }
    ],
    "services": [
      {
        "name": "registry-1",
        "database": "registry-1.db",
        "host": "localhost",
        "port": 50095,
        "tags": [
          "registry"
        ],
        "pragma": {
          "journalMode": "OFF",
          "synchronous": "OFF",
          "tempStore": "OFF",
          "lockingMode": "OFF",
          "countChanges": "OFF",
          "pageSize": "65536",
          "cacheSize": "10000",
          "queryOnly": "OFF",
          "autoVacuum": "INCREMENTAL",
          "autoVacuumPages": "64"
        }
      },
      {
        "name": "slave-1",
        "database": "slave-1.db",
        "host": "localhost",
        "port": 50101,
        "tags": [
          "slave 1"
        ],
        "pragma": {
          "journalMode": "OFF",
          "synchronous": "OFF",
          "tempStore": "OFF",
          "lockingMode": "OFF",
          "countChanges": "OFF",
          "pageSize": "65536",
          "cacheSize": "10000",
          "queryOnly": "OFF",
          "autoVacuum": "INCREMENTAL",
          "autoVacuumPages": "64"
        },
        "forwardingTo": [
          {
            "host": "localhost",
            "port": 50102
          }
        ]
      },
      {
        "name": "slave-2",
        "database": "slave-2.db",
        "host": "localhost",
        "port": 50102,
        "tags": [
          "slave 2"
        ],
        "pragma": {
          "journalMode": "OFF",
          "synchronous": "OFF",
          "tempStore": "OFF",
          "lockingMode": "OFF",
          "countChanges": "OFF",
          "pageSize": "65536",
          "cacheSize": "10000",
          "queryOnly": "OFF",
          "autoVacuum": "INCREMENTAL",
          "autoVacuumPages": "64"
        },
        "forwardingTo": [
          {
            "host": "localhost",
            "port": 50103
          }
        ]
      },
      {
        "name": "slave-3",
        "database": "slave-3.db",
        "host": "localhost",
        "port": 50103,
        "tags": [
          "slave 3"
        ],
        "pragma": {
          "journalMode": "OFF",
          "synchronous": "OFF",
          "tempStore": "OFF",
          "lockingMode": "OFF",
          "countChanges": "OFF",
          "pageSize": "65536",
          "cacheSize": "10000",
          "queryOnly": "OFF",
          "autoVacuum": "INCREMENTAL",
          "autoVacuumPages": "64"
        }
      },
      {
        "name": "master-1",
        "database": "master-1.db",
        "host": "localhost",
        "port": 50100,
        "tags": [
          "master"
        ],
        "pragma": {
          "journalMode": "OFF",
          "synchronous": "OFF",
          "tempStore": "OFF",
          "lockingMode": "OFF",
          "countChanges": "OFF",
          "pageSize": "65536",
          "cacheSize": "10000",
          "queryOnly": "OFF",
          "autoVacuum": "INCREMENTAL",
          "autoVacuumPages": "64"
        },
        "forwardingTo": [
          {
            "host": "localhost",
            "port": 50101
          }
        ]
      }
    ]
  }
```

 *See Also*:

  - [No appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#no-appsettingsjson)
  - [With appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#with-appsettingsjson)
  - [Services](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#services)

## Registries

<div align="right">
	<a href="#sqld-help---configuration">[Back to Top]</a>
</div>
<br/>

For specifying the registry URL over HTTP to call when a SqlD instance starts up. The time will be recorded as UTC.

```json
"SqlD": {
	"registries": [{
		"host": "localhost",
		"port": 50095
	}]
}
```

 *See Also*:

  - [No appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#no-appsettingsjson)
  - [With appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#with-appsettingsjson)
  - [Services](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#services)

## Services

<div align="right">
	<a href="#sqld-help---configuration">[Back to Top]</a>
</div>
<br/>

A service defines an actual SqlD instance with a host and a port number. 

You need to be sure that any SqlD instance service you forward to is defined above the SqlD instance service with the `forwardingTo` dependency. Also be sure not to create cyclic dependencies
with your `forwardingTo` SqlD instance service definitions.

 *See Also*:

  - [No appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#no-appsettingsjson)
  - [With appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#with-appsettingsjson)
  - [Example](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#services--example)
  - [Registries](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#registries)

### Services / Example

<div align="right">
	<a href="#sqld-help---configuration">[Back to Top]</a>
</div>
<br/>

An example of a service that forwards traffic to other url's.

```json
"SqlD": {
	"services": [{
		"name": "sql-d-master-1",
		"database": "sql-d-master-1.db",
		"host": "localhost",
		"port": 50100,
		"tags": ["master"],
		"pragma": {
			"journalMode": "OFF",
			"synchronous": "OFF",
			"tempStore": "OFF",
			"lockingMode": "OFF",
			"countChanges": "OFF",
			"pageSize": "65536",
			"cacheSize": "10000",
			"queryOnly": "OFF"
		},
		"forwardingTo": [{
			"host": "localhost",
			"port": 50101
		},
		{
			"host": "localhost",
			"port": 50102
		},
		{
			"host": "localhost",
			"port": 50103
		}]
	}]
}
```

 *See Also*:

  - [No appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#no-appsettingsjson)
  - [With appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#with-appsettingsjson)
  - [Services](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#services)


### Services / Database

<div align="right">
	<a href="#sqld-help---configuration">[Back to Top]</a>
</div>
<br/>

The persisted file path of the SqlD database file.

```json
"SqlD": {
	"services": [{
		"database": "sql-d-registry-1.db"
	}]
}
```

 *See Also*:

  - [No appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#no-appsettingsjson)
  - [With appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#with-appsettingsjson)
  - [Services](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#services)

### Services / Host & Port

<div align="right">
	<a href="#sqld-help---configuration">[Back to Top]</a>
</div>
<br/>

The host/port is the uri the SqlD instance will serve traffic from. 

```json
"SqlD": {
	"services": [{
		"host": "localhost",
		"port": 50095
	}]
}
```

 *See Also*:

  - [No appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#no-appsettingsjson)
  - [With appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#with-appsettingsjson)
  - [Services](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#services)

### Services / Tags

<div align="right">
	<a href="#sqld-help---configuration">[Back to Top]</a>
</div>
<br/>

For distinguishing SqlD instances when queried via the [SqlD UI API](https://github.com/RealOrko/sql-d/blob/master/docs/sqld-ui-api.md).

```json
"SqlD": {
	"services": [{
		"tags": ["registry", "insert additional tag classification here ... "]
	}]
}
```

 *See Also*:

  - [No appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#no-appsettingsjson)
  - [With appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#with-appsettingsjson)
  - [Services](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#services)

### Services / Pragma

<div align="right">
	<a href="#sqld-help---configuration">[Back to Top]</a>
</div>
<br/>

For defining pragma values that are passed directly to sqlite. 

```json
"SqlD": {
	"services": [{
		"pragma": {
			"journalMode": "OFF",
			"synchronous": "OFF",
			"tempStore": "OFF",
			"lockingMode": "OFF",	
			"countChanges": "OFF",
			"pageSize": "65536",
			"cacheSize": "10000", 
			"queryOnly": "OFF"
		}
	}]
}
```

Here are links to the supported pragma parameters for services:

 - [Journal Mode](https://www.sqlite.org/pragma.html#pragma_journal_mode)
 - [Synchronous](https://www.sqlite.org/pragma.html#pragma_synchronous)
 - [Temporary Store](https://www.sqlite.org/pragma.html#pragma_temp_store)
 - [Locking Mode](https://www.sqlite.org/pragma.html#pragma_locking_mode)
 - [Count Changes](https://www.sqlite.org/pragma.html#pragma_count_changes)
 - [Page Size](https://www.sqlite.org/pragma.html#pragma_page_size)
 - [Cache Size](https://www.sqlite.org/pragma.html#pragma_cache_size)
 - [Query Only](https://www.sqlite.org/pragma.html#pragma_query_only)

 *See Also*:

  - [No appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#no-appsettingsjson)
  - [With appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#with-appsettingsjson)
  - [Services](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#services)

### Services / Forwarding

<div align="right">
	<a href="#sqld-help---configuration">[Back to Top]</a>
</div>
<br/>

An example of a write forwarding configuration block for a SqlD instance:

```json
"SqlD": {
	"services": [{
		"forwardingTo": [{
			"host": "localhost",
			"port": 50101
		}, {
			"host": "localhost",
			"port": 50102
		}, {
			"host": "localhost",
			"port": 50103
		}]
	}]
}
```

These are services you would like to replicate your writes to. 
 - No opinions exist yet as to how you synchronise data or avoid data corruption. 
 - The default configuration for SqlD.Start/SqlD.UI is to have `master` which replicates to `slave 1`, `slave 2` and `slave 3`. 
 - Forwarding will also retry a failing request for a total of 5 times with a linear backoff of 250ms. This cannot be changed through config just yet.

 *See Also*:

  - [No appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#no-appsettingsjson)
  - [With appsettings.json](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#with-appsettingsjson)
  - [Services](https://github.com/RealOrko/sql-d/blob/master/docs/configuration.md#services)
