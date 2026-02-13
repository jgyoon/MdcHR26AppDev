# 작업지시서 재작성 가이드

**날짜**: 2026-02-03
**목적**: 컴포넌트 작업지시서 4개 재작성
**이유**: Entity 및 Repository 구조 변경으로 기존 작업지시서 05-08 폐기

---

## 1. 배경

### Entity/Repository 변경 작업 완료
- **작업지시서 11**: Entity 필드명 DB 테이블 기준 수정 (5개)
- **작업지시서 12**: Repository 25년 메서드 기준 재작성 (5개 + Interface 5개)

### 기존 작업지시서 문제점
- **작성 시점**: 2026-02-03 오전 10:34~10:49 (Entity 변경 전)
- **문제**: 잘못된 필드명, 메서드명 사용
- **영향**: 작업지시서 05, 06, 07, 08 전체

---

## 2. 주요 변경 사항 요약

### 2.1. Entity 필드명 변경

#### AgreementDb
| 작업지시서 05 (잘못됨) | 실제 Entity (현재) |
|----------------------|------------------|
| `Item_Number` | `Report_Item_Number` |
| `Item_Title` | `Report_Item_Name_1` |
| `Item_Contents` | `Report_Item_Name_2` |
| `Item_Proportion` | `Report_Item_Proportion` |

#### SubAgreementDb
| 작업지시서 06 (확인 필요) | 실제 Entity (현재) |
|------------------------|------------------|
| PK: `SAid` (추정) | PK: `Sid` |
| (필드명 확인 필요) | `Report_Item_Number`, `Report_Item_Name_1`, `Report_Item_Name_2` |
| (필드명 확인 필요) | `Report_SubItem_Number`, `Report_SubItem_Name` |
| (필드명 확인 필요) | `Task_Number` |

#### EvaluationLists
| 작업지시서 08 (확인 필요) | 실제 Entity (현재) |
|------------------------|------------------|
| PK: `ELid` (추정) | PK: `Eid` |
| (필드명 확인 필요) | `Evaluation_Department_Number`, `Evaluation_Department_Name` |
| (필드명 확인 필요) | `Evaluation_Index_Number`, `Evaluation_Index_Name` |
| (필드명 확인 필요) | `Evaluation_Task_Number`, `Evaluation_Task_Name` |

#### DeptObjectiveDb
| 작업지시서 (신규) | 실제 Entity (현재) |
|------------------|------------------|
| - | PK: `DeptObjectiveDbId` |
| - | 감사 필드: `CreatedBy`, `CreatedAt`, `UpdatedBy`, `UpdatedAt` |

#### TasksDb
| 작업지시서 (확인 필요) | 실제 Entity (현재) |
|---------------------|------------------|
| (필드명 확인 필요) | `TaskName`, `TaksListNumber` (오타 그대로), `TaskStatus`, `TaskObjective` |

---

### 2.2. Repository 메서드 변경

#### 25년 메서드 기준으로 재작성
| Repository | 25년 메서드 | 26년 메서드 (기존) | 최종 메서드 | 변경 사항 |
|-----------|------------|------------------|-----------|---------|
| AgreementRepository | 7개 | 11개 | 7개 | GetCountByUidAsync 등 4개 **제거됨** |
| SubAgreementRepository | 7개 | 12개 | 7개 | GetCountByUidAsync 등 5개 **제거됨** |
| DeptObjectiveRepository | - | 10개 | 5개 | 26년 신규, 기본 CRUD만 |
| EvaluationListsRepository | 9개 | 8개 | 9개 | SelectListModel 메서드 포함 |
| TasksRepository | 7개 | 10개 | 9개 | 25년 7개 + 26년 요구 2개 |

**주의**: 작업지시서에서 제거된 메서드를 사용하지 말 것!

---

### 2.3. 주요 API 변경

#### Repository 메서드 호출 패턴
```csharp
// ❌ 작업지시서 05-08에 있을 수 있는 잘못된 코드
var count = await agreementRepository.GetCountByUidAsync(userId);
await agreementRepository.DeleteAllByUidAsync(userId);

// ✅ 현재 정확한 코드 (25년 메서드 패턴)
var agreements = await agreementRepository.GetByUserIdAllAsync(uid);
var count = agreements.Count;

var agreements = await agreementRepository.GetByUserIdAllAsync(uid);
foreach (var agreement in agreements)
{
    await agreementRepository.DeleteAsync(agreement.Aid);
}
```

#### SelectListModel 속성
```csharp
// ❌ 작업지시서에 있을 수 있는 잘못된 코드
new SelectListModel
{
    SelectListNumber = item.Number.ToString(),
    SelectListName = item.Name
}

// ✅ 현재 정확한 코드
new SelectListModel
{
    Value = item.Number.ToString(),
    Text = item.Name
}
```

#### 파라미터 타입
```csharp
// ❌ 작업지시서에 있을 수 있는 잘못된 코드
public async Task LoadData(string userId)

// ✅ 현재 정확한 코드
public async Task LoadData(long uid)
```

---

## 3. 재작성할 작업지시서 목록

### 폐기 대상 (Entity 변경 전 작성)
- ❌ 20260203_05_components_agreement.md
- ❌ 20260203_06_components_subagreement.md
- ❌ 20260203_07_components_report.md
- ❌ 20260203_08_components_common_form.md

### 재작성 대상 (신규 번호)
- 🔄 20260203_13_components_agreement_v2.md
- 🔄 20260203_14_components_subagreement_v2.md
- 🔄 20260203_15_components_report_v2.md
- 🔄 20260203_16_components_common_form_v2.md

---

## 4. 재작성 절차

### 4.1. 사전 준비
1. **Entity 확인**:
   ```
   "AgreementDb, SubAgreementDb, TasksDb, EvaluationLists, DeptObjectiveDb
    Entity 파일 읽고 필드 구조 정리해줘"
   ```

2. **Repository 확인**:
   ```
   "AgreementRepository, SubAgreementRepository 인터페이스 읽고
    메서드 목록 정리해줘"
   ```

3. **2025년 프로젝트 참조**:
   - 경로: `C:\Codes\29_MdcHR25\MdcHR25Apps\MdcHR25Apps.BlazorApp\Components`
   - 컴포넌트 구조, 로직 패턴 참조

---

### 4.2. 작업지시서 작성 요청

#### Agreement 컴포넌트 (6개)
```
"20260203_13_components_agreement_v2.md 작업지시서 작성해줘.
- 현재 AgreementDb Entity 구조 반영
- 25년 Repository 메서드 패턴 사용
- 2025년 프로젝트 컴포넌트 참조
- 간소화된 템플릿 (TEMPLATE_simple.md) 사용
- 6개 컴포넌트: AgreementDbListTable, AgreementDetailsTable, AgreementListTable,
  AgreementDbListView, AgreementDeleteModal, AgreementComment"
```

#### SubAgreement 컴포넌트 (8개)
```
"20260203_14_components_subagreement_v2.md 작업지시서 작성해줘.
- 현재 SubAgreementDb Entity 구조 반영
- 25년 Repository 메서드 패턴 사용
- 8개 컴포넌트: SubAgreementDbListTable, SubAgreementDetailsTable, SubAgreementListTable,
  SubAgreementResetList, SubAgreementDbListView, SubAgreementDeleteModal,
  AgreeItemLists, ReportTaskListCommonView"
```

#### Report 컴포넌트 (17개)
```
"20260203_15_components_report_v2.md 작업지시서 작성해줘.
- 현재 ReportDb Entity 구조 반영
- 25년 Repository 메서드 패턴 사용
- 17개 컴포넌트 (목록은 2025년 프로젝트 참조)"
```

#### Common/Form 컴포넌트 (9개)
```
"20260203_16_components_common_form_v2.md 작업지시서 작성해줘.
- SelectListModel Value/Text 속성 사용
- EvaluationLists, TasksDb, DeptObjectiveDb Entity 구조 반영
- 9개 컴포넌트: CheckboxComponent, FormAgreeTask, FormAgreeTaskCreate, FormGroup,
  FormSelectList, FormSelectNumber, FormTaskItem, ObjectiveListTable, EDeptListTable"
```

---

### 4.3. 작업지시서 검증

각 작업지시서 작성 후 확인 사항:
- [ ] Entity 필드명이 현재 구조와 일치하는가?
- [ ] Repository 메서드가 25년 패턴(제거된 메서드 없음)인가?
- [ ] SelectListModel 속성이 Value/Text인가?
- [ ] 파라미터 타입이 `long uid`인가 (string userId 아님)?
- [ ] 2025년 프로젝트 컴포넌트를 참조했는가?

---

## 5. 참조 문서

### Entity 구조
- [20260203_11_fix_entity_db_field_names.md](20260203_11_fix_entity_db_field_names.md)

### Repository 구조
- [20260203_12_fix_repository_based_on_2025.md](20260203_12_fix_repository_based_on_2025.md)

### 이슈 문서
- [009_phase3_webapp_development.md](../issues/009_phase3_webapp_development.md)

### 템플릿
- [TEMPLATE_simple.md](TEMPLATE_simple.md) - 간소화된 템플릿 (권장)
- [TEMPLATE_detailed.md](TEMPLATE_detailed.md) - 상세 템플릿

---

## 6. 새 세션 시작 명령어 (퇴근 후)

### 빠른 시작 (권장)
```
"작업지시서 재작성 가이드(20260203_13_REWRITE_GUIDE.md) 읽고
Agreement 컴포넌트 작업지시서(13번)부터 작성해줘"
```

### 확인 후 시작
```
"작업지시서 재작성 가이드 읽고 현재 상황 요약해줘.
그리고 어떤 순서로 작업지시서를 재작성할지 알려줘."
```

---

## 7. 작업 순서 (권장)

```
Step 1: Entity 및 Repository 구조 확인
   └─ AgreementDb, SubAgreementDb, TasksDb, EvaluationLists, DeptObjectiveDb

Step 2: Agreement 작업지시서 재작성 (13번)
   └─ 6개 컴포넌트

Step 3: SubAgreement 작업지시서 재작성 (14번)
   └─ 8개 컴포넌트

Step 4: Report 작업지시서 재작성 (15번)
   └─ 17개 컴포넌트

Step 5: Common/Form 작업지시서 재작성 (16번)
   └─ 9개 컴포넌트

Step 6: 작업지시서 검증
   └─ Entity 필드명, Repository 메서드, SelectListModel 확인

Step 7: 구현 시작
   └─ "/execute 20260203_13"
```

---

## 8. 주의사항

### 절대 하지 말 것
- ❌ 기존 작업지시서 05-08 사용 금지
- ❌ 제거된 Repository 메서드(GetCountByUidAsync 등) 사용 금지
- ❌ SelectListNumber/SelectListName 속성 사용 금지
- ❌ string userId 파라미터 사용 금지

### 반드시 확인할 것
- ✅ 현재 Entity 필드명 사용
- ✅ 25년 Repository 메서드 패턴 사용
- ✅ SelectListModel Value/Text 속성 사용
- ✅ long uid 파라미터 사용
- ✅ 2025년 프로젝트 컴포넌트 참조

---

**작성일**: 2026-02-03 18:30
**담당**: Claude Sonnet 4.5
**관련 이슈**: [#009](../issues/009_phase3_webapp_development.md)
