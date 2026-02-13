# 프로젝트 동기화 검증 스크립트

$currentPath = "C:\Codes\00_Develop_Cursor\10_MdcHR26Apps"
$targetPath = "C:\Codes\41_MdcHR26\MdcHR26App"

# 검증할 파일 목록 (생성 파일)
$createdFiles = @(
    "MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/Details.razor",
    "MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/Details.razor.cs",
    "MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/Edit.razor",
    "MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/Edit.razor.cs",
    "MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/Index.razor",
    "MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/Index.razor.cs",
    "MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/ReportInit.razor",
    "MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/ReportInit.razor.cs",
    "MdcHR26Apps.BlazorServer/Components/Pages/Components/Common/AdminTaskViewExcel.razor",
    "MdcHR26Apps.BlazorServer/Components/Pages/Components/Common/AdminTaskViewExcel.razor.cs",
    "MdcHR26Apps.BlazorServer/Components/Pages/Components/Common/AdminViewExcel.razor",
    "MdcHR26Apps.BlazorServer/Components/Pages/Components/Common/AdminViewExcel.razor.cs",
    "MdcHR26Apps.BlazorServer/Components/Pages/Components/Modal/ReportInitModal.razor",
    "MdcHR26Apps.BlazorServer/Components/Pages/Components/Table/AdminReportListView.razor",
    "MdcHR26Apps.BlazorServer/Models/TotalScoreRankModel.cs",
    "MdcHR26Apps.BlazorServer/Utils/ExcelManage.cs",
    "MdcHR26Apps.BlazorServer/Utils/ScoreUtils.cs",
    "MdcHR26Apps.BlazorServer/wwwroot/files/tasks/file_tasks.html",
    "MdcHR26Apps.BlazorServer/wwwroot/js/site.js"
)

# 검증할 파일 목록 (수정 파일)
$modifiedFiles = @(
    "MdcHR26Apps.BlazorServer/Components/App.razor",
    "MdcHR26Apps.BlazorServer/Components/Pages/Admin/EvaluationUsers/Details.razor.cs",
    "MdcHR26Apps.BlazorServer/Components/Pages/Admin/EvaluationUsers/Edit.razor.cs",
    "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Index.razor",
    "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Index.razor.cs",
    "MdcHR26Apps.BlazorServer/Components/Pages/Admin/Users/Create.razor.cs",
    "MdcHR26Apps.BlazorServer/Data/UrlActions.cs",
    "MdcHR26Apps.BlazorServer/Program.cs",
    "MdcHR26Apps.Models/EvaluationAgreement/AgreementRepository.cs",
    "MdcHR26Apps.Models/EvaluationAgreement/IAgreementRepository.cs",
    "MdcHR26Apps.Models/EvaluationReport/IReportRepository.cs",
    "MdcHR26Apps.Models/EvaluationReport/ReportRepository.cs",
    "MdcHR26Apps.Models/EvaluationSubAgreement/ISubAgreementRepository.cs",
    "MdcHR26Apps.Models/EvaluationSubAgreement/SubAgreementRepository.cs",
    "MdcHR26Apps.Models/EvaluationTasks/ITasksRepository.cs",
    "MdcHR26Apps.Models/EvaluationTasks/TasksRepository.cs",
    "MdcHR26Apps.Models/Result/ITotalReportRepository.cs",
    "MdcHR26Apps.Models/Result/TotalReportRepository.cs",
    "MdcHR26Apps.Models/Views/v_DeptObjectiveListDb/v_DeptObjectiveListDb.cs",
    "MdcHR26Apps.Models/Views/v_ProcessTRListDB/Iv_ProcessTRListRepository.cs",
    "MdcHR26Apps.Models/Views/v_ProcessTRListDB/v_ProcessTRListDB.cs",
    "MdcHR26Apps.Models/Views/v_ProcessTRListDB/v_ProcessTRListRepository.cs",
    "MdcHR26Apps.Models/Views/v_ReportTaskListDB/Iv_ReportTaskListRepository.cs",
    "MdcHR26Apps.Models/Views/v_ReportTaskListDB/v_ReportTaskListDB.cs",
    "MdcHR26Apps.Models/Views/v_ReportTaskListDB/v_ReportTaskListRepository.cs",
    "MdcHR26Apps.Models/Views/v_TotalReportListDB/v_TotalReportListDB.cs",
    "MdcHR26Apps.Models/MdcHR26AppsAddDbContext.cs"
)

$allFiles = $createdFiles + $modifiedFiles

$passed = @()
$different = @()
$missing = @()

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "프로젝트 동기화 검증 시작" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$total = $allFiles.Count
$current = 0

foreach ($file in $allFiles) {
    $current++
    Write-Progress -Activity "파일 검증 중" -Status "$current/$total" -PercentComplete (($current / $total) * 100)

    $currentFile = Join-Path $currentPath $file
    $targetFile = Join-Path $targetPath $file

    $currentExists = Test-Path $currentFile
    $targetExists = Test-Path $targetFile

    if (-not $currentExists) {
        Write-Host "[ERROR] 현재 프로젝트에 파일 없음: $file" -ForegroundColor Red
        continue
    }

    if (-not $targetExists) {
        $missing += [PSCustomObject]@{
            File = $file
            Reason = "실제 프로젝트에 파일 없음"
        }
        continue
    }

    # 파일 해시 비교
    $currentHash = (Get-FileHash -Path $currentFile -Algorithm MD5).Hash
    $targetHash = (Get-FileHash -Path $targetFile -Algorithm MD5).Hash

    if ($currentHash -eq $targetHash) {
        $passed += $file
    } else {
        $different += [PSCustomObject]@{
            File = $file
            CurrentHash = $currentHash
            TargetHash = $targetHash
        }
    }
}

Write-Progress -Activity "파일 검증 중" -Completed

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "검증 결과" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "총 파일: $total" -ForegroundColor White
Write-Host "통과: $($passed.Count) ($([math]::Round(($passed.Count / $total) * 100, 1))%)" -ForegroundColor Green
Write-Host "차이: $($different.Count) ($([math]::Round(($different.Count / $total) * 100, 1))%)" -ForegroundColor Yellow
Write-Host "누락: $($missing.Count) ($([math]::Round(($missing.Count / $total) * 100, 1))%)" -ForegroundColor Red
Write-Host ""

if ($passed.Count -gt 0) {
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "통과한 파일 ($($passed.Count)개)" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    foreach ($file in $passed) {
        Write-Host "  $file" -ForegroundColor Green
    }
    Write-Host ""
}

if ($different.Count -gt 0) {
    Write-Host "========================================" -ForegroundColor Yellow
    Write-Host "차이가 있는 파일 ($($different.Count)개)" -ForegroundColor Yellow
    Write-Host "========================================" -ForegroundColor Yellow
    foreach ($item in $different) {
        Write-Host "  $($item.File)" -ForegroundColor Yellow
    }
    Write-Host ""
}

if ($missing.Count -gt 0) {
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "누락된 파일 ($($missing.Count)개)" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    foreach ($item in $missing) {
        Write-Host "  $($item.File)" -ForegroundColor Red
        Write-Host "    사유: $($item.Reason)" -ForegroundColor Red
    }
    Write-Host ""
}

# 결과를 JSON으로 출력
$result = @{
    Total = $total
    Passed = $passed.Count
    Different = $different.Count
    Missing = $missing.Count
    PassedFiles = $passed
    DifferentFiles = $different
    MissingFiles = $missing
}

$result | ConvertTo-Json -Depth 10 | Out-File -FilePath "validation_result.json" -Encoding UTF8

Write-Host "상세 결과가 validation_result.json에 저장되었습니다." -ForegroundColor Cyan
