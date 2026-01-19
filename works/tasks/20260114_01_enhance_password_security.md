# 작업지시서: UserDb 비밀번호 보안 강화

**날짜**: 2026-01-14
**작업 유형**: 보안 강화 - 비밀번호 처리 개선
**우선순위**: 높음
**예상 작업 범위**: 중간 (DB 스키마 수정 + Seed Data 수정)
**관련 이슈**: [#006: UserDb 비밀번호 보안 강화](../issues/006_enhance_password_security.md)

---

## 1. 작업 배경

### 현재 문제점
현재 [UserDb.sql](../../Database/dbo/UserDb.sql)의 비밀번호 처리 방식:
```sql
[UserPassword] VARBINARY(100) NOT NULL
```
- Salt 값 없이 단순 해시 방식
- Rainbow Table 공격에 취약
- 보안 수준이 낮음

### 참고 프로젝트의 개선된 방식
참고 프로젝트(`01_MdcLibrary`)의 UserDb.sql 구조:
```sql
-- SHA-256 해시 (32바이트)
[UserPassword] VARBINARY(32) NOT NULL,

-- Salt 값 (16바이트 랜덤)
[UserPasswordSalt] VARBINARY(16) NOT NULL,
```

**장점**:
1. **Salt 사용**: 동일한 비밀번호도 사용자마다 다른 해시값 생성
2. **Rainbow Table 공격 방지**: Salt로 인해 사전 계산된 해시 테이블 무력화
3. **SHA-256 알고리즘**: 안전한 일방향 암호화
4. **크기 최적화**: 정확한 바이트 크기 지정 (32바이트)

---

## 2. 작업 내용

### 2-1. UserDb.sql 수정

**파일**: [Database/dbo/UserDb.sql](../../Database/dbo/UserDb.sql)

**변경 전**:
```sql
-- [04] 비밀번호
-- 암호화를 위해서 VARBINARY로
[UserPassword] VARBINARY(100) NOT NULL,
```

**변경 후**:
```sql
-- [04] 비밀번호
-- 암호화를 위해서 VARBINARY로
-- SHA2_256 알고리즘을 활용한 일방향 암호화
-- SHA-256 해시값의 크기는 32바이트
[UserPassword] VARBINARY(32) NOT NULL,

-- [05] Salt
-- 랜덤데이터로 입력
-- CAST(NEWID() AS VARBINARY(128))
-- 16바이트 길이의 salt
[UserPasswordSalt] VARBINARY(16) NOT NULL,
```

**필드 번호 재정렬**:
- `[04]` UserPassword
- `[05]` UserPasswordSalt (신규)
- `[06]` ENumber (기존 [05])
- `[07]` Email (기존 [06])
- `[08]` EDepartId (기존 [07])
- `[09]` ERankId (기존 [08])
- `[10]` EStatus (기존 [09])
- `[11]` IsTeamLeader (기존 [10])
- `[12]` IsDirector (기존 [11])
- `[13]` IsAdministrator (기존 [12])

---

### 2-2. 01_CreateTables.sql 수정

**파일**: [Database/01_CreateTables.sql](../../Database/01_CreateTables.sql)

**수정 위치**: UserDb 생성 부분 (Line 83-125)

동일하게 UserPasswordSalt 필드 추가

---

### 2-3. 03_SeedData.sql 수정

**파일**: [Database/03_SeedData.sql](../../Database/03_SeedData.sql)

**변경 전** (Line 38-45):
```sql
INSERT INTO UserDb (UserId, UserName, UserPassword, EDepartId, ERankId, IsAdministrator)
VALUES (
    'admin',                                      -- UserId
    N'관리자',                                    -- UserName
    HASHBYTES('SHA2_256', 'admin1234'),          -- UserPassword
    1,                                            -- EDepartId (경영지원본부)
    9,                                            -- ERankId (사장)
    1                                             -- IsAdministrator
);
```

**변경 후**:
```sql
-- Salt 생성 및 저장
DECLARE @AdminSalt VARBINARY(16) = CAST(NEWID() AS VARBINARY(16));

-- 관리자 계정: admin / admin1234
INSERT INTO UserDb (UserId, UserName, UserPassword, UserPasswordSalt, EDepartId, ERankId, IsAdministrator)
VALUES (
    'admin',                                                  -- UserId
    N'관리자',                                                -- UserName
    HASHBYTES('SHA2_256', 'admin1234' + CAST(@AdminSalt AS VARCHAR(MAX))), -- UserPassword (Salt 적용)
    @AdminSalt,                                              -- UserPasswordSalt
    1,                                                        -- EDepartId (경영지원본부)
    9,                                                        -- ERankId (사장)
    1                                                         -- IsAdministrator
);
```

---

## 3. 비밀번호 검증 방법

### 로그인 시 비밀번호 검증 로직 (참고용)

```sql
-- 사용자가 입력한 비밀번호와 저장된 해시 비교
DECLARE @InputPassword VARCHAR(50) = 'admin1234';
DECLARE @UserId VARCHAR(50) = 'admin';

SELECT
    UId,
    UserId,
    UserName,
    CASE
        WHEN UserPassword = HASHBYTES('SHA2_256', @InputPassword + CAST(UserPasswordSalt AS VARCHAR(MAX)))
        THEN 'Success'
        ELSE 'Failed'
    END AS LoginResult
FROM UserDb
WHERE UserId = @UserId;
```

**동작 원리**:
1. 사용자 입력 비밀번호 + 저장된 Salt 값 결합
2. SHA-256 해시 생성
3. DB에 저장된 해시값과 비교

---

## 4. 테스트 계획

### 4-1. 스키마 수정 테스트
```sql
-- 1. 기존 테이블 삭제
DROP TABLE IF EXISTS UserDb;

-- 2. 새 구조로 테이블 생성
-- 01_CreateTables.sql 실행

-- 3. 테이블 구조 확인
EXEC sp_help 'UserDb';

-- 4. 컬럼 확인
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'UserDb'
ORDER BY ORDINAL_POSITION;
```

### 4-2. 시드 데이터 입력 테스트
```sql
-- 1. 시드 데이터 입력
-- 03_SeedData.sql 실행

-- 2. 데이터 확인
SELECT
    UId,
    UserId,
    UserName,
    LEN(UserPassword) AS PasswordLength,  -- 32바이트 확인
    LEN(UserPasswordSalt) AS SaltLength,  -- 16바이트 확인
    IsAdministrator
FROM UserDb;
```

### 4-3. 비밀번호 검증 테스트
```sql
-- 올바른 비밀번호 테스트
DECLARE @InputPassword VARCHAR(50) = 'admin1234';
DECLARE @UserId VARCHAR(50) = 'admin';

SELECT
    UId,
    UserId,
    UserName,
    CASE
        WHEN UserPassword = HASHBYTES('SHA2_256', @InputPassword + CAST(UserPasswordSalt AS VARCHAR(MAX)))
        THEN '✅ 로그인 성공'
        ELSE '❌ 로그인 실패'
    END AS LoginResult
FROM UserDb
WHERE UserId = @UserId;

-- 잘못된 비밀번호 테스트
SET @InputPassword = 'wrong_password';

SELECT
    UId,
    UserId,
    UserName,
    CASE
        WHEN UserPassword = HASHBYTES('SHA2_256', @InputPassword + CAST(UserPasswordSalt AS VARCHAR(MAX)))
        THEN '✅ 로그인 성공'
        ELSE '❌ 로그인 실패'
    END AS LoginResult
FROM UserDb
WHERE UserId = @UserId;
```

**예상 결과**:
- 'admin1234' 입력 시: ✅ 로그인 성공
- 'wrong_password' 입력 시: ❌ 로그인 실패

---

## 5. 영향도 분석

### 5-1. 수정이 필요한 파일
1. ✅ **Database/dbo/UserDb.sql** - 테이블 스키마 수정
2. ✅ **Database/01_CreateTables.sql** - 통합 생성 스크립트 수정
3. ✅ **Database/03_SeedData.sql** - 시드 데이터 생성 로직 수정

### 5-2. 영향받지 않는 부분
- ✅ **외래키 관계**: UId는 변경 없음
- ✅ **뷰(View)**: UserPassword를 조회하지 않으므로 영향 없음
- ✅ **Repository**: 아직 작성 전이므로 영향 없음

### 5-3. 향후 개발 시 주의사항
**Phase 2 (Model/Repository 작성 시)**:
- User 모델에 `UserPasswordSalt` 속성 추가
- UserRepository의 비밀번호 검증 로직에 Salt 처리 포함
- 회원가입 시 Salt 자동 생성 로직 구현

**예시 코드 (C#)**:
```csharp
// User 모델
public class User
{
    public long UId { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public byte[] UserPassword { get; set; }
    public byte[] UserPasswordSalt { get; set; }  // 신규 추가
    // ... 나머지 속성
}

// 비밀번호 해시 생성 메서드 (회원가입 시)
public static (byte[] hash, byte[] salt) HashPassword(string password)
{
    // Salt 생성
    byte[] salt = Guid.NewGuid().ToByteArray();

    // 비밀번호 + Salt 해시 생성
    using (var sha256 = SHA256.Create())
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] saltedPassword = passwordBytes.Concat(salt).ToArray();
        byte[] hash = sha256.ComputeHash(saltedPassword);

        return (hash, salt);
    }
}

// 비밀번호 검증 메서드 (로그인 시)
public static bool VerifyPassword(string inputPassword, byte[] storedHash, byte[] storedSalt)
{
    using (var sha256 = SHA256.Create())
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(inputPassword);
        byte[] saltedPassword = passwordBytes.Concat(storedSalt).ToArray();
        byte[] hashToCompare = sha256.ComputeHash(saltedPassword);

        return hashToCompare.SequenceEqual(storedHash);
    }
}
```

---

## 6. 체크리스트

### 작업 전 확인사항
- [ ] 현재 UserDb 구조 백업
- [ ] Phase 1 작업 완료 확인
- [ ] 개발자 승인 확인

### 작업 수행
- [ ] UserDb.sql 수정
- [ ] 01_CreateTables.sql 수정
- [ ] 03_SeedData.sql 수정

### 테스트
- [ ] 테이블 구조 검증 (컬럼 타입, 크기 확인)
- [ ] 시드 데이터 입력 검증
- [ ] 비밀번호 해시 길이 확인 (32바이트)
- [ ] Salt 길이 확인 (16바이트)
- [ ] 올바른 비밀번호 검증 성공
- [ ] 잘못된 비밀번호 검증 실패

### 문서화
- [ ] 변경 내역 기록
- [ ] Phase 1 진행 요약 업데이트 (필요 시)

---

## 7. 참고 자료

### 참고 프로젝트
- **프로젝트**: 01_MdcLibrary
- **파일**: [C:\Codes\00_Develop_Cursor\01_MdcLibrary\Database\dbo\UserDb.sql](C:\Codes\00_Develop_Cursor\01_MdcLibrary\Database\dbo\UserDb.sql)

### 보안 개선 포인트
1. **Salt 사용**: 동일 비밀번호도 다른 해시값 생성
2. **Rainbow Table 방어**: 사전 계산된 해시 테이블 공격 차단
3. **크기 최적화**: SHA-256(32바이트), Salt(16바이트) 정확한 크기 지정
4. **일방향 암호화**: 복호화 불가능한 안전한 저장

### SHA-256 해시 특성
- 입력값이 1비트만 달라도 완전히 다른 해시값 생성
- 고정 길이 출력: 항상 256비트(32바이트)
- 충돌 저항성: 동일한 해시값을 생성하는 두 입력을 찾기 극도로 어려움

---

## 8. 개발자 확인 사항

### 승인 필요 사항
1. **스키마 변경 승인**: UserPasswordSalt 필드 추가
2. **필드 번호 재정렬 승인**: 기존 [05]~[12] → [06]~[13]
3. **Seed Data 로직 변경 승인**: Salt 기반 해시 생성

### 질문 사항
1. 기존 데이터 마이그레이션이 필요한가요? (현재는 테스트 단계이므로 불필요로 판단)
2. 비밀번호 정책(최소 길이, 복잡도 등)을 추가로 설정할까요?
3. 비밀번호 변경 이력 관리가 필요한가요?

---

**작성일**: 2026-01-14
**작업 예상 시간**: 30분 (수정 + 테스트)
**우선순위**: 높음 (보안 강화는 초기에 적용할수록 좋음)
**다음 단계**: 개발자 승인 후 작업 진행 → Phase 2 진행
