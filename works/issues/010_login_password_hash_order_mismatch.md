# 이슈 #010: 로그인 비밀번호 검증 실패 - 해시 순서 불일치

**날짜**: 2026-01-21
**상태**: 완료
**우선순위**: 높음
**관련 이슈**: [#009](009_phase3_webapp_development.md)

---

## 개발자 요청

**배경**:
- Phase 3-2 로그인 페이지 구현 후 실제 테스트 중 로그인 실패 발생
- 사용자: jgyoon, 비밀번호 입력했으나 인증 실패
- 비밀번호 값이 제대로 넘어오지 않는 것으로 의심됨

**요청 사항**:
1. 로그인 인증 로직 검토
2. 비밀번호 해싱 방식 확인
3. SQL SeedData와 C# 코드 간 일관성 검증

---

## 문제 분석

### 근본 원인: 해시 순서 불일치

#### 1. SQL SeedData (Database/03_01_SeedData.sql:92-99)
```sql
HASHBYTES('SHA2_256', N'Password' + CAST(@Salt AS NVARCHAR(MAX)))
```
- **순서**: Password + Salt
- 비밀번호를 먼저, Salt를 뒤에 결합

#### 2. C# PasswordHasher (MdcHR26Apps.BlazorServer/Utils/PasswordHasher.cs:35-37)
```csharp
var combined = new byte[salt.Length + passwordBytes.Length];
Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
Buffer.BlockCopy(passwordBytes, 0, combined, salt.Length, passwordBytes.Length);
```
- **순서**: Salt + Password
- Salt를 먼저, 비밀번호를 뒤에 결합

### 문제점
- 같은 비밀번호와 Salt를 사용하더라도 순서가 다르면 완전히 다른 해시가 생성됨
- 결과적으로 DB에 저장된 해시와 로그인 시 생성된 해시가 불일치
- 로그인 실패

---

## 해결 방안

### 방안 1: C# PasswordHasher 수정 (권장)
**장점**:
- SQL 데이터베이스 재생성 불필요
- 기존 시드 데이터 유지
- 수정 범위 최소화 (1개 파일)

**수정 내용**:
- PasswordHasher.cs의 HashPassword 메서드 수정
- 순서를 `Salt + Password` → `Password + Salt`로 변경

### 방안 2: SQL SeedData 수정
**단점**:
- 데이터베이스 전체 재생성 필요
- 테스트 데이터 재입력
- 수정 범위 큰 편

---

## 진행 사항

- [x] 문제 원인 분석 완료
- [x] 작업지시서 작성 (20260121_01_fix_password_hash_order.md)
- [x] 개발자 승인
- [x] 코드 수정 완료 (PasswordHasher.cs)
- [x] 추가 문제 발견: Login.razor 로직 문제
- [x] Login.razor 수정 (UserRepository.LoginCheckAsync 사용)
- [x] 인코딩 불일치 발견 및 수정 (UTF-8 → Unicode)
- [x] 테스트 (jgyoon 계정으로 로그인) - 성공
- [x] 로그인 DB 쿼리 최적화 (v_MemberListDB 활용)
- [x] forceLoad 사용 정리 (Login 제거, Logout 유지)
- [x] Playwright 테스트 환경 구축
- [x] 프로젝트 동기화 관리 시스템 구축

---

## 기술적 세부사항

### 영향 받는 파일
1. `MdcHR26Apps.BlazorServer/Utils/PasswordHasher.cs` (수정 완료)
   - HashPassword 메서드 (라인 27-41)
   - 순서 변경: Salt + Password → Password + Salt
   - **인코딩 변경**: UTF-8 → Unicode (SQL NVARCHAR와 일치)

2. `MdcHR26Apps.BlazorServer/Components/Pages/Auth/Login.razor` (수정 완료)
   - HandleLogin 메서드 (라인 92-163)
   - **핵심 변경**: Utils.PasswordHasher 사용 → UserRepository.LoginCheckAsync 사용
   - LoginCheckAsync가 DB에서 저장된 Salt를 가져와서 정확히 비교

3. `MdcHR26Apps.Models/User/UserRepository.cs` (수정 완료)
   - LoginCheckAsync 메서드 (라인 237-264)
   - 순서: Password + Salt
   - **인코딩 변경**: UTF-8 → Unicode (SQL NVARCHAR와 일치)
   - **비교 방식**: StructuralComparisons.StructuralEqualityComparer 사용

4. `Database/03_01_SeedData.sql` (참조)
   - UserDb INSERT 구문 (라인 92-99)
   - SQL에서 NVARCHAR(Unicode) 사용

### 테스트 계정
```
ID: jgyoon
PW: xnd0580+
```

---

## 테스트 결과

### Test 1: 해시 순서 수정 후 (실패)
**결과**: 실패
**사유**: 인코딩 불일치 (C#: UTF-8, SQL: Unicode)

### Test 2: 인코딩 수정 후 (성공)
**결과**: 성공 ✅
**사유**:
- UserRepository.LoginCheckAsync 사용
- Encoding.Unicode로 변경 (SQL NVARCHAR와 일치)
- Password + Salt 순서 일치
- jgyoon 계정 로그인 성공 확인

---

## 관련 문서

**작업지시서**:
- [20260121_01_fix_password_hash_order.md](../tasks/20260121_01_fix_password_hash_order.md)

**관련 이슈**:
- [#009: Phase 3 Blazor Server WebApp 개발](009_phase3_webapp_development.md) - Phase 3-2 로그인 기능 구현 중 발견

---

## 추가 최적화 작업 (2026-01-22)

### 1. DB 쿼리 최적화
**문제**: Login.razor에서 3개의 쿼리 실행
- UserRepository.LoginCheckAsync (로그인 검증)
- IUserRepository.GetUserByIdAsync (사용자 정보)
- IDepartmentRepository.GetDepartmentByIdAsync (부서 정보)

**해결**:
- v_MemberListDB 뷰 활용
- Iv_MemberListRepository.GetByUserIdAsync 사용
- 쿼리 3개 → 2개로 감소

**수정 파일**:
- `MdcHR26Apps.Models/Views/v_MemberListDB/v_MemberListDB.cs`
- `MdcHR26Apps.BlazorServer/Components/Pages/Auth/Login.razor`

---

### 2. Navigation 상태 관리 개선
**문제**: Login.razor에서 `forceLoad: true` 사용으로 상태 손실

**해결**:
- `UrlActions.MoveMainPage()` 사용
- SPA 라우팅으로 상태 유지
- Logout.razor는 `forceLoad: true` 유지 (세션 초기화 필요)

**수정 파일**:
- `MdcHR26Apps.BlazorServer/Components/Pages/Auth/Login.razor`
- `MdcHR26Apps.BlazorServer/Components/Pages/Auth/Logout.razor`

---

### 3. Playwright 테스트 환경 구축
**추가 파일**:
- `tests/basic.spec.js` (4개 테스트) ✅ 통과
- `tests/auth.spec.js` (7개 로그인 테스트) - 내일 수정 예정
- `MdcHR26Apps.BlazorServer/Components/Routes.razor` (404 페이지 수정)

**테스트 결과**:
- basic.spec.js: 3개 통과, 1개 flaky
- auth.spec.js: 아직 미실행 (PageTitle, selector 수정 필요)

---

### 4. 프로젝트 동기화 관리 시스템
**목적**: 현재 프로젝트 ↔ 실제 프로젝트 간 안전한 코드 동기화

**추가 사항**:
- Custom Agent: `checklist-generator` (체크리스트 생성)
- Custom Agent: `sync-validator` (동기화 검증)
- `CLAUDE.md`에 "프로젝트 동기화" 섹션 추가
- `.gitignore`에 `.sync/` 폴더 제외

**워크플로우**:
1. 작업 완료 → Git commit
2. checklist-generator로 체크리스트 생성
3. 개발자가 수동 복사
4. sync-validator로 검증
5. 실제 프로젝트 빌드 및 테스트

---

## 개발자 피드백

**작업 완료 확인**: 2026-01-22
**최종 상태**: 완료 ✅
**비고**:
- 로그인 정상 작동 확인
- 근본 원인: SQL NVARCHAR(Unicode)와 C# UTF-8 인코딩 불일치
- 도서관리프로그램 코드 참조하여 해결
- DB 쿼리 최적화 및 테스트 인프라 구축 완료
- 프로젝트 동기화 시스템 구축 완료
