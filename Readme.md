# GhostNetwork - Gateway

Gateway is a part of GhostNetwork education project and provides public REST API

## Project overview

![Diagram](https://user-images.githubusercontent.com/9577482/119670869-efc8d480-be41-11eb-9c77-01db2a79b560.png)


## Installation

copy provided dev-compose.yml and customize for your needs

### Parameters

| Environment          | Description                             |
|----------------------|---------------------------------------- |
| CONTENT_ADDRESS      | Address of publication service instance |
| PROFILES_ADDRESS     | Address of profiles service instance    |

## Development

To run dependent environment use

```bash
docker-compose -f dev-compose.yml pull
docker-compose -f dev-compose.yml up --force-recreate
```

