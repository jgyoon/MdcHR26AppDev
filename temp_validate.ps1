$srcBase = 'C:\Codes\00_Develop_Cursor\10_MdcHR26Apps'
$destBase = 'C:\Codes\41_MdcHR26\MdcHR26App'

# 검증할 파일 목록 (주요 생성 파일)
$newFiles = @(
    'Database\03_06_SeedData_Process.sql',
    'MdcHR26Apps.BlazorServer\Components\Pages\Admin\Index.razor',
    'MdcHR26Apps.BlazorServer\Components\Pages\Admin\Index.razor.cs',
    'MdcHR26Apps.BlazorServer\Components\Pages\Admin\EUsersManage.razor',
    'MdcHR26Apps.BlazorServer\Components\Pages\Admin\UserManage.razor',
    'MdcHR26Apps.BlazorServer\Components\Pages\Components\Common\SearchbarComponent.razor',
    'MdcHR26Apps.BlazorServer\Components\Pages\Components\Table\UserListTable.razor',
    'MdcHR26Apps.BlazorServer\Components\Pages\Components\Modal\UserDeleteModal.razor',
    'MdcHR26Apps.BlazorServer\Utils\UserUtils.cs',
    'MdcHR26Apps.Models\Views\v_EvaluationUsersList\v_EvaluationUsersList.cs'
)

# 수정 파일 목록
$modifiedFiles = @(
    'Database\02_CreateViews.sql',
    'MdcHR26Apps.BlazorServer\Components\_Imports.razor',
    'MdcHR26Apps.BlazorServer\Program.cs',
    'MdcHR26Apps.BlazorServer\Data\AppStateService.cs',
    'MdcHR26Apps.Models\User\UserDb.cs',
    'MdcHR26Apps.Models\User\UserRepository.cs',
    'MdcHR26Apps.Models\MdcHR26AppsAddExtensions.cs'
)

Write-Host "=== 생성 파일 검증 ===" -ForegroundColor Cyan
$missingNew = @()
foreach ($file in $newFiles) {
    $srcPath = Join-Path $srcBase $file
    $destPath = Join-Path $destBase $file

    $destExists = Test-Path $destPath

    if ($destExists) {
        Write-Host "[OK] $file" -ForegroundColor Green
    } else {
        Write-Host "[MISSING] $file" -ForegroundColor Red
        $missingNew += $file
    }
}

Write-Host ""
Write-Host "=== 수정 파일 검증 ===" -ForegroundColor Cyan
$missingMod = @()
foreach ($file in $modifiedFiles) {
    $srcPath = Join-Path $srcBase $file
    $destPath = Join-Path $destBase $file

    $destExists = Test-Path $destPath

    if ($destExists) {
        Write-Host "[OK] $file" -ForegroundColor Green
    } else {
        Write-Host "[MISSING] $file" -ForegroundColor Red
        $missingMod += $file
    }
}

Write-Host ""
Write-Host "=== 요약 ===" -ForegroundColor Yellow
Write-Host "생성 파일: $($newFiles.Count)개 중 $($newFiles.Count - $missingNew.Count)개 동기화 완료"
Write-Host "수정 파일: $($modifiedFiles.Count)개 중 $($modifiedFiles.Count - $missingMod.Count)개 동기화 완료"
Write-Host "총 누락: $($missingNew.Count + $missingMod.Count)개"
