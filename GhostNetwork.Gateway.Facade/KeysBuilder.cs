namespace GhostNetwork.Gateway.Facade
{
    public static class KeysBuilder
    {
        public static string PublicationReactionsKey(string publicationId) => $"publication_{publicationId}";
    }
}