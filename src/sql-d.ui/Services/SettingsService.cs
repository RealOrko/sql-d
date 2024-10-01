using System;
using System.Threading;
using SqlD.Configs.Model;
using SqlD.Serialiser;
using SqlD.UI.Models.Settings;

namespace SqlD.UI.Services;

public class SettingsService
{
    public SettingsReadModel ReadConfig()
    {
        var settingsModel = new SettingsReadModel();
        settingsModel.Data = JsonSerialiser.Serialise(Configs.Configuration.Instance);
        return settingsModel;
    }

    public void WriteConfig(string config)
    {
        var configModel = JsonSerialiser.Deserialise<SqlDConfiguration>(config);
        if (configModel == null)
        {
            throw new Exception("Sorry invalid configuration.");
        }
        
        Configs.Configuration.Update(configModel);
        
        Interface.Stop();
        Interface.Start();
    }
}