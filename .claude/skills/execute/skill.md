# Execute - 작업지시서 자동 실행

작업지시서를 읽고 자동으로 실행하는 스킬입니다.
**Plan Mode 기반으로 권한을 한 번에 승인**받아 작업을 진행합니다.

---

## 사용법

```bash
/execute [작업지시서번호]
/execute                  # 최신 작업지시서 실행
```

---

## 예시

```bash
# 특정 작업지시서 실행
/execute 20260128_01

# 최신 작업지시서 자동 실행
/execute
```

---

## 동작 순서

### 1단계: 작업지시서 분석
- works/tasks/ 폴더에서 작업지시서 읽기
- 파라미터 생략 시 최신 파일 자동 선택
- 작업지시서 구조 파싱 (Step별 작업 내용)

### 2단계: Plan Mode 진입
- **EnterPlanMode** 호출
- 작업지시서 기반 작업 계획 수립
- 필요한 모든 작업과 권한 분석

### 3단계: 필요 권한 명시
**allowedPrompts 생성**:
- `Write`: 새 파일 생성
- `Edit`: 기존 파일 수정
- `Bash`: 빌드 테스트 (dotnet build)
- `Read`: 참고 파일 읽기

### 4단계: Plan 제출
- **ExitPlanMode** 호출
- 개발자에게 Plan 및 권한 승인 요청
- **한 번만 승인받음** ✅

### 5단계: 자동 실행
- Step별로 순차 실행
- 각 Step 완료 후 빌드 테스트
- 오류 발생 시 즉시 중단 및 보고

### 6단계: 완료 처리
- 이슈 자동 업데이트 (issue-updater 자동 실행)
- 완료 보고서 생성

---

## 권한 승인 방식

### 기존 방식 (수동)
```
파일 1 생성 → [권한 승인 1]
파일 2 생성 → [권한 승인 2]
파일 3 생성 → [권한 승인 3]
빌드 테스트 → [권한 승인 4]
(매번 승인 필요)
```

### execute Skill 방식
```
/execute 호출
↓
Plan 확인 및 권한 승인 [1회만!]
↓
파일 1, 2, 3 생성 + 빌드 테스트 (자동)
```

---

## 작업지시서 요구사항

execute Skill이 작동하려면 작업지시서가 다음 구조를 따라야 합니다:

### 필수 섹션

#### 1. 작업 단계 (Step)
```markdown
## 3. 작업 단계

### Step 1: [작업명]
- 파일: [경로]
- 작업: Write/Edit
- 내용: [상세 설명]

### Step 2: [작업명]
...
```

#### 2. 빌드 테스트
```markdown
### 빌드 테스트
dotnet build
```

#### 3. 완료 조건
```markdown
## 완료 조건
- [ ] Step 1 완료
- [ ] Step 2 완료
```

---

## 실행 예시

### Case 1: 특정 작업지시서 실행

**명령**:
```bash
/execute 20260128_01
```

**실행 과정**:
```
[1] 작업지시서 읽기
    ✅ works/tasks/20260128_01_implement_missing_components.md

[2] Plan Mode 진입
    ✅ 작업 계획 수립

    작업 계획:
    ━━━━━━━━━━━━━━━━━━━━━━
    Step 1: DisplayResultText.razor 생성
    Step 2: EUserListTable.razor 생성
    Step 3: MemberListTable.razor 생성
    각 Step 후: dotnet build
    ━━━━━━━━━━━━━━━━━━━━━━

    필요 권한:
    - Write: 파일 생성 (3개)
    - Bash: 빌드 테스트 (3회)

[3] 개발자 승인
    → [Plan 승인] 클릭 (1회)

[4] 자동 실행
    ✅ Step 1: DisplayResultText.razor 생성
    ✅ Bash: dotnet build (RZ10012 2건 감소)

    ✅ Step 2: EUserListTable.razor 생성
    ✅ Bash: dotnet build (RZ10012 1건 감소)

    ✅ Step 3: MemberListTable.razor 생성
    ✅ Bash: dotnet build (RZ10012 2건 감소)

[5] 완료 보고
    ━━━━━━━━━━━━━━━━━━━━━━
    작업 완료
    ━━━━━━━━━━━━━━━━━━━━━━
    생성된 파일: 3개
    빌드 경고: 10개 → 5개
    상태: 성공 ✅
```

---

### Case 2: 최신 작업지시서 자동 실행

**명령**:
```bash
/execute
```

**실행 과정**:
```
[1] 최신 작업지시서 찾기
    ✅ works/tasks/ 스캔
    ✅ 20260128_01_implement_missing_components.md (최신)

[2] 이하 동일...
```

---

## 오류 처리

### 빌드 실패 시
```
[4] Step 2 실행 중...
    ✅ EUserListTable.razor 생성

[5] 빌드 테스트
    ❌ 오류 발생:
       CS0246: 'EvaluationUsers' 형식을 찾을 수 없습니다.

→ 작업 중단
→ 개발자에게 오류 보고
→ 수정 후 /execute 재실행 가능
```

### 작업지시서 없음
```
/execute 20260199_01

❌ 오류: 작업지시서를 찾을 수 없습니다.
   경로: works/tasks/20260199_01_*.md
```

---

## 장점

| 구분 | 수동 실행 | execute Skill |
|------|----------|--------------|
| **권한 승인** | 매 작업마다 요청 | **1회만** ✅ |
| **작업 일관성** | 수동으로 단계 진행 | 작업지시서 기반 자동화 |
| **오류 감지** | 개발자가 직접 확인 | 각 Step 후 자동 빌드 테스트 |
| **중간 중단 위험** | 높음 | 낮음 (자동 진행) |
| **개발자 작업** | 계속 모니터링 필요 | 승인 후 다른 업무 가능 |

---

## 제한사항

### 1. 작업지시서 구조 의존
- 작업지시서가 명확한 Step 구조를 가져야 함
- Step별 파일 경로와 작업 내용이 명시되어야 함

### 2. 단순 작업에 적합
- 파일 생성/수정이 명확한 경우
- 복잡한 비즈니스 로직은 수동 검토 필요

### 3. 오류 발생 시 중단
- 빌드 실패 시 즉시 중단
- 수정 후 재실행 필요

---

## 연계 동작

### issue-updater Agent 자동 실행
```
/execute 완료
↓
issue-updater 자동 실행
↓
관련 이슈 진행률 업데이트
```

### test-runner Agent 연계 (향후)
```
/execute 완료
↓
test-runner 자동 실행
↓
Playwright 테스트 실행
```

---

## 권장 사용 시나리오

### ✅ 적합한 경우
- 작업지시서가 명확하게 작성된 경우
- 파일 생성/수정이 3개 이상인 경우
- 단계별 빌드 테스트가 필요한 경우
- 개발자가 다른 업무를 병행하는 경우

### ❌ 부적합한 경우
- 작업지시서가 모호한 경우
- 복잡한 판단이 필요한 경우
- 1-2개 파일만 수정하는 간단한 경우

---

## 관련 Skills

- **task Skill**: 작업지시서 생성 → execute로 실행
- **issue Skill**: 이슈 생성 → execute로 작업 수행
- **db-design Skill**: DB 작업지시서 생성 → execute로 실행

---

## 개발자 노트

### Plan Mode 활용
이 Skill의 핵심은 **Plan Mode + allowedPrompts**입니다:
- EnterPlanMode로 작업 계획 수립
- ExitPlanMode(allowedPrompts)로 필요한 모든 권한 명시
- 개발자 승인 후 자동 실행

### 작업지시서 표준화
execute Skill을 효과적으로 사용하려면:
- task Skill로 작업지시서 생성 (표준 형식)
- Step 구조를 명확하게 정의
- 빌드 테스트 포함

---

**버전**: 1.0.0
**최초 작성**: 2026-01-29
**작성자**: Claude AI
