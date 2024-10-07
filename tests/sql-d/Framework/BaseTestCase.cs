namespace SqlD.Tests.Framework;

public class BaseTestCase
{
    protected const string ConfigurationFileName = "appsettings.json";

    static BaseTestCase()
    {
        Interface.Setup(typeof(NetworkTestCase).Assembly, ConfigurationFileName);
    }
}