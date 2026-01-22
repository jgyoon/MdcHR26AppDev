# Checklist Generator Agent

프로젝트 동기화를 위한 체크리스트 자동 생성 에이전트

## 목적

현재 프로젝트에서 실제 프로젝트로 코드를 복사할 때 사용할 **체크리스트 자동 생성**

## 핵심 원칙

1. **읽기 전용**: 파일을 수정하지 않음
2. **간소화된 체크리스트**: 생성/수정/삭제로 파일 분류
3. **Git 기반**: 마지막 동기화 이후 변경된 파일만 추출 (`git diff --name-status`)

---

## 주요 기능

### 1. 변경 파일 목록 생성

마지막 동기화 커밋 이후 변경된 파일을 Git log로 추출

**대상 폴더:**
- `MdcHR26Apps.BlazorServer/`
- `MdcHR26Apps.Models/`

**제외 대상:**
- `tests/` (테스트 파일)
- `bin/`, `obj/` (빌드 산출물)
- `.sync/` (동기화 로그)

### 2. 체크리스트 마크다운 생성

**출력 위치:** `works/sync-checklists/YYYYMMDD_HHMM_sync_checklist.md`

**형식:**
```markdown
# 동기화 체크리스트

**생성일시**: 2026-01-22 15:30
**현재 커밋**: acca6c3
**마지막 동기화**: c8eb65f (선택사항)
**변경 파일**: 5개

---

## 📋 작업 파일 목록

### 1. 생성:
- MdcHR26Apps.BlazorServer/Components/Pages/NewPage.razor
- MdcHR26Apps.Models/NewModel/NewModel.cs

### 2. 수정:
- MdcHR26Apps.BlazorServer/Components/Pages/Auth/Login.razor
- MdcHR26Apps.BlazorServer/Components/Pages/Auth/Logout.razor
- MdcHR26Apps.BlazorServer/appsettings.json

### 3. 삭제:
- MdcHR26Apps.BlazorServer/wwwroot/css/LoadingSpinner.css
- MdcHR26Apps.BlazorServer/wwwroot/css/app.css

---

## 📌 메모

**커밋 정보**: acca6c3 (fix: 로그인 인증 수정)

**주요 변경 사항**:
- 로그인 인증 로직 개선
- 미사용 CSS 파일 제거

---

**완료 일시**: __________
```

---

## 사용 방법

### 1. 체크리스트 생성 요청

```
사용자: "checklist-generator로 동기화 체크리스트 만들어줘"
```

### 2. Agent 작업 순서

1. `.sync/last-sync-commit.txt` 읽기 (없으면 전체 파일)
2. `git diff --name-status <마지막커밋>..HEAD` 실행
3. 파일 상태별로 분류:
   - `A` (Added) → **생성** 목록
   - `M` (Modified) → **수정** 목록
   - `D` (Deleted) → **삭제** 목록
4. Git 커밋 메시지 및 주요 변경 사항 요약
5. 간소화된 체크리스트 마크다운 생성
6. `works/sync-checklists/` 폴더에 저장

### 3. 개발자 작업

생성된 체크리스트를 보면서:
- **생성** 파일: 현재 프로젝트 → 실제 프로젝트로 복사
- **수정** 파일: 현재 프로젝트 → 실제 프로젝트로 덮어쓰기
- **삭제** 파일: 실제 프로젝트에서 삭제
- 빌드 및 테스트
- Git commit

---

## 출력 예시

```markdown
# 동기화 체크리스트

**생성일시**: 2026-01-22 15:30
**현재 커밋**: acca6c3 (fix: 로그인 인증 수정)
**마지막 동기화**: b18a50f
**변경 파일**: 5개

---

## 📋 작업 파일 목록

### 1. 생성:
없음

### 2. 수정:
- MdcHR26Apps.BlazorServer/Components/Pages/Auth/Login.razor
- MdcHR26Apps.BlazorServer/Components/Pages/Auth/Logout.razor
- MdcHR26Apps.BlazorServer/Components/Routes.razor
- MdcHR26Apps.Models/Views/v_MemberListDB/v_MemberListDB.cs
- MdcHR26Apps.Models/User/UserRepository.cs

### 3. 삭제:
없음

---

## 📌 메모

**커밋 정보**: acca6c3 (fix: 로그인 인증 수정)

**주요 변경 사항**:
- v_MemberListDB 활용으로 DB 쿼리 최적화
- forceLoad 제거, UrlActions.MoveMainPage() 사용

---

**완료 일시**: __________
```

---

## 제약사항

- **파일 수정 금지**: 읽기만 수행
- **자동 복사 금지**: 체크리스트만 생성
- **Git 의존성**: Git이 설치되어 있어야 함

---

## 트러블슈팅

### `.sync/last-sync-commit.txt` 없음
→ 첫 동기화로 간주, 모든 파일 포함

### Git 명령어 실패
→ 현재 디렉토리가 Git 저장소인지 확인

### 변경 파일 없음
→ "변경 사항 없음" 메시지 출력

---

**작성일**: 2026-01-22
**버전**: 2.0 (간소화된 형식)
**담당**: Claude AI
**변경 이력**:
- v2.0 (2026-01-22): 복사 단계 제거, 생성/수정/삭제 분류 방식으로 변경
- v1.0 (2026-01-22): 초기 버전
