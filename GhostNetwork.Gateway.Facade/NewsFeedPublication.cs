namespace GhostNetwork.Gateway.Facade
{
    public class NewsFeedPublication
    {
        public NewsFeedPublication(string content, int? quantityComments)
        {
            Content = content;
            QuantityComments = quantityComments;
        }

        public string Content { get; }

        public int? QuantityComments { get; }
    }
}