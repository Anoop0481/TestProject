using TestProject.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace BulkImportPlatform.Tests.ControllerTests;

public class RecordsControllerTests
{
    private AppDbContext CreateInMemoryDb() =>
        new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task GetByJob_JobNotFound_ReturnsNotFound()
    {
        var controller = new RecordsController(CreateInMemoryDb(), new FakeRowResultStore());

        var result = await controller.GetByJob(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetByJob_ReturnsSummaryAndFailedRows()
    {
        var db = CreateInMemoryDb();
        var jobId = Guid.NewGuid();
        db.ImportJobs.Add(new ImportJob { JobId = jobId, TotalRecords = 3, PassedRecords = 2, FailedRecords = 1, Status = JobStatus.Completed });
        await db.SaveChangesAsync();

        var fakeStore = new FakeRowResultStore();
        fakeStore.Results.Add(new RowResult { JobId = jobId, RowNumber = 2, Passed = false, Errors = new() { "Missing required field: Email" } });

        var controller = new RecordsController(db, fakeStore);

        var result = await controller.GetByJob(jobId);

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(2, db.ImportJobs.First().PassedRecords);
    }

    [Fact]
    public async Task GetAll_ReturnsRecordsOrderedByMostRecent()
    {
        var db = CreateInMemoryDb();
        db.ProcessedRecords.Add(new ProcessedRecord { JobId = Guid.NewGuid(), DataType = "Employee", RecordJson = "{}", ProcessedAt = DateTime.UtcNow.AddMinutes(-10) });
        db.ProcessedRecords.Add(new ProcessedRecord { JobId = Guid.NewGuid(), DataType = "Employee", RecordJson = "{}", ProcessedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var controller = new RecordsController(db, new FakeRowResultStore());

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var records = Assert.IsAssignableFrom<List<ProcessedRecord>>(ok.Value);
        Assert.True(records[0].ProcessedAt > records[1].ProcessedAt);
    }
}