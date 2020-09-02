### Generation http client for publications

openapi-generator generate -i http://localhost:5300/swagger/v1/swagger.json \
    -g csharp-netcore \
    -o ./http \
    --additional-properties=packageName=GhostNetwork.Publications \
    --additional-properties=netCoreProjectFile=true

mkdir Infrastructure/Http
cp -r ./http/src/GhostNetwork.Publications ./Infrastructure/Http/GhostNetwork.Publications

rm -r ./http
