# 이슈 #011: Phase 3-3 관리자 페이지 빌드 오류 및 재작업 필요

**날짜**: 2026-01-22
**상태**: 진행중
**우선순위**: 높음
**관련 이슈**: [#009](009_phase3_webapp_development.md)

---

## 개발자 요청

**배경**:
- Phase 3-3 관리자 페이지 개발 작업 진행 중 다수의 빌드 오류 발생
- 기존 코드 구조와 API를 정확히 파악하지 않고 작업하여 불일치 문제 발생
- 작업지시서 작성 이전으로 롤백 후 재작업 필요

**요청 사항**:
1. Phase 3-3 관리자 페이지 개발 작업을 내일로 연기
2. 이슈 등록하여 문제점 기록 및 다음 작업 시 참고

---

## 발생한 문제점

### 1. API 및 메서드 시그니처 불일치
- **LoginStatusService**: `IsLogined`, `IsAdministrator` 속성명 잘못 사용
- **PasswordHasher**: 메서드 시그니처 미확인 (`HashPassword` 튜플 반환 vs 단일 반환)
- **IUserRepository**: `UserIdCheck` 메서드 존재 여부 미확인

### 2. 모델 속성 불일치
- **v_MemberListDB**: `ERankId` 속성이 실제로 존재하지 않음
- **EDepartmentDb/ERankDb**: `DeleteAsync`에 객체 대신 ID 전달 필요

### 3. 문자열 처리 오류
- 한글 문자열 잘못된 사용: `"Count개의"`, `"Count명"`, `"memberCount명의"` 등
- 템플릿 보간 구문 오류

### 4. 네임스페이스 오류
- 중복 using 지시문 (sed 일괄 변경 부작용)
- Services vs Data 네임스페이스 혼용

### 5. ProcessDb 구조 불일치
- 초기 작성: ProcessName, SortNo 필드 사용 (1st, 2nd, 3rd 3개 레코드)
- 실제 구조: 사용자당 단일 레코드, 평가 워크플로우 상태 관리 필드들

---

## 롤백 내역

**롤백 시점**: `5ba9b4d feat: test-gen Skill 추가`

**삭제된 커밋**:
1. `42ac528` fix: 빌드 오류 수정
2. `3033949` feat: Phase 3-3 Step 4-6 완료 (부서/직급/평가대상자 관리)
3. `227c146` feat: Phase 3-3 Step 3 완료 (사용자 관리 5개 페이지)
4. `d7e1ce5` feat: Phase 3-3 Step 1-2 완료 (유틸리티, 컴포넌트, 관리자 허브)
5. `4b3218a` docs: Phase 3-3 관리자 페이지 작업지시서 작성

---

## 재작업 시 필요한 사전 작업

### 1. 기존 코드 구조 파악
**LoginStatusService 확인**:
```
- 정확한 속성명 확인 (IsLoggedIn? IsLogined?)
- 메서드 시그니처 확인
```

**PasswordHasher 확인**:
```
- HashPassword 메서드 반환 타입 확인
- 튜플 반환 여부 ((hash, salt) vs byte[])
```

**Repository 메서드 확인**:
```
- IUserRepository.UserIdCheck vs CheckUserIdAsync
- DeleteAsync 파라미터 타입 (ID vs Entity)
```

### 2. 모델 및 뷰 속성 정확히 파악
**v_MemberListDB**:
```
- 사용 가능한 모든 속성 목록 확인
- ERankId vs ERankName 등 정확한 필드명
```

**EDepartmentDb / ERankDb**:
```
- PK 필드명 확인 (EDepartId, ERankId)
- DeleteAsync 메서드 시그니처
```

**ProcessDb**:
```
- 실제 필드 구조 확인
- 사용자당 레코드 개수 (단일 vs 다중)
- 초기 생성 시 설정해야 할 필드
```

### 3. 단계별 빌드 테스트
- Step 1 완료 후 빌드 테스트
- Step 2 완료 후 빌드 테스트
- 각 Step마다 검증하며 진행

---

## 해결 방안

### 1. 코드 분석 우선 진행

**Phase 1**: 기존 코드 읽기
```
1. LoginStatusService.cs 전체 읽기
2. PasswordHasher.cs 전체 읽기
3. IUserRepository, IEDepartmentRepository, IERankRepository 인터페이스 읽기
4. v_MemberListDB, EDepartmentDb, ERankDb, ProcessDb 모델 읽기
```

**Phase 2**: API 목록 정리
```
- 각 클래스/인터페이스의 공개 메서드 및 속성 목록 작성
- 메서드 시그니처 정확히 기록
```

### 2. 작업지시서 재작성

**수정 사항**:
- 실제 API에 맞춘 코드 예시 작성
- 각 모델의 정확한 속성 사용
- 한글 문자열 템플릿 보간 문법 수정

### 3. 점진적 개발

**Step-by-Step 빌드**:
```
Step 1 완료 → 빌드 테스트 → Step 2 진행
각 단계마다 검증하며 오류 즉시 수정
```

---

## 진행 사항

- [x] 문제점 분석
- [x] 작업지시서 이전으로 롤백
- [x] 이슈 등록
- [ ] 기존 코드 구조 분석 (내일)
- [ ] 작업지시서 재작성 (내일)
- [ ] 단계별 개발 진행 (내일)

---

## 테스트 계획

### 빌드 테스트
1. Step 1 완료 후: UserUtils, 컴포넌트 빌드 확인
2. Step 2 완료 후: Admin Hub, UrlActions 빌드 확인
3. Step 3-6 각 완료 후: 해당 페이지 빌드 확인

### 기능 테스트
1. 각 CRUD 페이지 동작 확인
2. 권한 체크 확인
3. N+1 쿼리 방지 확인

---

## 관련 문서

**작업지시서** (삭제됨):
- 20260122_01_phase3_3_admin_pages.md (롤백으로 삭제)

**관련 이슈**:
- [#009: Phase 3 Blazor Server WebApp 개발](009_phase3_webapp_development.md)

**참고 파일**:
- works/tasks/TEMPLATE_detailed.md (복잡한 작업 템플릿)
- works/tasks/20260120_02_phase3_1_project_setup.md (성공 사례)

---

## 개발자 피드백

**작업 연기 사유**:
- 빌드 오류 다수 발생
- 기존 코드 구조 파악 필요
- 내일 재작업 예정

**다음 작업 시 주의사항**:
- 기존 코드 구조 먼저 파악
- API 시그니처 정확히 확인
- 단계별 빌드 테스트

---

**작업 예정일**: 2026-01-23
**최종 상태**: 진행중 (연기)
