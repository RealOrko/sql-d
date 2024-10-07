namespace SqlD.Tests.Framework;

public class BaseTestCase
{
    static BaseTestCase()
    {
        Interface.Setup(typeof(BaseTestCase).Assembly, "appsettings.json");
    }
}