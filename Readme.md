# GhostNetwork - Gateway

Gateway is a part of GhostNetwork education project and provides public REST API

## Installation

copy provided dev-compose.yml and customize for your needs

### Parameters

| Environment          | Description                             |
|----------------------|---------------------------------------- |
| PUBLICATIONS_ADDRESS | Address of publication service instance |
| REACTIONS_ADDRESS    | Address of reactions service instance   |

## Development

To run dependent environment use `docker-compose -f dev-compose.yml up -d --build`

### Http client generation

Here is an example how to generate or update http client for publication microservice

```bash
openapi-generator generate -i http://localhost:5300/swagger/v1/swagger.json \
    -g csharp-netcore \
    -o ./http \
    --additional-properties=packageName=GhostNetwork.Publications \
    --additional-properties=netCoreProjectFile=true

rm -r ./Infrastructure/Http/GhostNetwork.Publications
mkdir ./Infrastructure/Http
cp -r ./http/src/GhostNetwork.Publications ./Infrastructure/Http/GhostNetwork.Publications

rm -r ./http
```
