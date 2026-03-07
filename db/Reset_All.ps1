# =============================================
# Reset_All.ps1
# 專案名稱：EatTogether (義起吃)
# 用途：一鍵重置資料庫結構並匯入所有測試資料
# 使用步驟：選此檔案 -> 滑鼠右鍵 -> "使用 PowerShell 執行"
# =============================================

# 自動抓取腳本所在目錄，組員不需要手動修改路徑
$path = $PSScriptRoot

# 執行前詢問連線資訊，組員輸入自己的設定即可
$inputServer = Read-Host "請輸入 SQL Server 實例名稱 (直接按 Enter 使用預設值 .\sql2025)"
$server = if ($inputServer -ne "") { $inputServer } else { ".\sql2025" }

$inputUser = Read-Host "請輸入登入帳號 (直接按 Enter 使用預設值 sa)"
$dbUser = if ($inputUser -ne "") { $inputUser } else { "sa" }

$securePass = Read-Host "請輸入登入密碼" -AsSecureString
$dbPass = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePass)
)

# CreateDatabase.sql 需要連到 master 才能 DROP/CREATE 資料庫
# 其餘所有資料檔案都連到 EatTogetherDB
# 這樣組員不需要在每個 sql 檔案裡寫 USE EatTogetherDB
$dbForCreate = "master"
$dbForData   = "EatTogetherDB"

Write-Host ">>> ==========================================" -ForegroundColor Cyan
Write-Host ">>> EatTogether 資料庫重置腳本啟動" -ForegroundColor Cyan
Write-Host ">>> 伺服器：$server" -ForegroundColor Cyan
Write-Host ">>> 腳本目錄：$path" -ForegroundColor Cyan
Write-Host ">>> ==========================================" -ForegroundColor Cyan

# =============================================
# 依序執行的 SQL 檔案清單
# =============================================
$files = @(

    # --- 建立資料庫結構 ---
    "CreateDatabase.sql",

    # --- Phase 1: 基礎獨立資料 ---
    "01_Roles.sql",
    "02_Functions.sql",
    "03_Users.sql",
    "04_Members.sql",
    "05_Categories.sql",
    "06_SetMeals.sql",
    "07_Tables.sql",
    "08_Coupons.sql",
    "09_Reservations.sql",
    "10_Events.sql",
    "11_ArticleCategories.sql",
    "12_EmailQueue.sql",

    # --- Phase 2: 依賴型資料 ---
    "13_UserRoles.sql",
    "14_RoleFunctions.sql",
    "15_Dishes.sql",
    "16_Articles.sql",
    "17_SubscriptionPreferences.sql",

    # --- Phase 3: 複合與關聯資料 ---
    "18_SetMealItems.sql",
    "18_1_Products.sql",
    # "19_MemberFavorites.sql",
    "20_MemberCoupons.sql",
    "21_UserNotifications.sql"

    # --- Phase 4: 核心交易資料 ---
    "22_PreOrders.sql",
    "23_PreOrderDetails.sql",
    "24_Payments.sql",
    "25_(更新 Payments).sql"
    "26_Orders.sql",
    "27_OrderDetails.sql"

    # --- Phase 5: 修復循環參照 ---
    # "99_Update_Payment_FK.sql"
)

# =============================================
# 逐一執行每個 SQL 檔案
# =============================================
$hasError = $false

foreach ($file in $files) {
    $fullPath = Join-Path $path $file

    # 檢查檔案是否存在
    if (-not (Test-Path $fullPath)) {
        Write-Host ">>> [跳過] 找不到檔案：$file" -ForegroundColor Yellow
        continue
    }

    Write-Host ">>> 執行：$file ..." -ForegroundColor White

    # CreateDatabase.sql 連 master，其餘全部連 EatTogetherDB
    if ($file -eq "CreateDatabase.sql") {
        $currentDb = $dbForCreate
    } else {
        $currentDb = $dbForData
    }

    # 自動偵測檔案編碼（支援 UTF-8 with BOM、UTF-16、Big5 等）
    $bytes = [System.IO.File]::ReadAllBytes($fullPath)

    # 判斷 BOM 來決定編碼，並解碼成字串
    if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
        $sql = [System.Text.Encoding]::UTF8.GetString($bytes, 3, $bytes.Length - 3)
        Write-Host "    [編碼] UTF-8 with BOM" -ForegroundColor DarkGray
    } elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE) {
        $sql = [System.Text.Encoding]::Unicode.GetString($bytes, 2, $bytes.Length - 2)
        Write-Host "    [編碼] UTF-16 LE" -ForegroundColor DarkGray
    } elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFE -and $bytes[1] -eq 0xFF) {
        $sql = [System.Text.Encoding]::BigEndianUnicode.GetString($bytes, 2, $bytes.Length - 2)
        Write-Host "    [編碼] UTF-16 BE" -ForegroundColor DarkGray
    } else {
        $sql = [System.Text.Encoding]::Default.GetString($bytes)
        Write-Host "    [編碼] Big5 (SSMS 預設)" -ForegroundColor DarkGray
    }

    # 寫入暫存檔再讓 sqlcmd 用 -i 讀取（管線傳入無法正確處理 GO）
    $tempFile = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), [System.IO.Path]::GetRandomFileName() + ".sql")
    $utf8Bom  = [System.Text.UTF8Encoding]::new($true)
    [System.IO.File]::WriteAllText($tempFile, $sql, $utf8Bom)

    sqlcmd -S $server -U $dbUser -P $dbPass -d $currentDb -i $tempFile -f 65001 -C

    # 清除暫存檔
    Remove-Item $tempFile -Force

    if ($LASTEXITCODE -ne 0) {
        Write-Host ">>> [錯誤] $file 執行失敗，請檢查上方錯誤訊息。" -ForegroundColor Red
        $hasError = $true
        break
    }

    Write-Host ">>> [完成] $file" -ForegroundColor Green
}

# =============================================
# 結果摘要
# =============================================
Write-Host ">>> ==========================================" -ForegroundColor Cyan
if ($hasError) {
    Write-Host ">>> 執行過程中發生錯誤，請確認後重新執行。" -ForegroundColor Red
} else {
    Write-Host ">>> 全部執行完畢！資料庫 EatTogetherDB 重置成功。" -ForegroundColor Green
}
Write-Host ">>> ==========================================" -ForegroundColor Cyan