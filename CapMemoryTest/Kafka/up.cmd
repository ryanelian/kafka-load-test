docker network create dev
docker volume create mssql
docker volume create kafka
docker compose up -d --force-recreate
