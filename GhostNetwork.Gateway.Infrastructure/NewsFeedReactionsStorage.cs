using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Content.Api;
using GhostNetwork.Gateway.NewsFeed;

namespace GhostNetwork.Gateway.Infrastructure
{
    public class NewsFeedReactionsStorage : INewsFeedReactionsStorage
    {
        private readonly IReactionsApi reactionsApi;

        public NewsFeedReactionsStorage(IReactionsApi reactionsApi)
        {
            this.reactionsApi = reactionsApi;
        }

        public async Task<ReactionShort> AddOrUpdateAsync(string publicationId, ReactionType reactionType, string userId)
        {
            var result = await reactionsApi.UpsertAsync(
                KeysBuilder.PublicationReactionsKey(publicationId),
                reactionType.ToString(),
                userId);

            return ToReactionShort(result, new UserReaction(reactionType));
        }

        public async Task<ReactionShort> RemoveAsync(string publicationId, string userId)
        {
            var result = await reactionsApi.DeleteByAuthorAsync(
                KeysBuilder.PublicationReactionsKey(publicationId),
                userId);

            return ToReactionShort(result, null);
        }

        public async Task RemoveManyAsync(string key)
        {
            await reactionsApi.DeleteAsync(key);
        }

        private static ReactionShort ToReactionShort(Dictionary<string, int> response, UserReaction userReaction)
        {
            var reactions = response.Keys
                .Select(k => (Enum.Parse<ReactionType>(k), response[k]))
                .ToDictionary(o => o.Item1, o => o.Item2);

            return new ReactionShort(reactions, userReaction);
        }
    }
}