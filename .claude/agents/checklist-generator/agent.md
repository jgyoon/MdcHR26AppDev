# Checklist Generator Agent

프로젝트 동기화를 위한 체크리스트 자동 생성 에이전트

## 목적

현재 프로젝트에서 실제 프로젝트로 코드를 복사할 때 사용할 **체크리스트 자동 생성**

## 핵심 원칙

1. **읽기 전용**: 파일을 수정하지 않음
2. **체크리스트 생성**: 개발자가 따라할 수 있는 단계별 가이드 제공
3. **Git 기반**: 마지막 동기화 이후 변경된 파일만 추출

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

---

## 📋 변경 파일 목록 (5개)

### MdcHR26Apps.BlazorServer (3개)
- [ ] Components/Pages/Auth/Login.razor
- [ ] Components/Pages/Auth/Logout.razor
- [ ] Components/Routes.razor

### MdcHR26Apps.Models (2개)
- [ ] Views/v_MemberListDB/v_MemberListDB.cs
- [ ] User/UserRepository.cs

---

## 🔄 복사 절차

### 파일 1: Login.razor

**현재 프로젝트:**
```
C:\Codes\00_Develop_Cursor\10_MdcHR26Apps\MdcHR26Apps.BlazorServer\Components\Pages\Auth\Login.razor
```

**실제 프로젝트:**
```
C:\Codes\41_MdcHR26\MdcHR26App\MdcHR26Apps.BlazorServer\Components\Pages\Auth\Login.razor
```

**변경 사항:**
- v_MemberListDB 활용으로 DB 쿼리 최적화
- forceLoad 제거, UrlActions.MoveMainPage() 사용

**복사 단계:**
1. [ ] VSCode에서 Login.razor 열기
2. [ ] Ctrl+A (전체 선택)
3. [ ] Ctrl+C (복사)
4. [ ] Visual Studio 2022에서 Login.razor 열기
5. [ ] Ctrl+A (전체 선택)
6. [ ] Ctrl+V (붙여넣기)
7. [ ] Ctrl+S (저장)
8. [ ] 빌드 확인 (Ctrl+Shift+B)

---

### 파일 2: Logout.razor

(동일한 형식 반복)

---

## ✅ 최종 검증

### 빌드
- [ ] Visual Studio에서 전체 빌드 성공
- [ ] 경고 메시지 확인

### 수동 테스트
- [ ] 로그인 기능 테스트
- [ ] 로그아웃 기능 테스트
- [ ] 404 페이지 테스트

### Git Commit
- [ ] 실제 프로젝트 Git commit
- [ ] 커밋 메시지 작성
- [ ] .sync/last-sync-commit.txt 업데이트

---

## 📌 메모

(추가 메모 작성 공간)

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
2. `git log --name-only` 실행
3. 변경된 파일 목록 추출
4. 각 파일의 변경 내용 분석 (`git diff`)
5. 체크리스트 마크다운 생성
6. `works/sync-checklists/` 폴더에 저장

### 3. 개발자 작업

생성된 체크리스트를 보면서:
- 각 파일을 수동으로 복사
- 체크박스 체크
- 빌드 및 테스트
- Git commit

---

## 출력 예시

```markdown
# 동기화 체크리스트

**생성일시**: 2026-01-22 15:30
**현재 커밋**: acca6c3 (fix: 로그인 인증 수정)
**변경 파일**: 5개

## 📋 파일 목록

- [ ] Login.razor
- [ ] Logout.razor
- [ ] Routes.razor
- [ ] v_MemberListDB.cs
- [ ] UserRepository.cs

## 🔄 복사 절차

### Login.razor
경로: MdcHR26Apps.BlazorServer\Components\Pages\Auth\Login.razor
변경: v_MemberListDB 사용, forceLoad 제거

1. [ ] VSCode에서 복사
2. [ ] Visual Studio에 붙여넣기
3. [ ] 빌드 확인

(반복)
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
**버전**: 1.0
**담당**: Claude AI
