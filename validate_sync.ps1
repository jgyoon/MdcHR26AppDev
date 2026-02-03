# Project Synchronization Validation Script
param(
    [string]$ChecklistFile = "works/sync-checklists/20260130_1345_sync_checklist.md"
)

$ErrorActionPreference = "Continue"

$src = "C:\Codes\00_Develop_Cursor\10_MdcHR26Apps"
$dst = "C:\Codes\41_MdcHR26\MdcHR26App"

# Created files
$created = @(
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

# Modified files
$modified = @(
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

$all = $created + $modified

$passed = @()
$different = @()
$missing = @()

Write-Host "========================================"
Write-Host "Validation Start"
Write-Host "========================================"
Write-Host ""
Write-Host "Total files: $($all.Count)"
Write-Host ""

$i = 0
foreach ($file in $all) {
    $i++
    Write-Progress -Activity "Validating" -Status "$i / $($all.Count)" -PercentComplete (($i / $all.Count) * 100)

    $srcPath = Join-Path $src $file
    $dstPath = Join-Path $dst $file

    $srcExists = Test-Path $srcPath
    $dstExists = Test-Path $dstPath

    if (-not $srcExists) {
        Write-Host "[ERROR] Source file missing: $file" -ForegroundColor Red
        continue
    }

    if (-not $dstExists) {
        $missing += $file
        continue
    }

    $srcHash = (Get-FileHash -Path $srcPath -Algorithm MD5).Hash
    $dstHash = (Get-FileHash -Path $dstPath -Algorithm MD5).Hash

    if ($srcHash -eq $dstHash) {
        $passed += $file
    } else {
        $different += $file
    }
}

Write-Progress -Activity "Validating" -Completed

Write-Host ""
Write-Host "========================================"
Write-Host "Validation Results"
Write-Host "========================================"
Write-Host ""

$passedPercent = [math]::Round($passed.Count / $all.Count * 100, 1)
$differentPercent = [math]::Round($different.Count / $all.Count * 100, 1)
$missingPercent = [math]::Round($missing.Count / $all.Count * 100, 1)

Write-Host "Passed: $($passed.Count) ($passedPercent%)" -ForegroundColor Green
Write-Host "Different: $($different.Count) ($differentPercent%)" -ForegroundColor Yellow
Write-Host "Missing: $($missing.Count) ($missingPercent%)" -ForegroundColor Red
Write-Host ""

if ($different.Count -gt 0) {
    Write-Host "Files with differences:" -ForegroundColor Yellow
    foreach ($f in $different) {
        Write-Host "  - $f" -ForegroundColor Yellow
    }
    Write-Host ""
}

if ($missing.Count -gt 0) {
    Write-Host "Missing files:" -ForegroundColor Red
    foreach ($f in $missing) {
        Write-Host "  - $f" -ForegroundColor Red
    }
    Write-Host ""
}

# Save JSON result
$result = @{
    ChecklistFile = $ChecklistFile
    Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Total = $all.Count
    Passed = $passed.Count
    Different = $different.Count
    Missing = $missing.Count
    PassedFiles = $passed
    DifferentFiles = $different
    MissingFiles = $missing
}

$jsonPath = Join-Path $src "validation_result.json"
$result | ConvertTo-Json -Depth 10 | Out-File -FilePath $jsonPath -Encoding UTF8

Write-Host "Detailed result saved to: $jsonPath" -ForegroundColor Cyan
