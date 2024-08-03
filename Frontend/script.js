document.addEventListener("DOMContentLoaded", () => {
  localStorage.removeItem("userId");
  localStorage.removeItem("email");
  localStorage.removeItem("token");
  const isDark = localStorage.getItem("isDark") === "true";
  const themeToggle = document.getElementById("theme-toggle");

  if (isDark) {
    applyDarkMode(true);
    themeToggle.checked = true;
  }

  document
    .getElementById("theme-toggle")
    .addEventListener("change", toggleTheme);
});

function toggleTheme() {
  const isDark = this.checked;
  applyDarkMode(isDark);
  localStorage.setItem("isDark", isDark);
}

function applyDarkMode(isDark) {
  const sunIcon = document.getElementById("sun-icon");
  const moonIcon = document.getElementById("moon-icon");
  const body = document.body;
  const darkModeElements = document.querySelectorAll("h1, p, .toast-container");

  if (isDark) {
    body.classList.add("dark-mode");
    sunIcon.src = "../public/icon-sun-light.svg";
    moonIcon.src = "../public/icon-moon-light.svg";
    darkModeElements.forEach((element) => element.classList.add("dark-mode"));
  } else {
    body.classList.remove("dark-mode");
    sunIcon.src = "../public/icon-sun-dark.svg";
    moonIcon.src = "../public/icon-moon-dark.svg";
    darkModeElements.forEach((element) =>
      element.classList.remove("dark-mode")
    );
  }
}

function showRegisterForm() {
  window.location.href = "./html/register.html";
}

function showLoginForm() {
  window.location.href = "./html/login.html";
}
