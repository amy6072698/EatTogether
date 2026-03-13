/*!
    * Start Bootstrap - SB Admin v7.0.7 (https://startbootstrap.com/template/sb-admin)
    * Copyright 2013-2023 Start Bootstrap
    * Licensed under MIT (https://github.com/StartBootstrap/startbootstrap-sb-admin/blob/master/LICENSE)
    */
// 
// Scripts
// 

// Toggle the side navigation
const sidebarToggle = document.body.querySelector('#sidebarToggle');
if (sidebarToggle) {
    // Uncomment Below to persist sidebar toggle between refreshes
    // if (localStorage.getItem('sb|sidebar-toggle') === 'true') {
    //     document.body.classList.toggle('sb-sidenav-toggled');
    // }
    sidebarToggle.addEventListener('click', event => {
        event.preventDefault();
        document.body.classList.toggle('sb-sidenav-toggled');
        localStorage.setItem('sb|sidebar-toggle', document.body.classList.contains('sb-sidenav-toggled'));
    });
}

// Navbar 員工字母頭像隨機背景色
const userInitialEl = document.querySelector('.navbar-user-initial');
if (userInitialEl) {
    const avatarColors = [
        '#C9A96E', '#8B5E3C', '#A0522D', '#6B4226',
        '#B07D4E', '#7A4F2D', '#9C6B3C', '#5C3317'
    ];
    const name = userInitialEl.dataset.name || '';
    // 依名字雜湊決定顏色，確保同一使用者每次顏色相同
    let hash = 0;
    for (let i = 0; i < name.length; i++) {
        hash = (hash * 31 + name.charCodeAt(i)) & 0xffffffff;
    }
    userInitialEl.style.backgroundColor = avatarColors[Math.abs(hash) % avatarColors.length];
}

    


