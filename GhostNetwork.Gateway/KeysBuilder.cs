namespace GhostNetwork.Gateway
{
    public static class KeysBuilder
    {
        public static string PublicationReactionsKey(string publicationId) => $"publication_{publicationId}";
    }
}