// Simple-DataTables
// https://github.com/fiduswriter/Simple-DataTables/wiki

const datatablesSimple = document.querySelector('#datatablesSimple');
if (datatablesSimple) {
    new simpleDatatables.DataTable(datatablesSimple, {
        perPageSelect: false,   // 停用 entries per page
        labels: {
            info: ""    // 設成空字串就不會顯示，停用底部筆數資訊
        },
        searchable: false,      // 停用 search
        sortable: false,        // 停用排序
        paging: true            // 保留分頁
    });
}

