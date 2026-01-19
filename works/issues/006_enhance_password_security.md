# 이슈 #006: UserDb 비밀번호 보안 강화

**날짜**: 2026-01-14
**상태**: 완료
**우선순위**: 높음
**관련 이슈**: [#004](004_phase1_database_design.md)

---

## 개발자 요청

**배경**:
- 현재 비밀번호 처리: Salt 없이 단순 해시 방식
- Rainbow Table 공격에 취약
- 참고 프로젝트(01_MdcLibrary)의 개선된 방식 적용 필요

**요청 사항**:
1. UserPasswordSalt 필드 추가
2. SHA-256 해시값 크기 최적화 (32바이트)
3. Seed Data에서 Salt 기반 해시 생성 로직 구현

---

## 해결 방안

### 1. 보안 강화 방식
- **Salt 사용**: 동일한 비밀번호도 사용자마다 다른 해시값 생성
- **Rainbow Table 공격 방지**: Salt로 사전 계산된 해시 테이블 무력화
- **SHA-256 알고리즘**: 안전한 일방향 암호화
- **크기 최적화**: 정확한 바이트 크기 지정

### 2. 스키마 변경
```sql
-- 변경 전
[UserPassword] VARBINARY(100) NOT NULL

-- 변경 후
[UserPassword] VARBINARY(32) NOT NULL,      -- SHA-256 해시 (32바이트)
[UserPasswordSalt] VARBINARY(16) NOT NULL   -- Salt (16바이트)
```

---

## 진행 사항

- [x] 작업지시서 작성
- [x] UserDb.sql 수정
- [x] 01_CreateTables.sql 수정
- [x] 03_SeedData.sql 수정 (Salt 기반 해시 생성)
- [x] 개발자 승인
- [x] 테스트 완료

---

## 테스트 결과

### Test 1: 스키마 생성
**결과**: 성공
- UserPassword: VARBINARY(32) 확인
- UserPasswordSalt: VARBINARY(16) 확인

### Test 2: 시드 데이터 입력
**결과**: 성공
- 관리자 계정 생성 확인
- Salt 값 정상 저장 확인

### Test 3: 비밀번호 검증
**결과**: 성공
- 올바른 비밀번호(admin1234): 로그인 성공
- 잘못된 비밀번호: 로그인 실패

---

## 비밀번호 검증 로직

### 로그인 시 검증 SQL
```sql
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

---

## 영향도 분석

### 수정된 파일
1. Database/dbo/UserDb.sql
2. Database/01_CreateTables.sql
3. Database/03_SeedData.sql

### 영향받지 않는 부분
- 외래키 관계: UId는 변경 없음
- 뷰(View): UserPassword를 조회하지 않으므로 영향 없음

### 향후 개발 시 주의사항
- User 모델에 `UserPasswordSalt` 속성 추가 필요
- UserRepository의 비밀번호 검증 로직에 Salt 처리 포함
- 회원가입 시 Salt 자동 생성 로직 구현

---

## 관련 문서

**작업지시서**:
- [20260114_01_enhance_password_security.md](../tasks/20260114_01_enhance_password_security.md)

**수정된 파일**:
- Database/dbo/UserDb.sql
- Database/01_CreateTables.sql
- Database/03_SeedData.sql

**관련 이슈**:
- [#004: Phase 1 데이터베이스 설계](004_phase1_database_design.md) - 선행 작업

---

## 개발자 피드백

**작업 완료 확인**: 2026-01-14
**최종 상태**: 완료
**비고**:
- Salt 기반 비밀번호 해시 적용 완료
- Rainbow Table 공격 방어 기능 추가
- SHA-256 알고리즘으로 보안 강화
