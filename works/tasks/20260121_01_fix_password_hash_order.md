# 작업지시서 (간소화): 비밀번호 해시 순서 수정

**날짜**: 2026-01-21
**파일**: `MdcHR26Apps.BlazorServer/Utils/PasswordHasher.cs`
**작업 유형**: 버그 수정
**관련 이슈**: [#010: 로그인 비밀번호 검증 실패 - 해시 순서 불일치](../issues/010_login_password_hash_order_mismatch.md)

---

## 1. 작업 개요

**목적**:
- SQL SeedData와 C# PasswordHasher 간 비밀번호 해시 생성 방식 일치시켜 로그인 인증 정상화

**배경**:
- 현재 SQL은 `Password + Salt` 순서로 해시 생성
- C# PasswordHasher는 `Salt + Password` 순서로 해시 생성
- 순서 불일치로 인해 같은 비밀번호라도 다른 해시 생성되어 로그인 실패

**근본 원인**:
```sql
-- SQL (Database/03_01_SeedData.sql:92)
HASHBYTES('SHA2_256', N'Password' + CAST(@Salt AS NVARCHAR(MAX)))
-- 순서: Password + Salt
```

```csharp
// C# (MdcHR26Apps.BlazorServer/Utils/PasswordHasher.cs:35-37)
Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
Buffer.BlockCopy(passwordBytes, 0, combined, salt.Length, passwordBytes.Length);
// 순서: Salt + Password
```

---

## 2. 수정 내용

### 2.1. 파일: `MdcHR26Apps.BlazorServer/Utils/PasswordHasher.cs`

**위치**: 라인 27-41 (HashPassword 메서드)

**변경 사항**:
```csharp
// ========== 기존 코드 (라인 34-37) ==========
// 비밀번호를 UTF-8 바이트로 변환
var passwordBytes = Encoding.UTF8.GetBytes(password);

// Salt + Password 결합 ← 순서 잘못됨
var combined = new byte[salt.Length + passwordBytes.Length];
Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
Buffer.BlockCopy(passwordBytes, 0, combined, salt.Length, passwordBytes.Length);

// ========== 수정 후 코드 ==========
// 비밀번호를 UTF-8 바이트로 변환
var passwordBytes = Encoding.UTF8.GetBytes(password);

// Password + Salt 결합 (SQL과 동일한 순서)
var combined = new byte[passwordBytes.Length + salt.Length];
Buffer.BlockCopy(passwordBytes, 0, combined, 0, passwordBytes.Length);
Buffer.BlockCopy(salt, 0, combined, passwordBytes.Length, salt.Length);
```

**설명**:
- Buffer.BlockCopy 순서를 변경하여 `Password + Salt` 순서로 결합
- combined 배열 크기는 동일 (순서만 변경)
- SQL HASHBYTES와 동일한 방식으로 해시 생성

**주석 수정**:
```csharp
// 기존: // Salt + Password 결합
// 수정: // Password + Salt 결합 (SQL과 동일한 순서)
```

---

## 3. 테스트 항목

개발자가 테스트할 항목:

### Test 1: jgyoon 계정 로그인
1. 브라우저에서 `/auth/login` 페이지 접속
2. 다음 정보 입력:
   - ID: `jgyoon`
   - PW: `xnd0580+`
3. 로그인 버튼 클릭
4. **확인**:
   - 로그인 성공
   - 홈 페이지("/")로 리다이렉트
   - 상단에 "윤종국" 사용자명 표시
   - 로그인 상태 정상 유지

### Test 2: 관리자 계정 로그인
1. `/auth/login` 페이지 접속
2. 다음 정보 입력:
   - ID: `mdcadmin`
   - PW: `xnd0580!!`
3. **확인**: 로그인 성공 및 관리자 권한 확인

### Test 3: 잘못된 비밀번호 입력
1. `/auth/login` 페이지 접속
2. ID: `jgyoon`, PW: `wrongpassword` 입력
3. **확인**: "사용자 ID 또는 비밀번호가 일치하지 않습니다." 에러 메시지 표시

### Test 4: 다른 사용자 계정 테스트
1. 다음 계정들로 순차 테스트:
   - ID: `jslim`, PW: `xnd852456+`
   - ID: `jeonghwa.seo`, PW: `xnd852456+`
2. **확인**: 모든 계정 로그인 성공

### Test 5: 로그아웃 후 재로그인
1. 로그인 후 로그아웃
2. 다시 로그인
3. **확인**: 정상 작동

---

## 4. 영향 분석

### 4.1. 영향 받는 기능
- 로그인 인증 (Login.razor)
- 비밀번호 검증 로직 (PasswordHasher.VerifyPassword)

### 4.2. 영향 받지 않는 부분
- 데이터베이스 (수정 불필요)
- 기존 시드 데이터 (그대로 사용)
- 다른 Repository 클래스들

### 4.3. 주의사항
- PasswordHasher.HashPassword 메서드는 VerifyPassword에서만 사용되므로 안전
- 신규 사용자 등록 시에도 이 메서드 사용 예정이므로 SQL과 일치해야 함

---

## 5. 완료 조건

- [ ] PasswordHasher.cs 코드 수정 완료
- [ ] Test 1: jgyoon 계정 로그인 성공
- [ ] Test 2: mdcadmin 계정 로그인 성공
- [ ] Test 3: 잘못된 비밀번호 검증 정상 작동
- [ ] Test 4: 다른 사용자 계정 로그인 성공
- [ ] Test 5: 로그아웃/재로그인 정상 작동

---

## 6. 참고 자료

### 6.1. 관련 파일
- `MdcHR26Apps.BlazorServer/Utils/PasswordHasher.cs` (수정 대상)
- `Database/03_01_SeedData.sql` (참조, 라인 92-99)
- `MdcHR26Apps.BlazorServer/Components/Pages/Auth/Login.razor` (사용처)

### 6.2. 테스트 계정 목록
```sql
-- Uid: 1, 관리자
ID: mdcadmin, PW: xnd0580!!

-- Uid: 2, 팀장+임원
ID: jslim, PW: xnd852456+

-- Uid: 3, 관리자
ID: jgyoon, PW: xnd0580+

-- Uid: 5, 팀장+관리자+부서목표작성자
ID: jeonghwa.seo, PW: xnd852456+
```

---

**작성자**: Claude AI
**검토자**: 개발자
**우선순위**: 높음 (로그인 기능 차단 중)
