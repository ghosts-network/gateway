using GraphQL.Types;

namespace GhostNetwork.Gateway.Api.GraphQL
{
    public class GhostNetworkSchema : Schema
    {
        public GhostNetworkSchema()
        {
            Query = new GhostNetworkQuery();
        }
    }

    public class GhostNetworkQuery : ObjectGraphType
    {
        public GhostNetworkQuery()
        {
            Field<ListGraphType<PublicationType>>("publications", resolve: context => new []
            {
                new Publication("sdads asd asd"),
                new Publication("sds asd asd"),
                new Publication("sd asd"),
                new Publication("s"),
                new Publication("ads asd asd")
            });
        }
    }

    public class PublicationType : ObjectGraphType<Publication>
    {
        public PublicationType()
        {
            Field(o => o.Content);
        }
    }

    public class Publication
    {
        public Publication(string content)
        {
            Content = content;
        }

        public string Content { get; }
    }
}