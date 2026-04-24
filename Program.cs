using Spectre.Console;
using Spectre.Console.Cli;
using System;

namespace NullOps.RdpSigner;

public static class Program
{
    public static int Main(string[] args)
    {
        var app = new CommandApp<SignCommand>();
        app.Configure(config =>
        {
            config.SetApplicationName("rdpsign");
        });

        try
        {
            return app.Run(args);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 1;
        }
    }
}
