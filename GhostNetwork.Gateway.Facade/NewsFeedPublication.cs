namespace GhostNetwork.Gateway.Facade
{
    public class NewsFeedPublication
    {
        public NewsFeedPublication(string content)
        {
            Content = content;
        }

        public string Content { get; }
    }
}