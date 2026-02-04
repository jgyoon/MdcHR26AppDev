$currentBase = 'C:\Codes\00_Develop_Cursor\10_MdcHR26Apps'
$productionBase = 'C:\Codes\41_MdcHR26\MdcHR26App'

$files = @(
    'MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/Details.razor.cs',
    'MdcHR26Apps.BlazorServer/Components/Pages/Admin/TotalReport/ReportInit.razor.cs',
    'MdcHR26Apps.Models/DeptObjective/DeptObjectiveDb.cs',
    'MdcHR26Apps.Models/DeptObjective/DeptObjectiveRepository.cs',
    'MdcHR26Apps.Models/DeptObjective/IDeptObjectiveRepository.cs',
    'MdcHR26Apps.Models/EvaluationAgreement/AgreementDb.cs',
    'MdcHR26Apps.Models/EvaluationAgreement/AgreementRepository.cs',
    'MdcHR26Apps.Models/EvaluationAgreement/IAgreementRepository.cs',
    'MdcHR26Apps.Models/EvaluationLists/EvaluationLists.cs',
    'MdcHR26Apps.Models/EvaluationLists/EvaluationListsRepository.cs',
    'MdcHR26Apps.Models/EvaluationLists/IEvaluationListsRepository.cs',
    'MdcHR26Apps.Models/EvaluationSubAgreement/SubAgreementDb.cs',
    'MdcHR26Apps.Models/EvaluationSubAgreement/SubAgreementRepository.cs',
    'MdcHR26Apps.Models/EvaluationSubAgreement/ISubAgreementRepository.cs',
    'MdcHR26Apps.Models/EvaluationTasks/TasksDb.cs',
    'MdcHR26Apps.Models/EvaluationTasks/TasksRepository.cs',
    'MdcHR26Apps.Models/EvaluationTasks/ITasksRepository.cs'
)

$passCount = 0
$diffCount = 0
$missingCount = 0

$passFiles = @()
$diffFiles = @()
$missingFiles = @()

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Sync Validation Start (SHA256)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Validation Time: $(Get-Date -Format 'yyyy-MM-dd HH:mm')" -ForegroundColor White
Write-Host "Total Files: $($files.Count)" -ForegroundColor White
Write-Host ""

foreach ($file in $files) {
    $currentPath = Join-Path $currentBase $file
    $productionPath = Join-Path $productionBase $file

    $currentExists = Test-Path $currentPath
    $productionExists = Test-Path $productionPath

    if (-not $currentExists) {
        Write-Host "[MISSING] $file" -ForegroundColor Red
        Write-Host "  Current project file not found" -ForegroundColor Red
        $missingCount++
        $missingFiles += $file
        continue
    }

    if (-not $productionExists) {
        Write-Host "[MISSING] $file" -ForegroundColor Red
        Write-Host "  Production project file not found" -ForegroundColor Red
        $missingCount++
        $missingFiles += $file
        continue
    }

    $currentHash = (Get-FileHash -Path $currentPath -Algorithm SHA256).Hash
    $productionHash = (Get-FileHash -Path $productionPath -Algorithm SHA256).Hash

    if ($currentHash -eq $productionHash) {
        Write-Host "[PASS] $file" -ForegroundColor Green
        $passCount++
        $passFiles += $file
    } else {
        Write-Host "[DIFF] $file" -ForegroundColor Yellow
        Write-Host "  Current:    $currentHash" -ForegroundColor Gray
        Write-Host "  Production: $productionHash" -ForegroundColor Gray
        $diffCount++
        $diffFiles += $file
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Validation Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Total:   $($files.Count) files" -ForegroundColor White
Write-Host "Pass:    $passCount files ($([Math]::Round($passCount / $files.Count * 100, 1))%)" -ForegroundColor Green
Write-Host "Diff:    $diffCount files ($([Math]::Round($diffCount / $files.Count * 100, 1))%)" -ForegroundColor Yellow
Write-Host "Missing: $missingCount files ($([Math]::Round($missingCount / $files.Count * 100, 1))%)" -ForegroundColor Red
Write-Host ""

if ($diffCount -gt 0) {
    Write-Host "========================================" -ForegroundColor Yellow
    Write-Host "Files with Differences" -ForegroundColor Yellow
    Write-Host "========================================" -ForegroundColor Yellow
    foreach ($f in $diffFiles) {
        Write-Host "  - $f" -ForegroundColor Yellow
    }
    Write-Host ""
}

if ($missingCount -gt 0) {
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "Missing Files" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    foreach ($f in $missingFiles) {
        Write-Host "  - $f" -ForegroundColor Red
    }
    Write-Host ""
}

if ($passCount -eq $files.Count) {
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "All files are synchronized successfully!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
} else {
    Write-Host "========================================" -ForegroundColor Yellow
    Write-Host "Synchronization needs review." -ForegroundColor Yellow
    Write-Host "========================================" -ForegroundColor Yellow
}

$result = @{
    ValidationTime = (Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
    TotalFiles = $files.Count
    PassCount = $passCount
    DiffCount = $diffCount
    MissingCount = $missingCount
    PassFiles = $passFiles
    DiffFiles = $diffFiles
    MissingFiles = $missingFiles
    AllPassed = ($passCount -eq $files.Count)
}

$result | ConvertTo-Json | Out-File -FilePath "$currentBase\validation_result_hash.json" -Encoding UTF8
Write-Host ""
Write-Host "Validation result saved to validation_result_hash.json" -ForegroundColor Cyan
