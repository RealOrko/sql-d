using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SqlD.Builders;
using SqlD.Configs.Model;
using SqlD.Tests.Framework.Models;

namespace SqlD.Tests;

[TestFixture]
public class DbConnectionLoadTests
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var connection = new NewDbBuilder().ConnectedTo("sql-d.db", SqlDPragmaModel.Default);
        connection.CreateTable<AnyTableB>();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        var connection = new NewDbBuilder().ConnectedTo("sql-d.db", SqlDPragmaModel.Default);
        connection.DropTable<AnyTableB>();
    }

    [Test]
    public void ShouldBeAbleToSustainMultithreadedReadWrites()
    {
        var numberOfIterations = 10000;

        var waitForReads1 = new ManualResetEvent(false);
        var waitForReads2 = new ManualResetEvent(false);
        var waitForReads3 = new ManualResetEvent(false);

        var waitForWrites1 = new ManualResetEvent(false);
        var waitForWrites2 = new ManualResetEvent(false);
        var waitForWrites3 = new ManualResetEvent(false);

        // Writes
        var insertInstance = GenFu.GenFu.New<AnyTableB>();

        var task1 = Task.Factory.StartNew(() =>
        {
            try
            {
                var connection = new NewDbBuilder().ConnectedTo("sql-d.db", SqlDPragmaModel.Default);
                for (var counter = 0; counter < numberOfIterations; counter++)
                {
                    connection.Insert(insertInstance);
                    Console.Write("+");
                }
            }
            finally
            {
                waitForWrites1.Set();
            }
        });

        var task2 = Task.Factory.StartNew(() =>
        {
            try
            {
                var connection = new NewDbBuilder().ConnectedTo("sql-d.db", SqlDPragmaModel.Default);
                for (var counter = 0; counter < numberOfIterations; counter++)
                {
                    connection.Insert(insertInstance);
                    Console.Write("+");
                }
            }
            finally
            {
                waitForWrites2.Set();
            }
        });

        var task3 = Task.Factory.StartNew(() =>
        {
            try
            {
                var connection = new NewDbBuilder().ConnectedTo("sql-d.db", SqlDPragmaModel.Default);
                for (var counter = 0; counter < numberOfIterations; counter++)
                {
                    connection.Insert(insertInstance);
                    Console.Write("+");
                }
            }
            finally
            {
                waitForWrites3.Set();
            }
        });

        // Reads
        var task4 = Task.Factory.StartNew(() =>
        {
            try
            {
                var connection = new NewDbBuilder().ConnectedTo("sql-d.db", SqlDPragmaModel.Default);
                for (var counter = 0; counter < numberOfIterations; counter++)
                {
                    var results = connection.Query<AnyTableB>("LIMIT 5");
                    Console.Write(".");
                }
            }
            finally
            {
                waitForReads1.Set();
            }
        });

        var task5 = Task.Factory.StartNew(() =>
        {
            try
            {
                var connection = new NewDbBuilder().ConnectedTo("sql-d.db", SqlDPragmaModel.Default);
                for (var counter = 0; counter < numberOfIterations; counter++)
                {
                    var results = connection.Query<AnyTableB>("LIMIT 5");
                    Console.Write(".");
                }
            }
            finally
            {
                waitForReads2.Set();
            }
        });

        var task6 = Task.Factory.StartNew(() =>
        {
            try
            {
                var connection = new NewDbBuilder().ConnectedTo("sql-d.db", SqlDPragmaModel.Default);
                for (var counter = 0; counter < numberOfIterations; counter++)
                {
                    connection.Query<AnyTableB>("LIMIT 5");
                    Console.Write(".");
                }
            }
            finally
            {
                waitForReads3.Set();
            }
        });

        waitForReads1.WaitOne();
        waitForReads2.WaitOne();
        waitForReads3.WaitOne();

        waitForWrites1.WaitOne();
        waitForWrites2.WaitOne();
        waitForWrites3.WaitOne();

        Assert.That(task1.IsFaulted, Is.False, $"{task1.Exception?.Message}");
        Assert.That(task2.IsFaulted, Is.False, $"{task2.Exception?.Message}");
        Assert.That(task3.IsFaulted, Is.False, $"{task3.Exception?.Message}");
        Assert.That(task4.IsFaulted, Is.False, $"{task4.Exception?.Message}");
        Assert.That(task5.IsFaulted, Is.False, $"{task5.Exception?.Message}");
        Assert.That(task6.IsFaulted, Is.False, $"{task6.Exception?.Message}");
    }
}