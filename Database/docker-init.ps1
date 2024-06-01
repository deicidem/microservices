# ������� ������
Remove-Item -Recurse -Force ..\postgres\data\*
Remove-Item -Recurse -Force ..\postgres\data-slave\*
docker-compose down

# ������ ���������� postgres_master
docker-compose up -d postgres_master
Write-Host "������ ���� postgres_master..."
Start-Sleep -Seconds 120  # �������� ���������� ������� ������-����

# ���������� ������������ �������
docker exec -it postgres_master sh /etc/postgresql/init-script/init.sh
Write-Host "���������� ������-����"
docker-compose restart postgres_master
Start-Sleep -Seconds 30

# ������ ���� postgres_slave
docker-compose up -d postgres_slave
Start-Sleep -Seconds 30  # �������� ������� ����

# ������ ���� pgpool
docker-compose up -d pgpool
Start-Sleep -Seconds 30  # �������� ������� ����

Write-Host "������"