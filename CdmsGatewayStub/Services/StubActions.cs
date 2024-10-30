namespace CdmsGatewayStub.Services;

public interface IStubActions
{
    Task<TimeSpan> AddDelay();
}

public class StubActions(StubDelaysConfig stubDelays) : IStubActions
{
    public async Task<TimeSpan> AddDelay()
    {
        var delay = TimeSpan.FromMilliseconds(Random.Shared.Next(stubDelays.MinimumMs, stubDelays.MaximumMs));
        await Task.Delay(delay);
        return delay;
    }
}