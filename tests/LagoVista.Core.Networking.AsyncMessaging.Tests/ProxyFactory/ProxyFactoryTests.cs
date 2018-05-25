using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Networking.AsyncMessaging.Tests.ProxyFactory
{
    [TestClass]
    public sealed class ProxyFactoryTests
    {

        [TestMethod]
        public void CreateProxy()
        {

        }

    }
}



protected void WriteResult(ListResponse<DataStreamResult> response)
{
    var idx = 1;
    foreach (var item in response.Model)
    {
        Console.WriteLine($"Record {idx++} - {item.Timestamp}");

        foreach (var fld in item)
        {
            Console.WriteLine($"\t{fld.Key} - {fld.Value}");
        }
        Console.WriteLine("----");
        Console.WriteLine();
    }
}

protected void AssertInvalidError(InvokeResult result, params string[] errs)
{
    Console.WriteLine("Errors (at least some are expected)");

    foreach (var err in result.Errors)
    {
        Console.WriteLine(err.Message);
    }

    foreach (var err in errs)
    {
        Assert.IsTrue(result.Errors.Where(msg => msg.Message == err).Any(), $"Could not find error [{err}]");
    }

    Assert.AreEqual(errs.Length, result.Errors.Count, "Validation error mismatch between");

    Assert.IsFalse(result.Successful, "Validated as successful but should have failed.");
}

protected void AssertSuccessful(InvokeResult result)
{
    if (result.Errors.Any())
    {
        Console.WriteLine("unexpected errors");
    }

    foreach (var err in result.Errors)
    {
        Console.WriteLine("\t" + err.Message);
    }

    Assert.IsTrue(result.Successful);
}
