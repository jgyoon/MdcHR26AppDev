# DB-Design - 데이터베이스 설계 및 수정

데이터베이스 테이블 설계, 수정, 최적화 작업을 수행하는 스킬입니다.

## 사용법

```
/db-design [작업 내용]
```

## 예시

```
/db-design UserDb에 비밀번호 Salt 필드 추가
/db-design MemberDb 제거하고 UserDb로 통합
/db-design 부서 목표 작성 권한 관리 추가
```

## 동작

1. **현재 DB 구조 분석**
   - 관련 테이블 파일 읽기 (Database/dbo/*.sql)
   - 외래키 관계 파악
   - 뷰(View) 의존성 확인

2. **변경 사항 설계**
   - 필드 추가/삭제/수정
   - 외래키 제약조건 검토
   - 인덱스 필요성 판단

3. **작업지시서 자동 생성**
   - /task 스킬 자동 호출
   - DB 특화 테스트 쿼리 포함

4. **영향도 분석**
   - 수정 대상 파일 목록
   - 01_CreateTables.sql 수정 필요 여부
   - 02_CreateViews.sql 수정 필요 여부
   - 03_SeedData.sql 수정 필요 여부

5. **검증 쿼리 자동 생성**
   - 스키마 확인 쿼리
   - 데이터 삽입 테스트
   - 외래키 동작 테스트

## 처리 가능한 작업

### 테이블 관련
- 새 테이블 생성
- 테이블 필드 추가/삭제/수정
- 외래키 추가/삭제
- 인덱스 추가
- 제약조건 추가/수정

### 뷰(View) 관련
- 새 뷰 생성
- 기존 뷰 수정
- 뷰 최적화

### 데이터 관련
- 시드 데이터 추가/수정
- 마이그레이션 스크립트 생성

## 작업 순서

1. 개별 테이블 파일 수정 (Database/dbo/*.sql)
2. 통합 생성 스크립트 수정 (Database/01_CreateTables.sql)
3. 뷰 생성 스크립트 수정 (Database/02_CreateViews.sql)
4. 시드 데이터 수정 (Database/03_SeedData.sql)

## 자동 검증 사항

- SQL 문법 확인
- 외래키 순환 참조 검사
- 필드 타입 호환성 검사
- NOT NULL 제약조건 체크
- 기본값(DEFAULT) 설정 확인

## 참고 프로젝트

필요 시 참고 프로젝트의 DB 구조 분석:
- C:\Codes\00_Develop_Cursor\01_MdcLibrary\Database
