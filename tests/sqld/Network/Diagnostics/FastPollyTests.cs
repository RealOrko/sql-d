using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SqlD.Network.Diagnostics;

namespace SqlD.Tests.Network.Diagnostics;

[TestFixture]
public class FastPollyTests
{
    [Test]
    public async Task ShouldBeAbleToRetry3TimesForAnException()
    {
        var numberOfExecutions = 0;

        var policy = FastPolly
            .Handle<ArgumentException>()
            .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(retry));

        await policy.ExecuteAsync(async () =>
        {
            numberOfExecutions++;

            if (numberOfExecutions < 3) throw new ArgumentException();

            return await Task.FromResult(0);
        });

        Assert.That(numberOfExecutions, Is.EqualTo(3));
    }

    [Test]
    public async Task ShouldBeAbleToRetryForInheritedExceptions()
    {
        var numberOfExecutions = 0;

        var policy = FastPolly
            .Handle<Child1Exception>()
            .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(retry));

        await policy.ExecuteAsync(async () =>
        {
            numberOfExecutions++;

            if (numberOfExecutions < 3) throw new Child2Exception();

            return await Task.FromResult(0);
        });

        Assert.That(numberOfExecutions, Is.EqualTo(3));
    }

    [Test]
    public void ShouldThrowIfExceptionTypesHandledDoNotMatch()
    {
        var policy = FastPolly
            .Handle<ArgumentException>()
            .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(retry));

        Assert.ThrowsAsync<InvalidOperationException>(async () => { await policy.ExecuteAsync<int>(async () => { throw await Task.FromResult(new InvalidOperationException()); }); });
    }

    public class ParentException : Exception
    {
    }

    public class Child1Exception : ParentException
    {
    }

    public class Child2Exception : Child1Exception
    {
    }
}