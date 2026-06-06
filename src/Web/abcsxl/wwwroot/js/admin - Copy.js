// 页面加载动画
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('mainContent').classList.add('page-loaded');
});

// 侧边栏切换
const sidebar = document.getElementById('sidebar');
const backdrop = document.getElementById('sidebarBackdrop');
const toggleBtn = document.getElementById('sidebarToggle');

function closeSidebar() {
    sidebar.classList.remove('show');
    backdrop.classList.remove('show');
    document.body.style.overflow = '';
}

function openSidebar() {
    sidebar.classList.add('show');
    backdrop.classList.add('show');
    document.body.style.overflow = 'hidden';
}

if (toggleBtn) {
    toggleBtn.addEventListener('click', function (e) {
        e.stopPropagation();
        if (sidebar.classList.contains('show')) {
            closeSidebar();
        } else {
            openSidebar();
        }
    });
}

if (backdrop) {
    backdrop.addEventListener('click', closeSidebar);
}

// 点击链接关闭侧边栏（手机）
if (sidebar) {
    sidebar.querySelectorAll('.nav-link').forEach(link => {
        link.addEventListener('click', function () {
            if (window.innerWidth < 992) {
                closeSidebar();
            }
        });
    });
}

// 窗口大小变化
window.addEventListener('resize', function () {
    if (window.innerWidth >= 992) {
        closeSidebar();
        document.body.style.overflow = '';
    }
});

// 暗黑模式
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

// 手势支持
let touchStartX = 0;
document.addEventListener('touchstart', function (e) {
    touchStartX = e.touches[0].clientX;
}, { passive: true });

document.addEventListener('touchmove', function (e) {
    if (window.innerWidth < 992 && sidebar && !sidebar.classList.contains('show')) {
        const touchX = e.touches[0].clientX;
        if (touchStartX < 30 && touchX - touchStartX > 50) {
            e.preventDefault();
            openSidebar();
        }
    }
}, { passive: false });