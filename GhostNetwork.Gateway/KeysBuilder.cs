namespace GhostNetwork.Gateway
{
    public static class KeysBuilder
    {
        public static string PublicationCommentKey(string publicationId) => $"publication_{publicationId}";
        public static string PublicationFromCommentKey(string key) => key[11..];
        public static string PublicationReactionsKey(string publicationId) => $"publication_{publicationId}";
    }
}