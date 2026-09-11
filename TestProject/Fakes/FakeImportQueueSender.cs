namespace TestProject.Tests.Fakes;

public class FakeImportQueueSender : IImportQueueSender
{
    public List<ImportJobMessage> SentMessages { get; } = new();

    public Task SendAsync(ImportJobMessage message)
    {
        SentMessages.Add(message);
        return Task.CompletedTask;
    }
}