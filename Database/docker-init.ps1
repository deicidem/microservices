# Очистка данных
Remove-Item -Recurse -Force ..\postgres\data\*
Remove-Item -Recurse -Force ..\postgres\data-slave\*
docker-compose down

# Запуск контейнера postgres_master
docker-compose up -d postgres_master
Write-Host "Запуск узла postgres_master..."
Start-Sleep -Seconds 120  # Ожидание завершения запуска мастер-узла

# Подготовка конфигурации реплики
docker exec -it postgres_master sh /etc/postgresql/init-script/init.sh
Write-Host "Перезапуск мастер-узла"
docker-compose restart postgres_master
Start-Sleep -Seconds 30

# Запуск узла postgres_slave
docker-compose up -d postgres_slave
Start-Sleep -Seconds 30  # Ожидание запуска узла

# Запуск узла pgpool
docker-compose up -d pgpool
Start-Sleep -Seconds 30  # Ожидание запуска узла

Write-Host "Готово"