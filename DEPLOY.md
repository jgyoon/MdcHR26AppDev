# MdcHR26Apps 배포 가이드

## 📋 목차
1. [환경 구성](#환경-구성)
2. [Database 초기화](#database-초기화)
3. [Docker 배포](#docker-배포)
4. [환경 변수 설정](#환경-변수-설정)
5. [문제 해결](#문제-해결)

---

## 환경 구성

### 필수 요구사항
- Docker 20.10 이상
- Docker Compose 1.29 이상
- SQL Server Management Studio (SSMS) - Database 초기화용

---

## Database 초기화

### 1. SQL Server 컨테이너 시작

```bash
# docker-compose.yml의 YOUR_SA_PASSWORD를 실제 비밀번호로 변경
# 예: xnd0580+

docker-compose up -d mssql_server
```

### 2. Database 생성 및 테이블 생성

```bash
# 컨테이너 내부 접속
docker exec -it mdchr26_mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YOUR_SA_PASSWORD

# Database 생성
> CREATE DATABASE MdcHR2026;
> GO
> EXIT
```

### 3. 테이블 및 View 생성

**Option 1: SSMS 사용 (권장)**
1. SSMS에서 `localhost,1433` 접속 (sa / YOUR_SA_PASSWORD)
2. `Database/01_CreateTables.sql` 실행
3. `Database/02_CreateViews.sql` 실행

**Option 2: sqlcmd 사용**
```bash
docker cp Database/01_CreateTables.sql mdchr26_mssql:/tmp/
docker cp Database/02_CreateViews.sql mdchr26_mssql:/tmp/

docker exec -it mdchr26_mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YOUR_SA_PASSWORD -d MdcHR2026 -i /tmp/01_CreateTables.sql
docker exec -it mdchr26_mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YOUR_SA_PASSWORD -d MdcHR2026 -i /tmp/02_CreateViews.sql
```

---

## Docker 배포

### 1. 환경 변수 설정

**docker-compose.yml** 파일에서 다음 항목 수정:

```yaml
# MSSQL Server
- MSSQL_SA_PASSWORD=YOUR_SA_PASSWORD  # 실제 비밀번호로 변경

# Blazor App
- ConnectionStrings__MdcHR26AppsContainerConnection=Data Source=mssql_server;Database=MdcHR2026;User ID=sa;Password=YOUR_SA_PASSWORD;...
```

### 2. 전체 서비스 시작

```bash
# 이미지 빌드 및 컨테이너 시작
docker-compose up -d

# 로그 확인
docker-compose logs -f blazor_app
```

### 3. 접속 확인

브라우저에서 `http://localhost:8080` 접속

---

## 환경 변수 설정

### appsettings.json (개발 환경)

```json
{
  "AppSettings": {
    "IsProduction": 0  // LocalDB 사용
  }
}
```

### appsettings.Production.json (프로덕션 환경)

```json
{
  "AppSettings": {
    "IsProduction": 1  // Docker MSSQL 사용
  }
}
```

### 환경 변수 우선순위

```
Docker 환경 변수 > appsettings.Production.json > appsettings.json
```

---

## 환경별 연결 문자열

### 개발 환경 (IsProduction = 0)
```
Server=(localdb)\\MSSQLLocalDB;Database=MdcHR26Apps;Integrated Security=true;TrustServerCertificate=true;
```

### 프로덕션 환경 (IsProduction = 1)
```
Data Source=mssql_server;Database=MdcHR2026;User ID=sa;Password=xnd0580+;TrustServerCertificate=true;Connect Timeout=30;
```

---

## 문제 해결

### 1. Database 연결 실패

**증상**: "연결 문자열을 찾을 수 없습니다"

**해결**:
- `appsettings.Production.json`에 `MdcHR26AppsContainerConnection` 확인
- `docker-compose.yml`의 환경 변수 확인
- `AppSettings:IsProduction` 값 확인 (0 or 1)

### 2. MSSQL 컨테이너 시작 실패

**증상**: "The password does not meet SQL Server password policy requirements"

**해결**:
- 비밀번호를 8자 이상, 대소문자+숫자+특수문자 포함으로 변경

### 3. libman restore 실패

**증상**: "libman: command not found"

**해결**:
- Dockerfile이 최신 버전인지 확인 (libman CLI 설치 코드 포함)

### 4. 빌드 경고

**증상**: CS8601, CS8602 (Null 참조 경고)

**해결**:
- 정상 동작하므로 무시 가능 (약 60개)

### 5. 로그인 시 Antiforgery 오류

**증상**: "The antiforgery token could not be decrypted" 또는 로그인 실패

**원인**: Docker 컨테이너 재시작 시 DataProtection 키 초기화

**해결**:
1. **즉시 해결** (권장): 브라우저 캐시 삭제
   - F12 → Application → Storage → "Clear site data"
   - 브라우저 완전히 닫고 재접속
2. **영구 해결**: DataProtection 키를 Database에 저장
   ```csharp
   // Program.cs의 AddAntiforgery() 다음에 추가
   builder.Services.AddDataProtection()
       .PersistKeysToDbContext<MdcHR26AppsAddDbContext>();
   ```

---

## 배포 체크리스트

- [ ] Database 생성 완료 (MdcHR2026)
- [ ] 테이블 생성 완료 (12개)
- [ ] View 생성 완료 (8개)
- [ ] docker-compose.yml 비밀번호 변경
- [ ] appsettings.Production.json 비밀번호 변경
- [ ] Docker 이미지 빌드 성공
- [ ] 컨테이너 시작 성공
- [ ] 웹 접속 확인 (http://localhost:8080)
- [ ] 로그인 테스트
- [ ] Database 연결 확인

---

## 유용한 명령어

```bash
# 전체 서비스 시작
docker-compose up -d

# 전체 서비스 중지
docker-compose down

# 로그 확인
docker-compose logs -f

# 특정 서비스 재시작
docker-compose restart blazor_app

# 이미지 재빌드
docker-compose build --no-cache

# 볼륨 삭제 (데이터 초기화)
docker-compose down -v
```

---

**배포 담당**: 개발자
**작성일**: 2026-02-08
