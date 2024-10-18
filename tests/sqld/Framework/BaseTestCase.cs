namespace SqlD.Tests.Framework;

public class BaseTestCase
{
    static BaseTestCase()
    {
        #if Linux 
        Interface.Setup(typeof(BaseTestCase).Assembly, "appsettings-linux-x64.json");
        #elif Windows 
        Interface.Setup(typeof(BaseTestCase).Assembly, "appsettings-win-x64.json");
        #endif
    }
}