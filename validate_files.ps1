$currentProject = 'C:\Codes\00_Develop_Cursor\10_MdcHR26Apps'
$productionProject = 'C:\Codes\41_MdcHR26\MdcHR26App'

$files = @(
    'MdcHR26Apps.Models\DeptObjective\IDeptObjectiveRepository.cs',
    'MdcHR26Apps.Models\EvaluationAgreement\AgreementDb.cs',
    'MdcHR26Apps.Models\EvaluationAgreement\AgreementRepository.cs',
    'MdcHR26Apps.Models\EvaluationAgreement\IAgreementRepository.cs',
    'MdcHR26Apps.Models\EvaluationLists\EvaluationLists.cs',
    'MdcHR26Apps.Models\EvaluationLists\EvaluationListsRepository.cs',
    'MdcHR26Apps.Models\EvaluationLists\IEvaluationListsRepository.cs',
    'MdcHR26Apps.Models\EvaluationSubAgreement\ISubAgreementRepository.cs',
    'MdcHR26Apps.Models\EvaluationSubAgreement\SubAgreementDb.cs',
    'MdcHR26Apps.Models\EvaluationSubAgreement\SubAgreementRepository.cs',
    'MdcHR26Apps.Models\EvaluationTasks\ITasksRepository.cs',
    'MdcHR26Apps.Models\EvaluationTasks\TasksDb.cs',
    'MdcHR26Apps.Models\EvaluationTasks\TasksRepository.cs'
)

$results = @()

foreach ($file in $files) {
    $currentPath = Join-Path $currentProject $file
    $productionPath = Join-Path $productionProject $file

    $currentExists = Test-Path $currentPath
    $productionExists = Test-Path $productionPath

    if ($currentExists -and $productionExists) {
        $currentHash = (Get-FileHash -Path $currentPath -Algorithm SHA256).Hash
        $productionHash = (Get-FileHash -Path $productionPath -Algorithm SHA256).Hash

        if ($currentHash -eq $productionHash) {
            $results += [PSCustomObject]@{
                File = $file
                Status = 'MATCH'
                Details = 'Content matches'
            }
        } else {
            $results += [PSCustomObject]@{
                File = $file
                Status = 'DIFFER'
                Details = 'Content differs'
            }
        }
    } elseif (-not $productionExists) {
        $results += [PSCustomObject]@{
            File = $file
            Status = 'MISSING'
            Details = 'File not found in production'
        }
    } elseif (-not $currentExists) {
        $results += [PSCustomObject]@{
            File = $file
            Status = 'ERROR'
            Details = 'File not found in current'
        }
    }
}

$results | Format-Table -AutoSize
Write-Host ''
Write-Host '=== Summary ==='
Write-Host 'MATCH:' ($results | Where-Object {$_.Status -eq 'MATCH'}).Count
Write-Host 'DIFFER:' ($results | Where-Object {$_.Status -eq 'DIFFER'}).Count
Write-Host 'MISSING:' ($results | Where-Object {$_.Status -eq 'MISSING'}).Count

$results | ConvertTo-Json -Depth 10 | Out-File -FilePath 'validation_models_result.json' -Encoding utf8
