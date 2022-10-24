namespace GhostNetwork.Gateway;

public class ContextProvider
{
    public ContextProvider(string correlationId)
    {
        CorrelationId = correlationId;
    }

    public string CorrelationId { get; }
}