using GhostNetwork.Content.Api;
using GhostNetwork.Content.Model;
using GhostNetwork.Gateway.GraphQL.Models;

namespace GhostNetwork.Gateway.GraphQL.DataLoaders
{
    public class ReactionsByPublicationIdDataLoader : BatchDataLoader<string, IEnumerable<ReactionsEntity>>
    {
        private readonly IReactionsApi reactionsApi;

        public ReactionsByPublicationIdDataLoader(
            IReactionsApi reactionsApi,
            IBatchScheduler batchScheduler,
            DataLoaderOptions? options = null) : base(batchScheduler, options)
        {
            this.reactionsApi = reactionsApi;
        }

        protected override async Task<IReadOnlyDictionary<string, IEnumerable<ReactionsEntity>>> LoadBatchAsync(IReadOnlyList<string> keys, CancellationToken cancellationToken)
        {
            var reactions = await reactionsApi.GetGroupedReactionsAsync(new ReactionsQuery(keys.ToList()), cancellationToken: cancellationToken);

            var grouppedReactions = reactions.ToDictionary(k => k.Key, v => v.Value.Select(x => new ReactionsEntity
            {
                Type = x.Key,
                TotalCount = x.Value,
            }));

            return keys.ToDictionary(
                key => key, 
                key => 
                    grouppedReactions.ContainsKey(key)
                        ? grouppedReactions[key]
                        : Enumerable.Empty<ReactionsEntity>());
        }
    }
}
