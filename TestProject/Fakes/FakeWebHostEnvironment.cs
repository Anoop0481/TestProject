using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace TestProject.Tests.Fakes;

public class FakeWebHostEnvironment : IWebHostEnvironment
{
    public string ContentRootPath { get; set; } = Path.GetTempPath();
    public string EnvironmentName { get; set; } = "Test";
    public string ApplicationName { get; set; } = "TestApp";
    public string WebRootPath { get; set; } = Path.GetTempPath();
    public IFileProvider ContentRootFileProvider { get; set; } = default!;
    public IFileProvider WebRootFileProvider { get; set; } = default!;
}