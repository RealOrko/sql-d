using NUnit.Framework;
using SqlD.Options;

namespace SqlD.Tests.Options;

[TestFixture]
public class ArgumentsParserTests
{
    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public void Should_be_able_to_Parse_single_args_as_strings_properly()
    {
        var arguments = ArgumentsParser.Parse<Arguments>(new[] { "-s", "localhost:8080" });
        Assert.That(arguments.Service, Is.EqualTo("localhost:8080"));
    }

    [Test]
    public void Should_be_able_to_Parse_single_args_as_bools_properly()
    {
        var arguments = ArgumentsParser.Parse<Arguments>(new[] { "-w" });
        Assert.That(arguments.Wait, Is.True);
    }

    [Test]
    public void Should_be_able_to_Parse_multi_args_properly()
    {
        var arguments = ArgumentsParser.Parse<Arguments>(new[] { "-r", "localhost:8080", "-r", "localhost:8081", "-r", "localhost:8082" });
        Assert.That(arguments.Registries, Contains.Item("localhost:8080"));
        Assert.That(arguments.Registries, Contains.Item("localhost:8081"));
        Assert.That(arguments.Registries, Contains.Item("localhost:8082"));
    }
}