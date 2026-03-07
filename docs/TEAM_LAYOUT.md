# 團隊 Layout 開發指南

## 資料夾結構

- `layout/` - 共用頁面範本（修改後請通知團隊同步更新）
- `pages/` - 功能頁面（組員開發區域）
- `css/pages/` - 各頁面專用樣式
- `js/pages/` - 各頁面專用腳本
- `css/styles.css` - 全域共用樣式 (含 Bootstrap 5，修改時請加註解說明修改內容)
- `js/scripts.js` - 全域共用腳本 (修改時請加註解說明修改內容)

## 命名規範

所有檔案使用 kebab-case 命名法（小寫字母，用 `-` 連接），保持 HTML/CSS/JS 同名：

pages/order-history.html
css/pages/order-history.css
js/pages/order-history.js

## 如何開始新頁面

### 複製範本開發

1. 複製範本檔案到 pages 資料夾並重新命名

   layout/layout.html → pages/user-management.html

2. 修改頁面標題 `<title>` 和內容區的 `<h1>`

3. 如需專用 CSS/JS，建立對應檔案

   css/pages/user-management.css
   js/pages/user-management.js

4. 更新 HTML 中的 CSS/JS 引入路徑

   ```html
   <link href="../css/pages/user-management.css" rel="stylesheet" />
   <script src="../js/pages/user-management.js"></script>
   ```

## 範本檔案

**共用頁面範本（Navbar + Sidebar + 內容區域）**  

- `layout/layout.html` - 共用頁面的 HTML 範本

## 快速開始範例

假設要建立「員工管理」頁面：

1. 複製範本到 pages 資料夾

   layout/layout.html → pages/user-management.html

2. 修改標題（在 `<head>` 和 `<main>` 區域）

   ```html
   <title>員工管理 | 餐廳管理後台</title>
   ...
   <h1 class="mt-4">員工管理</h1>
   ```

3. 如需專用樣式/腳本，建立新檔案

   css/pages/user-management.css
   js/pages/user-management.js

4. 更新引入路徑

   ```html
   <link href="../css/pages/user-management.css" rel="stylesheet" />
   <script src="../js/pages/user-management.js"></script>
   ```

5. 編寫你的內容（在 `<div class="card-body">` 內）

## 開發注意事項

### 注意引用路徑是否正確

- `pages/` 下的檔案引用路徑：`../css/styles.css`、`../js/scripts.js`
- `layout/` 下的檔案引用路徑：`../css/styles.css`、`../js/scripts.js`
- 頁面專用路徑：`../css/pages/檔名.css`、`../js/pages/檔名.js`

### CSS/JS 引入順序

```html
<!-- CSS：全域樣式(styles.css)在前，頁面專用樣式放最後 -->
<link href="../css/styles.css" rel="stylesheet" />
<link href="../css/pages/your-page.css" rel="stylesheet" />

<!-- JS：Bootstrap → 全域 → 頁面專用 -->
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/js/bootstrap.bundle.min.js"></script>
<script src="../js/scripts.js"></script>
<script src="../js/pages/your-page.js"></script>
```

### 樣式與腳本使用原則

- 全域共用的樣式和功能 → 放在 `css/styles.css` 和 `js/scripts.js`
- 頁面專屬的樣式和功能 → 建立對應的 `css/pages/` 和 `js/pages/` 檔案
- 只在需要的頁面引入專用檔案，避免資源浪費

## 共用範本修改與同步

`layout/layout.html` 是共用範本,組員可以自由修改。修改後請通知其他人同步更新。

### 什麼時候會修改範本？

- 新增或移除側欄選單項目
- 調整 Navbar 功能或樣式
- 更新頁面整體結構或布局
- 新增全域 CSS/JS 引用

### 修改流程

1. **直接修改範本**
   - 修改 `layout/layout.html`

2. **測試確認**
   - 確認修改後的範本正常運作

3. **通知團隊同步**  
   在群組中貼上通知，包含：
   - 📝 **修改內容**：說明改了什麼（例如：新增側欄「庫存管理」選單）
   - 📋 **修改位置**：指出哪個檔案的哪個區域（例如：`layout/layout.html` 第 45-50 行）
   - 🔄 **同步提醒**：提醒組員同步更新自己的頁面

4. **版本控制**
   - Commit 訊息請加上 `[範本更新]` 標籤
   - 例如：`[範本更新] 新增側欄選單：庫存管理`

### 同步更新到自己的頁面

收到組員的範本更新通知後：

1. **複製修改部分**  
   根據通知的位置說明，複製 `layout/layout.html` 的對應代碼到自己的 `pages/你的頁面.html`

2. **測試確認**  
   確認自己的頁面正常顯示

### 通知範例

📝 git commit 紀錄範例如下
[範本更新] 新增側欄選單：庫存管理

🔄 群組提醒組員共用範本有更新(訊息範例如下)
大家~我有修改共用頁面範本的內容
修改內容：在側欄功能選單中新增「庫存管理」項目
修改位置：layout/layout.html 第 48-51 行（在「商品管理」之後）
請大家記得同步更新到自己的頁面

### 小提醒

- 不是每次範本更新都需要同步，看你的頁面需求
- 發現問題隨時在群組討論
- 發表前請確認共用樣式是否與自己的頁面相符
