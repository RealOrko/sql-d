using System;
using System.Threading;
using SqlD.Configs.Model;
using SqlD.Logging;
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

    public SettingsResultModel WriteConfig(string config)
    {
        var resultModel = new SettingsResultModel();

        try
        {
            var configModel = JsonSerialiser.Deserialise<SqlDConfiguration>(config);
            if (configModel == null)
            {
                throw new Exception("Sorry invalid configuration.");
            }

            Interface.Stop();
            Configs.Configuration.Update(configModel);
            Interface.Start();
        }
        catch (Exception ex)
        {
            resultModel.Error = ex.Message + " Configuration has been reverted.";
            Configs.Configuration.Revert();
            Interface.Start();
        }

        return resultModel;
    }
}