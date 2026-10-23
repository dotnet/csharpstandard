using System.Diagnostics;
using System.Text.Json;
using MarkdownConverter.Spec;
using Xunit;

namespace MarkdownConverter.Tests;

public class DiagnosticReportTests : IDisposable
{
    private readonly string tempDirectory =
        Directory.CreateTempSubdirectory("MarkdownConverterReportTests-").FullName;

    [Theory]
    [InlineData("arguments", null)]
    [InlineData("missing-file", null)]
    [InlineData("conversion", "MDC027")]
    [InlineData("reading", "MDC033")]
    public async Task FailuresWriteReportsAndKeepNonzeroExitCodes(
        string failure, string? diagnosticId)
    {
        var reportPath = Path.Combine(tempDirectory, "reports", "converter.json");
        await File.WriteAllTextAsync(Path.Combine(tempDirectory, "template.docx"), "Not a Word document");
        await File.WriteAllTextAsync(Path.Combine(tempDirectory, "invalid.md"),
            "Not a heading\n- A list without a preceding blank line\n");
        string[] args = failure switch
        {
            "arguments" => [],
            "missing-file" => ["missing.md"],
            "conversion" => ["template.docx", "-o", "output.docx"],
            "reading" => ["invalid.md", "template.docx", "-o", "output.docx"],
            _ => throw new ArgumentException("Unknown failure scenario", nameof(failure))
        };

        var result = await RunConverterAsync(reportPath, args);
        using var report = JsonDocument.Parse(await File.ReadAllTextAsync(reportPath));

        Assert.NotEqual(0, result.ExitCode);
        Assert.Equal("Markdown to Word Converter", report.RootElement.GetProperty("tool").GetString());
        Assert.False(report.RootElement.GetProperty("success").GetBoolean());
        var diagnostics = report.RootElement.GetProperty("diagnostics").EnumerateArray().ToArray();
        if (diagnosticId is null)
        {
            Assert.Empty(diagnostics);
        }
        else
        {
            Assert.Contains(diagnostics, d => d.GetProperty("id").GetString() == diagnosticId);
            Assert.Contains("::error ", result.Output);
        }
    }

    [Fact]
    public async Task ExecutionWithoutReportPathNeedsNoAuthentication()
    {
        var result = await RunConverterAsync(null, ["missing.md"]);

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("Not found", result.Error);
        Assert.Empty(Directory.GetFiles(tempDirectory));
    }

    private async Task<(int ExitCode, string Output, string Error)> RunConverterAsync(
        string? reportPath, string[] args)
    {
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = tempDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add(typeof(Reporter).Assembly.Location);
        foreach (var arg in args)
        {
            startInfo.ArgumentList.Add(arg);
        }
        startInfo.Environment.Remove("GH_TOKEN");
        startInfo.Environment.Remove("HEAD_SHA");
        startInfo.Environment.Remove("DIAGNOSTICS_REPORT_PATH");
        startInfo.Environment["GITHUB_ACTIONS"] = "true";
        if (reportPath is not null)
        {
            startInfo.Environment["DIAGNOSTICS_REPORT_PATH"] = reportPath;
        }

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Could not start the converter");
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        try
        {
            await process.WaitForExitAsync(timeout.Token);
            return (process.ExitCode, await output, await error);
        }
        finally
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                await process.WaitForExitAsync();
            }
        }
    }

    public void Dispose() => Directory.Delete(tempDirectory, recursive: true);
}
