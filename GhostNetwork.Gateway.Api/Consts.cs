namespace GhostNetwork.Gateway.Api
{
    public static class Consts
    {
        public static class Headers
        {
            public const string TotalCount = "X-TotalCount";
            public const string HasMore = "X-HasMore";

            public static string[] All => new[]
            {
                TotalCount,
                HasMore
            };
        }
    }
}