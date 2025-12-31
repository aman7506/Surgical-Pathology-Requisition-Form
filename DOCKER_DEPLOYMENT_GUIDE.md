# Docker Deployment Guide
## Surgical Pathology Requisition Form - Container Deployment

**Purpose**: Deploy the Surgical Pathology Requisition Form using Docker containers for consistent, portable, and scalable deployment.

---

## Quick Start (5 Minutes)

### Prerequisites
- Docker Desktop installed (Windows/Mac) or Docker Engine (Linux)
- Docker Compose installed
- 4GB RAM minimum, 8GB recommended

### Deploy Everything with One Command

```bash
# Clone repository
git clone https://github.com/aman7506/Surgical-Pathology-Requisition-Form.git
cd Surgical-Pathology-Requisition-Form

# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Access application
# Open browser: http://localhost
```

**Done!** Application is running with SQL Server database.

---

## What Gets Deployed

The `docker-compose.yml` file deploys:

1. **SQL Server 2022** (Database)
   - Port: 1433
   - Default password: `YourStrong@Password123` (change in production)
   - Persistent data storage

2. **Pathology Application** (ASP.NET Core)
   - Port: 80 (HTTP)
   - Port: 443 (HTTPS)
   - Automatic database connection
   - Health checks enabled

3. **Nginx Reverse Proxy** (Optional - Production profile)
   - Port: 8080 (HTTP)
   - Port: 8443 (HTTPS)
   - SSL termination
   - Load balancing ready

---

## Detailed Deployment Steps

### Step 1: Configure Environment Variables

Create `.env` file in project root:

```env
# Database Configuration
SA_PASSWORD=YourStrong@Password123!
MSSQL_PID=Developer

# Application Configuration
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80

# Database Connection
DB_SERVER=sqlserver
DB_NAME=pathology_db
DB_USER=sa
DB_PASSWORD=YourStrong@Password123!

# Security (change these!)
JWT_SECRET=your-super-secret-jwt-key-change-this
ENCRYPTION_KEY=your-encryption-key-change-this
```

Update `docker-compose.yml` to use environment file:

```yaml
env_file:
  - .env
```

### Step 2: Build Custom Image

```bash
# Build application image
docker build -t pathologyapp:1.0.0 -f Dockerfile .

# Tag for local registry
docker tag pathologyapp:1.0.0 pathologyapp:latest

# Verify image created
docker images | grep pathologyapp
```

### Step 3: Start Services

```bash
# Start in detached mode
docker-compose up -d

# Check service status
docker-compose ps

# View service logs
docker-compose logs pathologyapp
docker-compose logs sqlserver
```

### Step 4: Initialize Database

```bash
# Execute SQL scripts
docker exec -it pathology-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong@Password123!' \
  -i /docker-entrypoint-initdb.d/01_CreateDatabase.sql

docker exec -it pathology-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong@Password123!' \
  -i /docker-entrypoint-initdb.d/02_CreateUsersAndRoles.sql
```

Or run migrations from application:

```bash
docker exec -it pathology-app dotnet ef database update
```

### Step 5: Verify Deployment

```bash
# Check application health
curl http://localhost/health

# Check logs for errors
docker-compose logs --tail=50 pathologyapp

# Access application
# Browser: http://localhost
```

---

## Production Deployment

### With Nginx Reverse Proxy

```bash
# Start with production profile
docker-compose --profile production up -d

# This starts:
# - SQL Server
# - Application
# - Nginx reverse proxy
```

### SSL Certificate Configuration

1. **Obtain SSL Certificate**:
   - From Let's Encrypt, or
   - From your organization's CA

2. **Place certificates**:
```bash
mkdir -p ssl
# Copy certificate files
cp your-cert.crt ssl/
cp your-cert.key ssl/
```

3. **Update nginx.conf**:
```nginx
server {
    listen 443 ssl;
    ssl_certificate /etc/nginx/ssl/your-cert.crt;
    ssl_certificate_key /etc/nginx/ssl/your-cert.key;
    
    location / {
        proxy_pass http://pathologyapp;
    }
}
```

4. **Restart Nginx**:
```bash
docker-compose restart nginx
```

---

## Scaling and High Availability

### Scale Application Instances

```bash
# Scale to 3 application instances
docker-compose up -d --scale pathologyapp=3

# Verify all instances running
docker-compose ps
```

### Load Balancing with Nginx

Update `nginx.conf`:

```nginx
upstream pathology_backend {
    least_conn;
    server pathologyapp_1:80;
    server pathologyapp_2:80;
    server pathologyapp_3:80;
}

server {
    listen 80;
    location / {
        proxy_pass http://pathology_backend;
    }
}
```

---

## Data Persistence and Backup

### Backup Database

```bash
# Backup SQL Server database
docker exec pathology-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong@Password123!' \
  -Q "BACKUP DATABASE pathology_db TO DISK='/var/opt/mssql/backup/pathology_db.bak'"

# Copy backup to host
docker cp pathology-db:/var/opt/mssql/backup/pathology_db.bak ./backups/
```

### Backup Uploaded Files

```bash
# Backup uploads volume
docker run --rm \
  -v pathology-form_app-uploads:/source \
  -v $(pwd)/backups:/backup \
  alpine tar czf /backup/uploads-$(date +%Y%m%d).tar.gz -C /source .
```

### Restore Database

```bash
# Copy backup to container
docker cp ./backups/pathology_db.bak pathology-db:/var/opt/mssql/backup/

# Restore database
docker exec pathology-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong@Password123!' \
  -Q "RESTORE DATABASE pathology_db FROM DISK='/var/opt/mssql/backup/pathology_db.bak' WITH REPLACE"
```

---

## Monitoring and Logs

### View Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f pathologyapp

# Last 100 lines
docker-compose logs --tail=100 pathologyapp

# Since specific time
docker-compose logs --since 2023-01-01T00:00:00 pathologyapp
```

### Application Metrics

```bash
# Container stats
docker stats pathology-app pathology-db

# Detailed inspection
docker inspect pathology-app

# Health check status
docker inspect --format='{{.State.Health.Status}}' pathology-app
```

### Resource Limits

Add to `docker-compose.yml`:

```yaml
services:
  pathologyapp:
    deploy:
      resources:
        limits:
          cpus: '2.0'
          memory: 4G
        reservations:
          cpus: '1.0'
          memory: 2G
```

---

## Troubleshooting

### Application Won't Start

```bash
# Check logs
docker-compose logs pathologyapp

# Common issues:
# 1. Database not ready - wait for health check
docker-compose logs sqlserver

# 2. Connection string wrong - verify environment variables
docker-compose config

# 3. Port conflict - check if ports are available
netstat -ano | findstr :80
netstat -ano | findstr :1433
```

### Database Connection Failed

```bash
# Test database connectivity
docker exec pathology-app ping sqlserver

# Test SQL connection
docker exec pathology-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong@Password123!' \
  -Q "SELECT @@VERSION"

# Verify connection string
docker exec pathology-app env | grep ConnectionStrings
```

### High Memory Usage

```bash
# Check memory usage
docker stats --no-stream

# Restart services
docker-compose restart

# Clear unused images and containers
docker system prune -a
```

---

## Updates and Maintenance

### Update Application

```bash
# Pull latest code
git pull origin main

# Rebuild image
docker-compose build pathologyapp

# Restart with new image
docker-compose up -d pathologyapp

# Verify new version
docker-compose logs pathologyapp | head -20
```

### Update Database Schema

```bash
# Run migrations
docker exec pathology-app dotnet ef database update

# Or execute SQL script
docker exec -i pathology-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong@Password123!' \
  < migration-script.sql
```

---

## Security Hardening

### Production Checklist

- [ ] Change default SA password
- [ ] Use secrets management (Docker secrets or external vault)
- [ ] Enable HTTPS only
- [ ] Use non-root user in containers (already configured)
- [ ] Enable Docker content trust
- [ ] Scan images for vulnerabilities
- [ ] Implement network segmentation
- [ ] Enable audit logging
- [ ] Configure firewall rules
- [ ] Regular security updates

### Using Docker Secrets

```yaml
services:
  pathologyapp:
    secrets:
      - db_password
      - jwt_secret
    environment:
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=pathology_db;User Id=sa;Password_File=/run/secrets/db_password

secrets:
  db_password:
    file: ./secrets/db_password.txt
  jwt_secret:
    file: ./secrets/jwt_secret.txt
```

---

## Kubernetes Deployment (Advanced)

For enterprise-scale deployment:

```bash
# Generate Kubernetes manifests from docker-compose
kompose convert -f docker-compose.yml

# Apply to Kubernetes cluster
kubectl apply -f .

# Or use Helm chart (create custom chart)
helm create pathology-app
```

---

## Summary

Docker deployment provides:
- ✅ Consistent environment across development and production
- ✅ Easy scaling and load balancing
- ✅ Simple backup and restore
- ✅ Fast deployment and updates
- ✅ Resource management and monitoring
- ✅ Production-ready with minimal configuration

**One command to deploy**: `docker-compose up -d`

**One command to update**: `docker-compose up -d --build`

**One command to backup**: Automated with cron or Windows Task Scheduler

---

*Docker Deployment Guide - Healthcare Application Container Deployment*  
*Last Updated: December 2025*
