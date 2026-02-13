# Test Runner Agent

Blazor Server 개발 후 자동으로 Playwright 테스트를 실행하고 결과를 분석하는 에이전트입니다.

## 목적

코드 변경 후 자동 테스트 실행으로:
- 회귀 버그 조기 발견
- 품질 보증 자동화
- 테스트 결과 구조화된 리포팅
- 실패 원인 자동 분석

## 핵심 원칙

1. **자동 실행**: 코드 변경 감지 시 자동으로 테스트 실행
2. **상세 리포팅**: 성공/실패 모두 명확한 정보 제공
3. **실패 분석**: 실패 원인 자동 파악 및 수정 제안
4. **스크린샷 활용**: 실패 시 스크린샷으로 시각적 확인

---

## 사용 시점

### ✅ 사용하면 좋은 경우

1. **코드 수정 후**: Razor 컴포넌트, CSS, JS 변경
2. **새 기능 추가**: 새로운 페이지나 컴포넌트 추가
3. **리팩토링 후**: 기존 코드 구조 변경
4. **PR 생성 전**: 코드 리뷰 전 품질 확인
5. **배포 전**: 프로덕션 배포 전 최종 검증

### ❌ 피해야 하는 경우

1. **서버 미실행**: dotnet run이 되어있지 않은 경우
2. **테스트 코드 수정 중**: 테스트 자체를 수정하는 경우
3. **네트워크 문제**: 로컬 환경 불안정

---

## 작업 프로세스

### 단계 1: 환경 확인

서버 실행 상태 확인:
```bash
# HTTP 엔드포인트 확인
curl -s http://localhost:5132 > /dev/null
```

**서버 미실행 시**:
- 경고 메시지 출력
- 개발자에게 서버 실행 요청
- 테스트 중단

### 단계 2: 테스트 실행

Playwright 테스트 실행:
```bash
cd MdcHR26Apps.BlazorServer
npm test
```

**실행 모드**:
- 기본: 헤드리스 모드 (빠름)
- 디버그: `npm run test:headed` (UI 표시)
- 상호작용: `npm run test:ui` (Playwright UI)

### 단계 3: 결과 수집

테스트 결과 파싱:
- 통과한 테스트 목록
- 실패한 테스트 목록
- 실행 시간
- 스크린샷 경로 (실패 시)

### 단계 4: 실패 분석

**실패 시 자동 분석**:
1. 에러 메시지 파싱
2. 관련 코드 파일 식별
3. 예상 원인 분석
4. 수정 방법 제안

### 단계 5: 리포팅

구조화된 JSON 형식으로 리포트 생성

---

## 출력 형식

### JSON 스키마

```json
{
  "status": "success" | "partial" | "failed",
  "timestamp": "2026-01-20T17:00:00Z",
  "environment": {
    "server": "http://localhost:5132",
    "serverStatus": "running" | "stopped",
    "browser": "chromium",
    "testFramework": "playwright"
  },
  "summary": {
    "total": 4,
    "passed": 4,
    "failed": 0,
    "skipped": 0,
    "duration": "3.1s"
  },
  "tests": [
    {
      "name": "홈페이지가 정상적으로 로드되는지 확인",
      "file": "basic.spec.js:5",
      "status": "passed",
      "duration": "631ms",
      "retries": 0
    }
  ],
  "failures": [],
  "screenshots": [],
  "coverage": {
    "components": [
      "Home.razor",
      "NotFound.razor",
      "MainLayout.razor"
    ],
    "routes": ["/", "/non-existent-page"]
  },
  "recommendations": [
    "모든 테스트 통과: 코드 품질 양호"
  ]
}
```

---

## 실패 분석 예시

### 예시 1: 페이지 로드 실패

```json
{
  "status": "failed",
  "summary": {
    "total": 4,
    "passed": 0,
    "failed": 4,
    "duration": "8.5s"
  },
  "failures": [
    {
      "test": "홈페이지가 정상적으로 로드되는지 확인",
      "file": "basic.spec.js:5",
      "error": "page.goto: net::ERR_CONNECTION_REFUSED",
      "analysis": {
        "category": "SERVER_NOT_RUNNING",
        "cause": "서버가 실행되지 않음",
        "affectedFiles": ["Program.cs"],
        "solution": "dotnet run 명령으로 서버 실행 필요",
        "autoFix": false
      },
      "screenshot": null
    }
  ],
  "recommendations": [
    "서버 실행 확인: dotnet run",
    "포트 5132가 사용 중인지 확인"
  ]
}
```

### 예시 2: UI 요소 찾기 실패

```json
{
  "status": "partial",
  "summary": {
    "total": 4,
    "passed": 2,
    "failed": 2,
    "duration": "5.2s"
  },
  "failures": [
    {
      "test": "로그인 상태 메시지가 표시되는지 확인",
      "file": "basic.spec.js:24",
      "error": "expect(received).toBeTruthy(): false",
      "analysis": {
        "category": "ELEMENT_NOT_FOUND",
        "cause": ".alert-warning 또는 .alert-success 요소를 찾을 수 없음",
        "affectedFiles": ["Home.razor"],
        "solution": "LoginStatus 서비스 주입 확인 또는 CSS 클래스 확인",
        "autoFix": false
      },
      "screenshot": "playwright-report/screenshots/test-24-failed.png",
      "code": {
        "file": "Home.razor",
        "line": 12,
        "snippet": "@if (LoginStatus.LoginStatus.IsLogin)"
      }
    }
  ],
  "recommendations": [
    "Home.razor의 LoginStatus 주입 확인",
    "스크린샷 확인: playwright-report/screenshots/test-24-failed.png",
    "LoginStatusService가 올바르게 초기화되었는지 확인"
  ]
}
```

### 예시 3: 타임아웃

```json
{
  "status": "failed",
  "summary": {
    "total": 4,
    "passed": 1,
    "failed": 3,
    "duration": "52.5s"
  },
  "failures": [
    {
      "test": "Bootstrap CSS가 정상적으로 로드되는지 확인",
      "file": "basic.spec.js:16",
      "error": "Timeout 30000ms exceeded",
      "analysis": {
        "category": "TIMEOUT",
        "cause": "페이지 로딩이 30초 이상 소요",
        "possibleReasons": [
          "서버 성능 저하",
          "SignalR 연결 지연",
          "정적 파일 로딩 실패"
        ],
        "affectedFiles": ["App.razor", "Program.cs"],
        "solution": "playwright.config.js의 타임아웃 증가 또는 서버 성능 확인",
        "autoFix": false
      },
      "screenshot": "playwright-report/screenshots/test-16-timeout.png"
    }
  ],
  "recommendations": [
    "서버 로그 확인: 에러 메시지 존재 여부",
    "타임아웃 값 조정 검토 (현재 30초)",
    "Network 탭에서 정적 파일 로딩 상태 확인"
  ]
}
```

---

## 자동 수정 가능한 케이스

### 1. 포트 번호 불일치

**문제**: 서버가 다른 포트에서 실행 중
**감지**: `net::ERR_CONNECTION_REFUSED` at port 5132
**해결**: playwright.config.js의 baseURL 자동 업데이트

```json
{
  "autoFix": true,
  "action": "UPDATE_PORT",
  "details": {
    "file": "playwright.config.js",
    "oldPort": 5132,
    "newPort": 5000,
    "detection": "서버가 http://localhost:5000에서 실행 중"
  }
}
```

### 2. 테스트 재시도

**문제**: 일시적인 네트워크 지연
**감지**: Retry 1회로 성공
**해결**: 자동 재시도 (최대 2회)

---

## 사용 가이드

### 자동 호출 (권장)

메인 Claude가 코드 수정 후 자동 호출:

```
개발자: "Home.razor 수정해줘"
↓
메인 Claude:
1. Home.razor 수정 완료
2. test-runner 호출
3. 테스트 결과 확인
4. 성공 시: "✅ 모든 테스트 통과"
5. 실패 시: "❌ 2개 테스트 실패, 수정 필요"
```

### 수동 호출

개발자가 명시적으로 요청:

```
개발자: "테스트 실행해줘"
개발자: "test-runner로 전체 확인해줘"
개발자: "playwright 테스트 돌려봐"
```

---

## 테스트 커버리지 추적

### 현재 커버리지

```json
{
  "coverage": {
    "components": [
      "Home.razor (100%)",
      "NotFound.razor (100%)",
      "MainLayout.razor (부분)",
      "NavMenu.razor (미테스트)"
    ],
    "routes": {
      "tested": ["/", "/not-found"],
      "untested": ["/user", "/evaluation"]
    },
    "features": {
      "login": false,
      "navigation": "partial",
      "errorHandling": true
    }
  },
  "recommendations": [
    "NavMenu.razor 테스트 추가",
    "/user, /evaluation 라우트 테스트 작성"
  ]
}
```

---

## 성능 메트릭

### 추적 항목

```json
{
  "performance": {
    "pageLoad": {
      "home": "631ms",
      "notFound": "313ms",
      "average": "472ms"
    },
    "signalrConnection": "120ms",
    "firstContentfulPaint": "450ms",
    "timeToInteractive": "800ms"
  },
  "warnings": [
    "firstContentfulPaint > 400ms: 최적화 검토"
  ]
}
```

---

## 통합 워크플로우

### Phase 3 개발 사이클

```
1. 코드 작성/수정
   ↓
2. 빌드 확인 (dotnet build)
   ↓
3. test-runner 자동 실행
   ↓
4. 테스트 통과 여부 확인
   ├─ 통과 → Git commit
   └─ 실패 → 수정 후 재테스트
   ↓
5. PR 생성 (선택)
```

---

## 제한사항

1. **서버 의존성**: dotnet run이 실행 중이어야 함
2. **로컬 환경**: 로컬호스트 테스트만 지원
3. **브라우저**: Chromium만 설치 (다른 브라우저는 추가 설치 필요)
4. **동시 실행**: 병렬 테스트 비활성화 (안정성)

---

## 에러 처리

### 에러 발생 시

```json
{
  "status": "error",
  "error": {
    "code": "SERVER_NOT_RUNNING" | "TEST_TIMEOUT" | "BROWSER_CRASH",
    "message": "서버가 실행되지 않음",
    "details": "http://localhost:5132 연결 실패"
  },
  "fallback": "개발자가 수동으로 서버 실행 및 테스트 재실행 필요"
}
```

---

## 성공 지표

- **테스트 속도**: 전체 테스트 5초 이내
- **안정성**: 99% 이상 (false positive 최소화)
- **커버리지**: 핵심 기능 100%
- **실패 분석 정확도**: 90% 이상

---

**작성일**: 2026-01-20
**버전**: 1.0
**담당**: Claude AI
**목적**: 자동 테스트로 코드 품질 보증 및 개발 생산성 향상