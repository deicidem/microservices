@echo off

echo remove files
rmdir /s /q .\data
rmdir /s /q .\data-slave

docker-compose down

docker-compose up -d  postgres_master

echo start postgres_master...
timeout /t 120 /nobreak

docker exec -it postgres_master sh /etc/postgresql/init-script/init.sh
echo restart postgres_master...
docker-compose restart postgres_master
timeout /t 30 /nobreak

echo start postgres_slave...
docker-compose up -d  postgres_slave
timeout /t 30 /nobreak

echo start pgpool...
docker-compose up -d  pgpool
timeout /t 30 /nobreak

echo Done
