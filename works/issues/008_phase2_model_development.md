# 이슈 #008: Phase 2 - Model 개발 (Dapper)

**날짜**: 2026-01-16
**상태**: 진행중
**우선순위**: 높음
**관련 이슈**: [#004](004_phase1_database_design.md), [#007](007_remove_memberdb_optimize_structure.md)

---

## 개발자 요청

**배경**:
- Phase 1 완료: 데이터베이스 설계 및 구축 완료
- 현재 상태: 12개 테이블, 6개 뷰 구축 완료
- 다음 단계: Dapper 기반 Model 계층 개발 필요

**요청 사항**:
1. 2025년 인사평가 프로젝트의 Model 구조 참조
2. 도서관리 프로젝트의 최신 기술 및 패턴 적용
3. Dapper를 사용한 고성능 데이터 액세스 계층 구축
4. Repository 패턴 및 의존성 주입 적용

---

## 해결 방안

### 1. 참조 프로젝트 분석

#### 2025년 인사평가 프로젝트 (비즈니스 로직)
- **경로**: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models`
- **분석 완료**: ✅
- **주요 참조 사항**:
  - 도메인별 폴더 구조
  - Repository 패턴
  - 비동기 메서드 (async/await)
  - SQL Server 보안 함수 활용

#### 도서관리 프로젝트 (최신 기술)
- **경로**: `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Model`
- **분석 완료**: ✅
- **주요 참조 사항**:
  - SHA-256 + Salt 비밀번호 보안
  - 감사 추적 (CreatedDate, ModifiedDate)
  - 복잡한 검색 메서드
  - View 활용 패턴
  - .NET 8.0 최신 기능

### 2. 프로젝트 구조 설계 (확정)

```
MdcHR26Apps.Models/
├── Common/                    # 공통 모델
├── Department/                # 부서 마스터 (EDepartmentDb)
├── Rank/                      # 직급 마스터 (ERankDb)
├── User/                      # 사용자 관리 (UserDb - EStatus BIT 포함)
├── DeptObjective/             # 부서 목표 (DeptObjectiveDb)
├── EvaluationAgreement/       # 직무평가 협의서 (AgreementDb)
├── EvaluationSubAgreement/    # 상세 직무평가 협의서 (SubAgreementDb)
├── EvaluationProcess/         # 평가 프로세스 (ProcessDb)
├── EvaluationReport/          # 평가 보고서 (ReportDb)
├── EvaluationUsers/           # 평가자 관리 (EvaluationUsers)
├── EvaluationTasks/           # 업무 관리 (TasksDb)
├── EvaluationLists/           # 평가 항목 마스터 (EvaluationLists)
├── Result/                    # 종합 평가 결과 (TotalReportDb)
└── Views/                     # 뷰 모델 (5개)
```

**중요**:
- **EStatusDb 테이블은 존재하지 않음** - 재직 상태는 `UserDb.EStatus` (BIT) 컬럼으로 관리
- 총 **12개 테이블**, **5개 뷰** (DB 구조 분석 완료)

### 3. 기술 스택 (실제 적용)

- **프레임워크**: .NET 9.0 ✅
- **C# 버전**: C# 13 (Primary Constructors, Raw String Literals) ✅
- **ORM**: Dapper 2.1.66 (Micro ORM) ✅
- **DB 연결**: Microsoft.Data.SqlClient 5.2.2 ✅
- **보조 ORM**: Entity Framework Core 9.0.0 ✅
- **보안**: SHA-256 + Salt (C# 구현, LoginCheckAsync) ✅
- **패턴**: Repository Pattern, Dependency Injection ✅
- **신기능**: Primary Constructors, Raw String Literals ✅

### 4. 핵심 구현 사항

#### 비밀번호 보안 (개선)
```
기존 (2025): PWDENCRYPT (SQL Server 함수)
개선 (2026): SHA-256 + Salt (C# 구현)
- Salt: 16바이트 (128비트)
- 해시: SHA-256 (256비트)
- 저장: UserPassword (VARBINARY(128)), UserPasswordSalt (VARBINARY(16))
```

#### Repository 패턴
```
각 Entity별:
1. Entity 클래스 (UserDb.cs)
2. Repository Interface (IUserRepository.cs)
3. Repository 구현 (UserRepository.cs)

표준 메서드:
- AddAsync() - 추가
- GetByAllAsync() - 전체 조회
- GetByIdAsync() - 단일 조회
- UpdateAsync() - 수정
- DeleteAsync() - 삭제
+ 비즈니스 특화 메서드
```

---

## 진행 사항

### 분석 단계
- [x] 현재 프로젝트 DB 구조 분석
- [x] 2025년 Model 프로젝트 분석
- [x] 도서관리 Model 프로젝트 분석
- [x] 작업지시서 작성

### 개발 단계 (Phase 2-1) ✅ 완료
- [x] MdcHR26Apps.Models 프로젝트 생성 (.NET 9.0)
- [x] NuGet 패키지 설치 (Dapper 2.1.66, EF Core 9.0.0, SqlClient 5.2.2)
- [x] Common 모듈 작성 (SelectListModel)
- [x] DbContext 작성 (3개 테이블 활성화, 9개 주석 처리)
- [x] DI Extensions 작성 (3개 Repository 등록)
- [x] UserDb 완전 구현 (EStatus BIT, 18개 메서드, SHA-256 로그인)
- [x] EDepartmentDb, ERankDb 구현 (각 7개 메서드)
- [x] 빌드 성공 (오류 0개, 7.587초)

### 개발 단계 (Phase 2-2)
- [ ] ProcessDb 구현 (평가 프로세스)
- [ ] ReportDb 구현 (개별 평가)
- [ ] TotalReportDb 구현 (종합 평가)
- [ ] EvaluationUsers 구현 (평가자 관리)

### 개발 단계 (Phase 2-3)
- [ ] DeptObjectiveDb 구현 (부서 목표)
- [ ] AgreementDb, SubAgreementDb 구현 (직무평가 협의)
- [ ] TasksDb 구현 (업무 관리)
- [ ] EvaluationLists 구현 (평가 항목 마스터)

### 개발 단계 (Phase 2-4)
- [ ] v_MemberListDB 구현 (재직자 목록 - EStatus=1 필터링)
- [ ] 나머지 4개 View 모델 구현

### 테스트 단계
- [ ] 단위 테스트 작성
- [ ] 통합 테스트 실행
- [ ] 성능 테스트 실행
- [ ] 보안 테스트 (로그인)

---

## 개발 대상 목록

### Entity 클래스 (12개 테이블)

| 번호 | Entity | 파일 | Repository | Interface | 비고 |
|------|--------|------|-----------|-----------|------|
| 1 | EDepartmentDb | Department/EDepartmentDb.cs | EDepartmentRepository.cs | IEDepartmentRepository.cs | 부서 마스터 |
| 2 | ERankDb | Rank/ERankDb.cs | ERankRepository.cs | IERankRepository.cs | 직급 마스터 |
| 3 | UserDb | User/UserDb.cs | UserRepository.cs | IUserRepository.cs | **EStatus(BIT)** 포함 |
| 4 | DeptObjectiveDb | DeptObjective/DeptObjectiveDb.cs | DeptObjectiveRepository.cs | IDeptObjectiveRepository.cs | 부서 목표 |
| 5 | AgreementDb | EvaluationAgreement/AgreementDb.cs | AgreementRepository.cs | IAgreementRepository.cs | 직무평가 협의서 |
| 6 | SubAgreementDb | EvaluationSubAgreement/SubAgreementDb.cs | SubAgreementRepository.cs | ISubAgreementRepository.cs | 상세 직무평가 협의서 |
| 7 | ProcessDb | EvaluationProcess/ProcessDb.cs | ProcessRepository.cs | IProcessRepository.cs | 평가 프로세스 |
| 8 | ReportDb | EvaluationReport/ReportDb.cs | ReportRepository.cs | IReportRepository.cs | 평가 보고서 |
| 9 | TotalReportDb | Result/TotalReportDb.cs | TotalReportRepository.cs | ITotalReportRepository.cs | 종합 보고서 |
| 10 | EvaluationUsers | EvaluationUsers/EvaluationUsers.cs | EvaluationUsersRepository.cs | IEvaluationUsersRepository.cs | 평가자 관리 |
| 11 | TasksDb | EvaluationTasks/TasksDb.cs | TasksRepository.cs | ITasksRepository.cs | 업무 관리 |
| 12 | EvaluationLists | EvaluationLists/EvaluationLists.cs | EvaluationListsRepository.cs | IEvaluationListsRepository.cs | 평가 항목 마스터 |

### View 모델 (5개 뷰)

| 번호 | View Entity | 파일 | Repository | Interface | 비고 |
|------|------------|------|-----------|-----------|------|
| 1 | v_MemberListDB | Views/v_MemberListDB.cs | v_MemberListRepository.cs | Iv_MemberListRepository.cs | 재직자 목록 (EStatus=1) |
| 2 | v_DeptObjectiveListDb | Views/v_DeptObjectiveListDb.cs | v_DeptObjectiveListRepository.cs | Iv_DeptObjectiveListRepository.cs | 부서 목표 목록 |
| 3 | v_ProcessTRListDB | Views/v_ProcessTRListDB.cs | v_ProcessTRListRepository.cs | Iv_ProcessTRListRepository.cs | 프로세스 & 종합 보고서 |
| 4 | v_ReportTaskListDB | Views/v_ReportTaskListDB.cs | v_ReportTaskListRepository.cs | Iv_ReportTaskListRepository.cs | 평가 보고서 & 업무 |
| 5 | v_TotalReportListDB | Views/v_TotalReportListDB.cs | v_TotalReportListRepository.cs | Iv_TotalReportListRepository.cs | 종합 보고서 목록 |

### 공통 파일

| 파일 | 용도 |
|------|------|
| Common/SelectListModel.cs | 드롭다운 목록용 공통 모델 |
| MdcHR26AppsAddDbContext.cs | EF Core DbContext |
| MdcHR26AppsAddExtensions.cs | DI 컨테이너 설정 |
| MdcHR26Apps.Models.csproj | 프로젝트 파일 |

**총 개발 파일 수**: 54개
- Entity: 12개 (테이블)
- Repository 구현: 17개 (12 테이블 + 5 뷰)
- Interface: 17개 (12 테이블 + 5 뷰)
- View Entity: 5개
- 공통: 3개 (SelectListModel, DbContext, Extensions)

---

## 개발 우선순위

### 🔴 High Priority (Phase 2-1)
1. 프로젝트 설정 및 공통 모듈
2. UserDb (참조 모델, EStatus BIT 포함)
3. EDepartmentDb, ERankDb (마스터 데이터)

### 🟡 Medium Priority (Phase 2-2)
4. ProcessDb (평가 프로세스)
5. ReportDb (평가 보고서)
6. TotalReportDb (종합 결과)
7. EvaluationUsers (평가자 관리)

### 🟢 Low Priority (Phase 2-3, 2-4)
8. DeptObjectiveDb (부서 목표)
9. AgreementDb, SubAgreementDb (직무평가 협의)
10. TasksDb (업무 관리)
11. EvaluationLists (평가 항목 마스터)
12. View 모델 5개

---

## 기술적 개선 사항

### 2025년 프로젝트 대비 개선

| 항목 | 2025년 | 2026년 (현재) |
|------|--------|--------------|
| **.NET 버전** | .NET 7.0 | **.NET 10.0** ⭐ |
| **C# 버전** | C# 11 | **C# 13** ⭐ |
| **EF Core** | 7.0.16 | **10.0.0** ⭐ |
| **Dapper** | 2.0.123 | 2.1.66 |
| **비밀번호 보안** | PWDENCRYPT (SQL) | SHA-256 + Salt (C#) |
| **테이블 구조** | 13개 (MemberDb 포함) | 12개 (MemberDb 제거) |
| **권한 관리** | 3개 플래그 | 4개 플래그 (+IsDeptObjectiveWriter) |
| **감사 추적** | 미적용 | CreatedDate, ModifiedDate |
| **생성자** | 전통적 생성자 | **Primary Constructor** ⭐ |
| **컬렉션** | new List<T>() | **Collection Expressions []** ⭐ |

### 도서관리 프로젝트에서 적용

1. ✅ **SHA-256 + Salt**: 비밀번호 보안 강화
2. ✅ **감사 추적**: CreatedDate, ModifiedDate 필드
3. ✅ **복잡한 검색**: SearchBy, GetBy 패턴
4. ✅ **View 활용**: 복잡한 조인을 View로 처리
5. ✅ **비즈니스 메서드**: 도메인 로직 캡슐화
6. ⭐ **.NET 10.0**: 최신 프레임워크 (.NET 8.0 → 10.0 업그레이드)

### .NET 10 신기능 활용

1. ⭐ **Primary Constructors**: 모든 Repository에 적용
   ```csharp
   public class UserRepository(string connectionString, ILoggerFactory loggerFactory)
       : IUserRepository, IDisposable
   ```

2. ⭐ **Collection Expressions**: 간결한 컬렉션 초기화
   ```csharp
   return result?.ToList() ?? [];  // 빈 리스트
   ```

3. ⭐ **Raw String Literals**: 복잡한 SQL 쿼리 가독성 향상
   ```csharp
   const string query = """
       SELECT * FROM UserDb
       WHERE EDepartId = @departId
       """;
   ```

4. ⭐ **EF Core 10 Complex Types**: 값 객체 지원 강화

5. ⭐ **성능 최적화**: JIT, GC, LINQ 자동 성능 향상

---

## 테스트 계획

### 단위 테스트
```csharp
// 각 Repository별 테스트
- AddAsync_ShouldReturnEntityWithId()
- GetByAllAsync_ShouldReturnList()
- GetByIdAsync_ShouldReturnEntity()
- UpdateAsync_ShouldReturnTrue()
- DeleteAsync_ShouldReturnTrue()
```

### 통합 테스트
```csharp
// CRUD 전체 사이클 테스트
- FullCRUD_Lifecycle_Test()
- ForeignKey_Relationship_Test()
```

### 보안 테스트
```csharp
// UserRepository 보안 테스트
- LoginCheck_ValidCredentials_ShouldReturnTrue()
- LoginCheck_InvalidCredentials_ShouldReturnFalse()
- PasswordHashing_ShouldBeDifferentWithSamePlaintext()
- SaltGeneration_ShouldBeUnique()
```

### 성능 테스트
```csharp
// 성능 기준
- GetByAllAsync: 1000개 레코드 < 1초
- GetByIdAsync: < 100ms
- AddAsync: < 200ms
- UpdateAsync: < 150ms
```

---

## 관련 문서

**작업지시서**:
- [20260116_01_phase2_model_development.md](../tasks/20260116_01_phase2_model_development.md) - ✅ 개발자 승인 완료 (2026-01-19)
- [20260119_01_phase2_1_project_setup.md](../tasks/20260119_01_phase2_1_project_setup.md) - ✅ 작업 완료 (2026-01-19)

**선행 이슈**:
- [#004: Phase 1 데이터베이스 설계](004_phase1_database_design.md)
- [#007: MemberDb 제거 및 최적화](007_remove_memberdb_optimize_structure.md)

**참조 프로젝트**:
- 2025년 인사평가: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.Models`
- 도서관리: `C:\Codes\36_MdcLibrary\MdcLibrary\MdcLibrary.Model`

---

## 개발자 확인 사항

### 질문 사항
1. Phase 2-1부터 단계적으로 진행할까요, 아니면 전체를 한 번에 진행할까요?
2. 단위 테스트는 각 Repository 개발 시 함께 작성할까요?
3. UserDb를 참조 모델로 먼저 완성하고 나머지를 진행할까요?
4. View 모델은 나중에 필요 시 추가하는 방식으로 할까요?

### 승인 필요 사항
1. ⭐ **.NET 10.0 사용** (최신 프레임워크)
2. ⭐ **C# 13 신기능 활용** (Primary Constructors, Collection Expressions)
3. **EF Core 10.0** 및 최신 NuGet 패키지 사용
4. **SHA-256 + Salt** 비밀번호 보안 방식 적용
5. **Repository 패턴** 및 DI 구조
6. **감사 추적** (CreatedDate, ModifiedDate) 적용
7. **총 54개 파일** 개발 계획 (12 테이블 + 5 뷰 + 공통 3개)
8. **DB 구조 확정**: EStatusDb 테이블 없음, UserDb.EStatus(BIT) 컬럼으로 관리

---

## 개발자 피드백

**작업 시작**: 2026-01-16
**작업지시서 작성**: 2026-01-16
**개발자 승인**: 2026-01-19
**Phase 2-1 완료**: 2026-01-19
**현재 상태**: 진행 중 (Phase 2-1 완료, Phase 2-2 준비)
**비고**:
- 3개 프로젝트 분석 완료 (현재, 2025년, 도서관리)
- 상세 작업지시서 작성 완료 (57페이지)
- 개발 구조 및 우선순위 설계 완료
- 코드 예시 및 테스트 계획 포함

**진행 이력**:
- 2026-01-16: Phase 2 전체 작업지시서 작성 (20260116_01)
- 2026-01-19: 개발자 승인 완료
- 2026-01-19: Phase 2-1 상세 작업지시서 작성 (20260119_01)
- 2026-01-19: Phase 2-1 프로젝트 생성 및 13개 파일 작성
- 2026-01-19: Phase 2-1 빌드 성공 (오류 0개, 7.587초)

**Phase 2-1 완료 내용**:
- ✅ MdcHR26Apps.Models 프로젝트 생성 (.NET 9.0)
- ✅ 13개 파일 작성 (Entity 3개, Repository 6개, 공통 4개)
- ✅ Primary Constructor, Raw String Literals 적용
- ✅ SHA-256 + Salt 로그인 구현
- ✅ Dapper + EF Core 하이브리드 구조

**다음 단계**:
1. ✅ Phase 2-1 개발 완료
2. ⏳ Phase 2-2 작업지시서 작성 (ProcessDb, ReportDb, TotalReportDb, EvaluationUsers)
3. Phase 2-2 개발 진행
