
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace TestProject.Tests.ControllerTests;

public class ImportControllerTests
{
    private AppDbContext CreateInMemoryDb() =>
        new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Upload_NoFile_ReturnsBadRequest()
    {
        var controller = new ImportController(CreateInMemoryDb(), new FakeImportQueueSender(), new FakeWebHostEnvironment());

        var result = await controller.Upload(null!, "Employee");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Upload_ValidFile_CreatesJobAndSendsMessage()
    {
        var db = CreateInMemoryDb();
        var fakeSender = new FakeImportQueueSender();

        var content = "SupplierId,SupplierName\nS1,Acme";
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        IFormFile file = new FormFile(stream, 0, bytes.Length, "file", "test.csv");

        var controller = new ImportController(db, fakeSender, new FakeWebHostEnvironment());

        var result = await controller.Upload(file, "Supplier");

        Assert.IsType<OkObjectResult>(result);
        Assert.Single(db.ImportJobs);
        Assert.Single(fakeSender.SentMessages);
        Assert.Equal("Supplier", fakeSender.SentMessages[0].DataType);
    }
}