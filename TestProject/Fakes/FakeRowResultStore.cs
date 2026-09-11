namespace TestProject.Tests.Fakes;

public class FakeRowResultStore : IRowResultStore
{
    public List<RowResult> Results { get; } = new();

    public Task InsertAsync(RowResult result)
    {
        Results.Add(result);
        return Task.CompletedTask;
    }

    public Task<List<RowResult>> GetFailedByJobAsync(Guid jobId) =>
        Task.FromResult(Results.Where(r => r.JobId == jobId && !r.Passed).ToList());
}