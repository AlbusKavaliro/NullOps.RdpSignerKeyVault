using NullOps.RdpSigner;
using Spectre.Console;
using Spectre.Console.Cli;
using System;

var app = new CommandApp<SignCommand>();
app.Configure(config =>
{
    config.SetApplicationName("rdpsign");
});

try
{
    return await app.RunAsync(args).ConfigureAwait(false);
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
    return 1;
}
