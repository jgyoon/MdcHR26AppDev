# 작업지시서: 알림톡 발송 시 검진 모드 시간 반영

**날짜**: 2025-11-10
**파일**: `public/message-list.html`
**작업 유형**: 버그 수정
**관련 이슈**: [#001: 알림톡 발송 시 검진 모드 시간 반영](../issues/001_checkup_mode_alimtalk_fix.md)
**관련 작업지시서**:
- `20250107_01_checkup_time_adjustment.md`
- `20250107_02_checkup_button_visual_feedback.md`

---

## 1. 문제 상황

### 1.1. 증상
- 검진 버튼을 클릭하여 시간 조정 후 문자 전송 시:
  - **LMS/SMS**: 조정된 시간(11:30)으로 정상 전송 ✅
  - **알림톡**: 원본 시간(11:50)으로 전송됨 ❌

### 1.2. 테스트 결과
- **예약 시간**: 11:50
- **검진 버튼 클릭**: 11:30으로 조정
- **전송 화면**: "11:30에 예약 완료되었습니다." 표시 (정상)
- **실제 수신**: "11:50에 예약 완료되었습니다." 수신 (오류)

### 1.3. 서버 로그 분석
```json
"mergeData": {"name":"윤종국", "time":"11:50"}  // ← 원본 시간 11:50
```

알림톡 발송 시 `mergeData`에 **조정된 시간이 아닌 원본 시간**이 전달되고 있음.

---

## 2. 원인 분석

### 2.1. 정상 작동하는 경우 (LMS/SMS)

**호출 흐름**:
```
sendMessage()
  → sendSmsOrLms()
    → messageToSend 사용 (이미 조정된 시간 포함)
```

**코드 위치**: 1241-1297라인 (`sendSmsOrLms()` 메서드)
```javascript
const messageData = {
    phoneNumber: this.patientInfo.phoneNumber,
    message: this.messageToSend,  // ← selectTemplate()에서 이미 조정됨
    // ...
};
```

- `this.messageToSend`는 `selectTemplate()` 메서드(913-965라인)에서 생성
- `selectTemplate()` 내부에서 `isCheckupMode` 확인하여 시간 조정:
  ```javascript
  let displayTime = this.reservationInfo.timeStart;
  if (this.isCheckupMode) {
      displayTime = this.adjustTimeForCheckup(this.reservationInfo.timeStart);
  }
  messageContent = messageContent.replace(/#{time}/g, displayTime || '');
  ```

### 2.2. 문제가 발생하는 경우 (알림톡)

**호출 흐름**:
```
sendMessage()
  → extractMergeData() 호출 (1078라인)
    → sendAlimtalk()
      → mergeData 전송 (원본 시간 포함)
```

**문제 코드 위치**: 1377-1418라인 (`extractMergeData()` 메서드)
```javascript
else if (variableName === '예약시간' && this.reservationInfo) {
    mergeData[variableName] = this.reservationInfo.timeStart || '';  // ← 항상 원본
}
// ...
else if (variableName === 'time' && this.reservationInfo) {
    mergeData[variableName] = this.reservationInfo.timeStart || '';  // ← 항상 원본
}
```

**문제점**:
- `extractMergeData()`가 `isCheckupMode`를 확인하지 않음
- 항상 `this.reservationInfo.timeStart` (원본 시간) 사용
- 검진 모드가 활성화되어도 조정된 시간이 반영되지 않음

---

## 3. 수정 내용

### 3.1. extractMergeData() 메서드 수정

**위치**: 1377-1418라인

**현재 코드**:
```javascript
// mergeData 생성을 위한 함수 추가
extractMergeData(content) {
    // #{변수명} 패턴을 찾는 정규식
    const variableRegex = /#{([^}]+)}/g;
    const mergeData = {};
    let match;

    // 모든 변수를 찾아 mergeData 객체에 추가
    while ((match = variableRegex.exec(content)) !== null) {
        const variableName = match[1]; // 변수명만 추출 (# 및 {} 제외)

        // 예약 정보에서 해당 변수에 맞는 값 설정
        if (variableName === '환자명' && this.patientInfo) {
            mergeData[variableName] = this.patientInfo.patientName || '';
        }
        else if (variableName === '예약일' && this.reservationInfo) {
            // mergeData[variableName] = this.reservationInfo.date || '';
            mergeData[variableName] = this.formatDateWithDay(this.reservationInfo.date) || '';
        }
        else if (variableName === '예약시간' && this.reservationInfo) {
            mergeData[variableName] = this.reservationInfo.timeStart || '';
        }
        else if (variableName === '진료과' && this.reservationInfo) {
            mergeData[variableName] = this.getDeptName(this.reservationInfo.department) || '';
        }
        else if (variableName === '의사명' && this.reservationInfo) {
            mergeData[variableName] = this.reservationInfo.doctorName || '';
        }
        else if (variableName === 'name' && this.patientInfo) {
            mergeData[variableName] = this.patientInfo.patientName || '';
        }
        else if (variableName === 'time' && this.reservationInfo) {
            mergeData[variableName] = this.reservationInfo.timeStart || '';
        }
        else {
            // 기타 변수는 빈 값으로 설정
            mergeData[variableName] = '';
        }
    }

    return mergeData;
},
```

**수정 후**:
```javascript
// mergeData 생성을 위한 함수 추가
extractMergeData(content) {
    // #{변수명} 패턴을 찾는 정규식
    const variableRegex = /#{([^}]+)}/g;
    const mergeData = {};
    let match;

    // 모든 변수를 찾아 mergeData 객체에 추가
    while ((match = variableRegex.exec(content)) !== null) {
        const variableName = match[1]; // 변수명만 추출 (# 및 {} 제외)

        // 예약 정보에서 해당 변수에 맞는 값 설정
        if (variableName === '환자명' && this.patientInfo) {
            mergeData[variableName] = this.patientInfo.patientName || '';
        }
        else if (variableName === '예약일' && this.reservationInfo) {
            // mergeData[variableName] = this.reservationInfo.date || '';
            mergeData[variableName] = this.formatDateWithDay(this.reservationInfo.date) || '';
        }
        else if (variableName === '예약시간' && this.reservationInfo) {
            // 검진 모드일 경우 시간 조정
            let timeValue = this.reservationInfo.timeStart;
            if (this.isCheckupMode) {
                timeValue = this.adjustTimeForCheckup(this.reservationInfo.timeStart);
            }
            mergeData[variableName] = timeValue || '';
        }
        else if (variableName === '진료과' && this.reservationInfo) {
            mergeData[variableName] = this.getDeptName(this.reservationInfo.department) || '';
        }
        else if (variableName === '의사명' && this.reservationInfo) {
            mergeData[variableName] = this.reservationInfo.doctorName || '';
        }
        else if (variableName === 'name' && this.patientInfo) {
            mergeData[variableName] = this.patientInfo.patientName || '';
        }
        else if (variableName === 'time' && this.reservationInfo) {
            // 검진 모드일 경우 시간 조정
            let timeValue = this.reservationInfo.timeStart;
            if (this.isCheckupMode) {
                timeValue = this.adjustTimeForCheckup(this.reservationInfo.timeStart);
            }
            mergeData[variableName] = timeValue || '';
        }
        else {
            // 기타 변수는 빈 값으로 설정
            mergeData[variableName] = '';
        }
    }

    return mergeData;
},
```

### 3.2. 수정 요약

**변경 대상**: 두 군데

1. **'예약시간' 변수 처리** (1396-1398라인):
   ```javascript
   // 기존
   mergeData[variableName] = this.reservationInfo.timeStart || '';

   // 수정 후
   let timeValue = this.reservationInfo.timeStart;
   if (this.isCheckupMode) {
       timeValue = this.adjustTimeForCheckup(this.reservationInfo.timeStart);
   }
   mergeData[variableName] = timeValue || '';
   ```

2. **'time' 변수 처리** (1408-1410라인):
   ```javascript
   // 기존
   mergeData[variableName] = this.reservationInfo.timeStart || '';

   // 수정 후
   let timeValue = this.reservationInfo.timeStart;
   if (this.isCheckupMode) {
       timeValue = this.adjustTimeForCheckup(this.reservationInfo.timeStart);
   }
   mergeData[variableName] = timeValue || '';
   ```

---

## 4. 수정 로직 설명

### 4.1. 검진 모드 확인
- `this.isCheckupMode`가 `true`일 때만 시간 조정
- `selectTemplate()` 메서드와 동일한 로직 사용

### 4.2. 시간 조정 로직
- `adjustTimeForCheckup()` 헬퍼 함수 재사용 (993-1017라인)
- 00~25분 → 00분
- 30~55분 → 30분

### 4.3. 일관성 유지
- `selectTemplate()`에서 사용하는 로직과 동일
- LMS/SMS와 알림톡이 동일한 시간으로 전송되도록 보장

---

## 5. 테스트 항목

개발자가 테스트할 항목:

### 5.1. 알림톡 검진 모드 테스트

**시나리오 1: 검진 모드 ON**
1. 11:50 예약 환자 선택
2. 알림톡 템플릿 선택 (알림톡 코드 있는 템플릿)
3. "검진" 버튼 클릭
4. 전송 화면에 "11:30" 표시 확인
5. "전송" 버튼 클릭
6. **실제 수신된 알림톡**: "11:30"으로 수신 확인 ✅

**시나리오 2: 검진 모드 OFF (기본)**
1. 11:50 예약 환자 선택
2. 알림톡 템플릿 선택
3. "검진" 버튼 클릭하지 않음
4. 전송 화면에 "11:50" 표시 확인
5. "전송" 버튼 클릭
6. **실제 수신된 알림톡**: "11:50"으로 수신 확인 ✅

**시나리오 3: 검진 모드 토글**
1. 11:50 예약 환자 선택
2. 알림톡 템플릿 선택
3. "검진" 버튼 클릭 → "11:30" 표시
4. "검진" 버튼 다시 클릭 → "11:50"으로 복원
5. "전송" 버튼 클릭
6. **실제 수신된 알림톡**: "11:50"으로 수신 확인 ✅

### 5.2. 다양한 시간대 테스트

| 원본 시간 | 검진 모드 OFF | 검진 모드 ON |
|---------|------------|------------|
| 08:10   | 08:10      | 08:00      |
| 08:30   | 08:30      | 08:30      |
| 09:45   | 09:45      | 09:30      |
| 11:50   | 11:50      | 11:30      |
| 14:25   | 14:25      | 14:00      |

### 5.3. LMS/SMS 정상 작동 확인 (회귀 테스트)
- 기존에 정상 작동하던 LMS/SMS가 여전히 정상 작동하는지 확인
- 검진 모드 ON/OFF 모두 테스트

### 5.4. 서버 로그 확인
```
알림톡 발송 페이로드: {
  "mergeData": {
    "name": "윤종국",
    "time": "11:30"  // ← 조정된 시간 확인
  }
}
```

### 5.5. 콘솔 로그 확인
- 브라우저 콘솔에서 다음 로그 확인:
  ```
  검진 모드 ON - 시간: 11:50
  추출된 mergeData: {name: "윤종국", time: "11:30"}
  ```

---

## 6. 예상 결과

### 수정 전
- **알림톡**: 항상 원본 시간 전송 (11:50)
- **LMS/SMS**: 조정된 시간 전송 (11:30)
- **불일치 발생** ❌

### 수정 후
- **알림톡**: 검진 모드에 따라 조정된 시간 전송 (11:30)
- **LMS/SMS**: 검진 모드에 따라 조정된 시간 전송 (11:30)
- **일관성 유지** ✅

---

## 7. 주의사항

1. **기존 로직 재사용**: `adjustTimeForCheckup()` 헬퍼 함수를 그대로 재사용하므로 안전
2. **일관성 보장**: `selectTemplate()`과 `extractMergeData()`에서 동일한 로직 사용
3. **회귀 테스트 필수**: LMS/SMS가 여전히 정상 작동하는지 확인
4. **검진 모드 초기화**: `closeModal()` 시 `isCheckupMode = false`로 초기화되므로 안전

---

## 8. 백업 안내

수정 전 현재 파일 백업 권장:
```
public/message-list.html → public/message-list.html.backup_20251110
```

---

## 9. 관련 코드 참조

### 9.1. selectTemplate() 메서드의 시간 조정 로직 (933-937라인)
```javascript
// 검진 모드일 경우 시간 조정
let displayTime = this.reservationInfo.timeStart;
if (this.isCheckupMode) {
    displayTime = this.adjustTimeForCheckup(this.reservationInfo.timeStart);
}
```

### 9.2. adjustTimeForCheckup() 헬퍼 함수 (993-1017라인)
```javascript
adjustTimeForCheckup(timeStr) {
    if (!timeStr) return timeStr;

    const [hours, minutes] = timeStr.split(':').map(Number);

    if (isNaN(hours) || isNaN(minutes)) {
        return timeStr;
    }

    let adjustedMinutes;
    if (minutes >= 0 && minutes <= 25) {
        adjustedMinutes = '00';
    } else if (minutes >= 30 && minutes <= 55) {
        adjustedMinutes = '30';
    } else {
        adjustedMinutes = minutes >= 30 ? '30' : '00';
    }

    const adjustedHours = String(hours).padStart(2, '0');
    return `${adjustedHours}:${adjustedMinutes}`;
}
```

---

## 10. 완료 조건

- [ ] `extractMergeData()` 메서드 수정 완료
- [ ] 알림톡 검진 모드 ON 테스트 성공
- [ ] 알림톡 검진 모드 OFF 테스트 성공
- [ ] LMS/SMS 회귀 테스트 성공
- [ ] 서버 로그에서 `mergeData.time` 조정 확인
- [ ] 실제 수신된 메시지에서 올바른 시간 확인
