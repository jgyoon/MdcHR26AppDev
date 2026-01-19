# 2026년 인사평가프로그램 개발 로드맵

**날짜**: 2025-12-16
**작업 유형**: 프로젝트 로드맵 작성
**프로젝트**: MdcHR26Apps
**관련 이슈**: [#003: 2026년 인사평가프로그램 개발 로드맵](../issues/003_project_roadmap.md)

---

## 1. 프로젝트 개요

### 1.1. 목적
- 2025년 인사평가프로그램을 기반으로 2026년 버전 개발
- .NET 10 기반 Blazor Web App으로 업그레이드
- DB 테이블 간 연관성 구축 (외래키 활용)

### 1.2. 참고 프로젝트

| 프로젝트 | 경로 | 플랫폼 | 참고 목적 | 권한 |
|---------|------|--------|----------|------|
| 2025년 인사평가 | `C:\Codes\29_MdcHR25\MdcHR25Apps` | .NET 7 | DB 구조, App 구조 | Read Only |
| 메디칼파크 도서관 | `C:\Codes\00_Develop_Cursor\01_MdcLibrary` | .NET 8 | DB 외래키 구조 | Read Only |

### 1.3. 개발 환경
- **플랫폼**: .NET 10
- **프레임워크**: Blazor Web App
- **데이터베이스**: SQL Server
- **ORM**: Dapper
- **개발 환경**: Windows
- **배포 환경**: Docker (Rocky Linux)

### 1.4. 주요 개선사항
- DB 테이블 간 외래키 연결 (기존: 독립 테이블 → 변경: 연관 테이블)
- UI/UX는 기존 구조 유지 (추후 변경 가능)
- 컴포넌트화를 통한 재사용성 향상

---

## 2. 프로젝트 구조

### 2.1. 폴더 구조
```
MdcHR26Apps/
├── Database/           # SQL 스크립트 (테이블, 뷰, SP)
├── Model/             # Dapper 모델 및 Repository
├── App/               # Blazor Web App (개발자가 생성 예정)
├── works/             # Claude AI 작업 폴더
│   ├── tasks/        # 작업지시서
│   ├── issues/       # 이슈 관리
│   └── images/       # 참고 이미지
└── claude.md         # 프로젝트 작업 규칙
```

### 2.2. 2025년 프로젝트 DB 테이블 현황

#### 주요 테이블 (12개)
1. **UserDb** - 사용자 계정 정보
2. **EDepartmentDb** - 부서 정보
3. **MemberDb** - 사용자-부서 연결 (외래키 사용 중)
4. **AgreementDb** - 직무평가 합의
5. **SubAgreementDb** - 세부직무평가 합의
6. **ProcessDb** - 평가 프로세스 상태
7. **ReportDb** - 평가 보고서
8. **TasksDb** - 세부 업무 목록
9. **TotalReportDb** - 종합 평가 보고서
10. **EvaluationLists** - 평가 항목 리스트
11. **EvaluationUsers** - 평가 대상자 목록
12. **DeptObjectiveDb** - 부서 목표

#### 주요 뷰 (5개)
1. **v_MemberListDB** - 사용자-부서 조인 뷰
2. **v_DeptObjectiveListDb** - 부서 목표 리스트
3. **v_ProcessTRListDB** - 프로세스-종합보고서 조인
4. **v_ReportTaskListDB** - 보고서-업무 조인
5. **v_TotalReportListDB** - 종합 보고서 리스트

---

## 3. 개발 로드맵

### Phase 1: 데이터베이스 설계 및 구축 (1단계)

#### 1.1. DB 분석 및 설계
- **작업 내용**:
  - 2025년 DB 테이블 구조 분석
  - 테이블 간 관계도(ERD) 작성
  - 외래키 관계 정의
  - 정규화 검토 및 개선사항 도출

- **참고 사항**:
  - 도서관 프로젝트의 외래키 구조 참조
  - 기존 MemberDb는 이미 외래키 사용 중

- **산출물**:
  - ERD 다이어그램
  - 테이블 관계 정의서
  - DB 설계 문서

#### 1.2. 테이블 생성 스크립트 작성
- **작업 순서**:
  1. 기본 테이블 (외래키 없음)
     - EDepartmentDb (부서)
     - UserDb (사용자)

  2. 연관 테이블 (외래키 포함)
     - MemberDb (사용자-부서 연결)
     - AgreementDb (직무평가 합의)
     - SubAgreementDb (세부직무평가 합의)
     - ProcessDb (평가 프로세스)
     - ReportDb (평가 보고서)
     - TasksDb (세부 업무)
     - TotalReportDb (종합 평가)
     - EvaluationLists (평가 항목)
     - EvaluationUsers (평가 대상자)
     - DeptObjectiveDb (부서 목표)

- **주요 개선사항**:
  - UserId, UserName 중복 → UserDb 외래키 참조로 변경
  - 평가자 정보 중복 → 외래키 참조로 변경
  - 부서 정보 중복 제거

#### 1.3. 뷰(View) 생성
- **작업 내용**:
  - 기존 5개 뷰 재작성
  - 외래키 기반 조인 뷰 추가 생성
  - 성능 최적화 고려

#### 1.4. 초기 데이터(Seed Data) 준비
- **작업 내용**:
  - 부서 마스터 데이터
  - 관리자 계정 생성
  - 평가 항목 기본 데이터
  - 테스트 데이터 준비

#### 1.5. DB 테스트
- **검증 항목**:
  - 테이블 생성 확인
  - 외래키 제약조건 동작 확인
  - 뷰 조회 성능 테스트
  - 데이터 무결성 검증

---

### Phase 2: Model 개발 (2단계)

#### 2.1. Entity 모델 작성
- **작업 내용**:
  - 각 테이블에 대응하는 C# 클래스 작성
  - 데이터 어노테이션 추가
  - DTO(Data Transfer Object) 정의

- **모델 목록** (12개):
  1. UserModel
  2. EDepartmentModel
  3. MemberModel
  4. AgreementModel
  5. SubAgreementModel
  6. ProcessModel
  7. ReportModel
  8. TasksModel
  9. TotalReportModel
  10. EvaluationListsModel
  11. EvaluationUsersModel
  12. DeptObjectiveModel

#### 2.2. Repository 패턴 구현
- **작업 내용**:
  - IRepository 인터페이스 정의
  - Dapper 기반 Repository 구현
  - CRUD 메서드 작성
  - 트랜잭션 처리 구현

- **Repository 목록**:
  - UserRepository
  - EDepartmentRepository
  - MemberRepository
  - AgreementRepository
  - SubAgreementRepository
  - ProcessRepository
  - ReportRepository
  - TasksRepository
  - TotalReportRepository
  - EvaluationListsRepository
  - EvaluationUsersRepository
  - DeptObjectiveRepository

#### 2.3. 뷰 모델(View Model) 작성
- **작업 내용**:
  - 뷰(View) 조회용 모델 작성
  - 복잡한 조인 쿼리 결과 매핑
  - 통계 데이터 모델

- **뷰 모델 목록**:
  - v_MemberListViewModel
  - v_DeptObjectiveListViewModel
  - v_ProcessTRListViewModel
  - v_ReportTaskListViewModel
  - v_TotalReportListViewModel

#### 2.4. 연결 문자열 관리
- **작업 내용**:
  - appsettings.json 구성
  - 환경별 설정 (Development, Production)
  - 연결 문자열 암호화 고려

#### 2.5. Model 테스트
- **테스트 항목**:
  - CRUD 기본 동작 확인
  - 외래키 제약조건 동작 확인
  - 트랜잭션 롤백 테스트
  - 성능 테스트

---

### Phase 3: Blazor Web App 개발 (3단계)

#### 3.1. 프로젝트 초기 설정
- **작업 내용**:
  - Blazor Web App 프로젝트 생성 (.NET 10)
  - NuGet 패키지 설치 (Dapper, 기타 필요 패키지)
  - 프로젝트 참조 설정 (Model 프로젝트)
  - 기본 레이아웃 구성

#### 3.2. 인증/인가 시스템
- **작업 내용**:
  - 로그인/로그아웃 기능
  - 사용자 권한 관리 (일반, 팀장, 임원, 관리자)
  - 세션 관리
  - 비밀번호 암호화 (VARBINARY)

#### 3.3. 공통 컴포넌트 개발
- **컴포넌트 목록**:
  - 그리드(Grid) 컴포넌트
  - 모달(Modal) 컴포넌트
  - 알림(Toast) 컴포넌트
  - 페이지네이션 컴포넌트
  - 로딩 스피너 컴포넌트
  - 검색 필터 컴포넌트

#### 3.4. 주요 페이지 개발

##### 3.4.1. 관리자 페이지
- 사용자 관리
- 부서 관리
- 평가 항목 관리
- 부서 목표 관리
- 전체 평가 현황

##### 3.4.2. 사용자 페이지
- 직무평가 합의 신청
- 세부직무평가 합의 신청
- 자가 평가 입력
- 평가 결과 조회
- 피드백 확인

##### 3.4.3. 평가자 페이지 (팀장)
- 직무평가 합의 승인
- 세부직무평가 합의 승인
- 팀원 평가 입력
- 팀 평가 현황

##### 3.4.4. 평가자 페이지 (임원)
- 최종 평가 입력
- 전체 평가 현황
- 피드백 작성

##### 3.4.5. 공통 페이지
- 대시보드
- 평가 일정 안내
- 공지사항
- FAQ

#### 3.5. 상태 관리 및 데이터 흐름
- **작업 내용**:
  - 상태 관리 패턴 구현
  - 서비스 계층 구성
  - 의존성 주입(DI) 설정
  - 데이터 캐싱 전략

#### 3.6. UI/UX 구현
- **작업 내용**:
  - 2025년 프로그램 UI 구조 참조
  - 반응형 디자인 적용
  - CSS 스타일 구성
  - 접근성 고려

#### 3.7. 기능 테스트
- **테스트 시나리오**:
  - 평가 프로세스 전체 흐름
  - 권한별 접근 제어
  - 데이터 유효성 검증
  - 동시성 처리
  - 에러 핸들링

---

## 4. 단계별 체크리스트

### Phase 1: Database (1단계)
- [ ] 2025년 DB 구조 분석 완료
- [ ] ERD 작성 완료
- [ ] 테이블 관계 정의 완료
- [ ] 기본 테이블 생성 스크립트 작성
- [ ] 연관 테이블 생성 스크립트 작성
- [ ] 외래키 제약조건 적용
- [ ] 뷰(View) 생성 스크립트 작성
- [ ] 초기 데이터(Seed) 스크립트 작성
- [ ] DB 생성 및 테스트 완료
- [ ] Phase 1 문서화 완료

### Phase 2: Model (2단계)
- [ ] Entity 모델 12개 작성 완료
- [ ] DTO 클래스 작성 완료
- [ ] IRepository 인터페이스 정의
- [ ] Repository 구현 12개 완료
- [ ] 뷰 모델 5개 작성 완료
- [ ] 연결 문자열 설정 완료
- [ ] CRUD 테스트 완료
- [ ] 트랜잭션 처리 테스트 완료
- [ ] Phase 2 문서화 완료

### Phase 3: Blazor App (3단계)
- [ ] Blazor 프로젝트 생성
- [ ] 인증/인가 시스템 구현
- [ ] 공통 컴포넌트 6개 이상 개발
- [ ] 관리자 페이지 개발
- [ ] 사용자 페이지 개발
- [ ] 팀장 페이지 개발
- [ ] 임원 페이지 개발
- [ ] 공통 페이지 개발
- [ ] 전체 기능 테스트 완료
- [ ] UI/UX 최종 검토
- [ ] Phase 3 문서화 완료

---

## 5. 개발 원칙

### 5.1. 작업 규칙 (절대 준수)
1. **작업지시서 우선 작성**
   - 모든 작업은 작업지시서 작성 후 진행
   - works/tasks 폴더에 저장
   - 명명규칙: `YYYYMMDD_순번_작업명.md`

2. **개발자 승인 필수**
   - 작업지시서 검토 및 승인 후 작업 시작
   - 임의 수정 금지

3. **단계별 완료 후 진행**
   - Phase 1 완료 → Phase 2 시작
   - Phase 2 완료 → Phase 3 시작

### 5.2. 코드 작성 원칙
- **명명 규칙**: 2025년 프로젝트와 일관성 유지
- **주석**: 중요 로직에 한글 주석 추가
- **에러 처리**: Try-Catch 적절히 사용
- **로깅**: 중요 이벤트 로깅 구현

### 5.3. 테스트 원칙
- 개발자가 직접 테스트 진행
- Claude AI는 테스트 항목만 제공
- 단위 테스트 항목 문서화

---

## 6. 참고 문서

### 6.1. 2025년 프로젝트
- **경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps`
- **참고 대상**:
  - Database 폴더: 테이블 구조, 뷰 구조
  - Models 폴더: 모델 작성 방법
  - BlazorApp 폴더: UI 구조, 컴포넌트 구조

### 6.2. 도서관 프로젝트
- **경로**: `C:\Codes\00_Develop_Cursor\01_MdcLibrary`
- **참고 대상**:
  - Database/dbo 폴더: 외래키 구조
  - Database_config.md: ERD 참조

---

## 7. 예상 일정 (참고용)

### Phase 1: Database
- DB 분석 및 설계
- 스크립트 작성
- 테스트 및 검증

### Phase 2: Model
- 모델 클래스 작성
- Repository 구현
- 테스트 및 검증

### Phase 3: Blazor App
- 프로젝트 초기 설정
- 공통 컴포넌트 개발
- 페이지별 기능 개발
- 통합 테스트

---

## 8. 주요 고려사항

### 8.1. DB 외래키 도입 효과
- **장점**:
  - 데이터 무결성 보장
  - 중복 데이터 제거
  - 유지보수 용이

- **주의사항**:
  - 삭제 시 참조 무결성 체크 필요
  - CASCADE 옵션 신중히 설정
  - 성능 영향 모니터링

### 8.2. Dapper 사용 시 고려사항
- SQL 인젝션 방지 (파라미터 사용)
- 복잡한 조인은 뷰(View) 활용
- 트랜잭션 처리 명확히 구현

### 8.3. Blazor 개발 시 고려사항
- 서버 vs 클라이언트 렌더링 선택
- 상태 관리 패턴 일관성 유지
- 컴포넌트 재사용성 고려

### 8.4. 배포 고려사항
- Docker 이미지 최적화
- 환경변수 설정
- 데이터베이스 마이그레이션 전략
- 로깅 및 모니터링

---

## 9. 향후 확장 가능성

### 9.1. 기능 확장
- 다면 평가 기능
- 평가 통계 및 리포트
- 알림톡 연동
- 엑셀 일괄 업로드/다운로드
- API 제공

### 9.2. 성능 최적화
- 캐싱 전략
- 데이터베이스 인덱싱
- 쿼리 최적화
- 프론트엔드 최적화

### 9.3. 보안 강화
- 2FA 인증
- 비밀번호 정책 강화
- 감사 로그
- HTTPS 강제

---

## 10. 문의 및 이슈

- **작업지시서 위치**: `works/tasks/`
- **이슈 관리**: `works/issues/`
- **이슈 인덱스**: `works/issue.md`

---

## 11. 문서 이력

| 버전 | 날짜 | 작성자 | 변경 내용 |
|------|------|--------|----------|
| 1.0 | 2025-12-16 | Claude AI | 초안 작성 |

---

## 12. 다음 단계

### 즉시 진행 가능한 작업
1. **Phase 1 시작 준비**
   - 2025년 DB 전체 테이블 분석 작업지시서 작성
   - ERD 작성 작업지시서 작성
   - 개발자 검토 및 승인

### 개발자 확인 필요 사항
- [ ] 로드맵 전체 내용 검토
- [ ] 누락된 요구사항 확인
- [ ] Phase 1 시작 승인
- [ ] App 폴더 생성 완료

---

**작성 완료**: 2025-12-16
**검토 대기 중**
