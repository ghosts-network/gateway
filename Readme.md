# GhostNetwork - Gateway

Gateway is a part of GhostNetwork education project and provides public REST API

## Project overview

![Diagram](https://user-images.githubusercontent.com/9577482/119799258-949bee00-bee4-11eb-98d2-f457ec1af40f.png)


## Installation

copy provided dev-compose.yml and customize for your needs

### Parameters

| Environment          | Description                             |
|----------------------|---------------------------------------- |
| CONTENT_ADDRESS      | Address of publication service instance |
| PROFILES_ADDRESS     | Address of profiles service instance    |
| AUTHORITY            | Address of oauth2 authority server      |

## Development

To run dependent environment use

```bash
docker-compose -f dev-compose.yml pull
docker-compose -f dev-compose.yml up --force-recreate
```

