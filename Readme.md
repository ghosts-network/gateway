# GhostNetwork - Gateway

Gateway is a part of GhostNetwork education project and provides public REST API

## Installation

copy provided dev-compose.yml and customize for your needs

### Parameters

| Environment          | Description                             |
|----------------------|---------------------------------------- |
| PUBLICATIONS_ADDRESS | Address of publication service instance |
| REACTIONS_ADDRESS    | Address of reactions service instance   |
| PROFILES_ADDRESS     | Address of profiles service instance    |

## Development

To run dependent environment use

```bash
docker-compose -f dev-compose.yml pull
docker-compose -f dev-compose.yml up --force-recreate
```

