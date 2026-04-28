// === PC端侧边栏折叠/展开（收缩成图标栏） ===
const sidebar = document.getElementById('sidebar');
const navbarTop = document.getElementById('navbarTop');
const mainContent = document.getElementById('mainContent');
const sidebarTogglePC = document.getElementById('sidebarTogglePC');
const sidebarTogglePCNav = document.getElementById('sidebarTogglePCNav');

// 从本地存储获取侧边栏状态
let sidebarCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';

// 初始化侧边栏状态
function initSidebar() {
    if (window.innerWidth >= 768) {
        if (sidebarCollapsed) {
            sidebar.classList.add('collapsed');
            navbarTop.classList.add('expanded');
            mainContent.classList.add('expanded');
            if (sidebarTogglePC) {
                sidebarTogglePC.innerHTML = '<i class="bi bi-chevron-right"></i>';
            }
        } else {
            sidebar.classList.remove('collapsed');
            navbarTop.classList.remove('expanded');
            mainContent.classList.remove('expanded');
            if (sidebarTogglePC) {
                sidebarTogglePC.innerHTML = '<i class="bi bi-chevron-left"></i>';
            }
        }
    }
}

initSidebar();

// PC端折叠/切换 - 收缩成图标栏
function toggleSidebarPC() {
    sidebarCollapsed = !sidebarCollapsed;
    localStorage.setItem('sidebarCollapsed', sidebarCollapsed);

    if (sidebarCollapsed) {
        sidebar.classList.add('collapsed');
        navbarTop.classList.add('expanded');
        mainContent.classList.add('expanded');
        if (sidebarTogglePC) {
            sidebarTogglePC.innerHTML = '<i class="bi bi-chevron-right"></i>';
        }
    } else {
        sidebar.classList.remove('collapsed');
        navbarTop.classList.remove('expanded');
        mainContent.classList.remove('expanded');
        if (sidebarTogglePC) {
            sidebarTogglePC.innerHTML = '<i class="bi bi-chevron-left"></i>';
        }
    }
}

// 绑定PC端折叠按钮
if (sidebarTogglePC) {
    sidebarTogglePC.addEventListener('click', toggleSidebarPC);
}

if (sidebarTogglePCNav) {
    sidebarTogglePCNav.addEventListener('click', toggleSidebarPC);
}

// === 手机端侧边栏控制 ===
const backdrop = document.getElementById('sidebarBackdrop');
const sidebarToggleMobile = document.getElementById('sidebarToggleMobile');

function closeSidebarMobile() {
    sidebar.classList.remove('show');
    if (backdrop) backdrop.classList.remove('show');
    document.body.style.overflow = '';
}

function openSidebarMobile() {
    sidebar.classList.add('show');
    if (backdrop) backdrop.classList.add('show');
    document.body.style.overflow = 'hidden';
}

if (sidebarToggleMobile) {
    sidebarToggleMobile.addEventListener('click', function (e) {
        e.stopPropagation();
        if (window.innerWidth < 768) {
            if (sidebar.classList.contains('show')) {
                closeSidebarMobile();
            } else {
                openSidebarMobile();
            }
        }
    });
}

if (backdrop) {
    backdrop.addEventListener('click', closeSidebarMobile);
}

if (sidebar) {
    sidebar.querySelectorAll('.nav-link').forEach(link => {
        link.addEventListener('click', function () {
            if (window.innerWidth < 768) {
                closeSidebarMobile();
            }
        });
    });
}

// === 窗口大小变化处理 ===
window.addEventListener('resize', function () {
    if (window.innerWidth >= 768) {
        closeSidebarMobile();
        document.body.style.overflow = '';
        initSidebar();
    } else {
        sidebar.classList.remove('collapsed');
        navbarTop.classList.remove('expanded');
        mainContent.classList.remove('expanded');
    }
});

// === 暗黑模式 ===
const darkModeToggle = document.getElementById('darkModeToggle');
const html = document.documentElement;

const savedTheme = localStorage.getItem('theme') || 'light';
html.setAttribute('data-bs-theme', savedTheme);
updateThemeIcon(savedTheme);

if (darkModeToggle) {
    darkModeToggle.addEventListener('click', function () {
        const currentTheme = html.getAttribute('data-bs-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';

        html.setAttribute('data-bs-theme', newTheme);
        localStorage.setItem('theme', newTheme);
        updateThemeIcon(newTheme);
    });
}

function updateThemeIcon(theme) {
    const icon = darkModeToggle?.querySelector('i');
    if (icon) {
        icon.className = theme === 'dark' ? 'bi bi-sun-fill fs-5' : 'bi bi-moon-fill fs-5';
    }
}

// === 手机端手势 ===
if (window.innerWidth < 768) {
    let touchStartX = 0;

    document.addEventListener('touchstart', function (e) {
        touchStartX = e.touches[0].clientX;
    }, { passive: true });

    document.addEventListener('touchmove', function (e) {
        if (!sidebar.classList.contains('show')) {
            const touchX = e.touches[0].clientX;
            if (touchStartX < 30 && touchX - touchStartX > 50) {
                e.preventDefault();
                openSidebarMobile();
            }
        }
    }, { passive: false });
}