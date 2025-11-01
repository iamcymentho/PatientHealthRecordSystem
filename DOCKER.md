# Docker Deployment Guide

This guide explains how to build and run the Patient Health Record System using Docker.

## Prerequisites

- Docker Desktop installed
- Docker Compose installed (included with Docker Desktop)

## Quick Start

### 1. Environment Setup

Copy the example environment file and configure it:

```bash
cp .env.example .env
```

Edit `.env` and update the following critical settings:
- `JWT_SECRET_KEY`: Use a strong, random key (minimum 32 characters)
- SMTP settings if you need email functionality

### 2. Build and Run

Build and start the application:

```bash
docker-compose up -d
```

The API will be available at: **http://localhost:5000**

### 3. Access Swagger Documentation

Open your browser and navigate to:
- **Swagger UI**: http://localhost:5000/swagger

### 4. Stop the Application

```bash
docker-compose down
```

To stop and remove volumes (deletes database):

```bash
docker-compose down -v
```

## Docker Commands

### Build the Docker Image

```bash
docker build -t phr-api .
```

### Run the Container

```bash
docker run -d -p 5000:8080 --name phr-api phr-api
```

### View Logs

```bash
docker-compose logs -f
```

Or for a specific container:

```bash
docker logs phr-api -f
```

### Access Container Shell

```bash
docker exec -it phr-api /bin/bash
```

### Rebuild After Code Changes

```bash
docker-compose up -d --build
```

## Database

The application uses SQLite by default. The database file is stored in a Docker volume named `phr-data`, which persists data between container restarts.

### Backup Database

```bash
docker cp phr-api:/app/data/phr_prod.db ./backup-$(date +%Y%m%d).db
```

### Restore Database

```bash
docker cp ./backup.db phr-api:/app/data/phr_prod.db
docker-compose restart
```

## Production Deployment

### Security Checklist

1. **Change JWT Secret Key**
   - Generate a strong random key (32+ characters)
   - Update in `.env` or use environment variables

2. **Use HTTPS**
   - Configure a reverse proxy (nginx, Traefik, etc.)
   - Obtain SSL certificates (Let's Encrypt)

3. **Update CORS Settings**
   - Configure allowed origins in `appsettings.Production.json`


### Example: Using with Nginx Reverse Proxy

docker-compose.yml with nginx:

```yaml
version: '3.8'

services:
  phr-api:
    build: .
    expose:
      - "8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    volumes:
      - phr-data:/app/data
    networks:
      - phr-network

  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf
      - ./ssl:/etc/nginx/ssl
    depends_on:
      - phr-api
    networks:
      - phr-network

volumes:
  phr-data:

networks:
  phr-network:
```

## Troubleshooting

### Container won't start

Check logs:
```bash
docker-compose logs phr-api
```

### Database permission issues

Ensure the data volume has correct permissions:
```bash
docker-compose down
docker volume rm phr-data
docker-compose up -d
```

### Port already in use

Change the port mapping in `docker-compose.yml`:
```yaml
ports:
  - "5001:8080"  # Use port 5001 instead
```

## Health Check

The application includes a health check endpoint:

```bash
curl http://localhost:5000/api/Health
```

Expected response:
```json
{
  "status": "Healthy"
}
```

## Environment Variables

All available environment variables:

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment name | `Production` |
| `ASPNETCORE_URLS` | URLs to bind | `http://+:8080` |
| `JWT_SECRET_KEY` | JWT signing key | *(required)* |
| `JWT_ISSUER` | JWT issuer | `PHR-API` |
| `JWT_AUDIENCE` | JWT audience | `PHR-Client` |
| `JWT_EXPIRATION_MINUTES` | Token expiration | `60` |
| `ConnectionStrings__DefaultConnection` | Database path | `/app/data/phr_prod.db` |

## Upgrading

To upgrade to a new version:

1. Pull latest code
2. Rebuild and restart:

```bash
git pull
docker-compose down
docker-compose up -d --build
```

## Support

For issues or questions, please check the main README.md or create an issue in the repository.
