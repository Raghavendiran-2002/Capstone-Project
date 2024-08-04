const IP = "https://quizbackend.raghavendiran.cloud";
//const IP = "http://127.0.0.1:8000";
document.addEventListener("DOMContentLoaded", function () {
  const userId = localStorage.getItem("userId");
  const token = localStorage.getItem("token");

  if (!userId || !token) {
    showToast("User ID or token not found. Please log in again.", "error");
    setTimeout(() => {
      window.location.href = "../index.html";
    }, 1000);
    setTimeout(() => {
      window.location.href = "/index.html";
    }, 1000);
  }
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

document.getElementById("theme-toggle").addEventListener("change", toggleTheme);
document
  .getElementById("reset-password-form")
  .addEventListener("submit", handlePasswordChange);

function toggleTheme() {
  const sunIcon = document.getElementById("sun-icon");
  const moonIcon = document.getElementById("moon-icon");

  if (this.checked) {
    document.body.classList.add("dark-mode");
    sunIcon.src = "../public/icon-sun-light.svg";
    moonIcon.src = "../public/icon-moon-light.svg";
  } else {
    document.body.classList.remove("dark-mode");
    sunIcon.src = "../public/icon-sun-dark.svg";
    moonIcon.src = "../public/icon-moon-dark.svg";
  }
}

function showToast(toastId) {
  const toast = new bootstrap.Toast(document.getElementById(toastId));
  toast.show();
}

async function handlePasswordChange(event) {
  event.preventDefault();
  const email = localStorage.getItem("email");
  const oldPassword = document.getElementById("old-password").value;
  const newPassword = document.getElementById("new-password").value;

  const response = await fetch(`${IP}api/Auth/change-password`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      email,
      oldpassword: oldPassword,
      newpassword: newPassword,
    }),
  });

  const result = await response.json();

  if (response.ok) {
    showToast("success-toast");
  } else {
    document.getElementById("error-message").innerText =
      result.message || "An error occurred";
    showToast("error-toast");
  }
}

const showToast = (message, type) => {
  const toastElement =
    type === "error"
      ? document.getElementById("error-toast")
      : document.getElementById("success-toast");
  const toastBody = toastElement.querySelector(".toast-body");
  toastBody.textContent = message;
  const toast = new bootstrap.Toast(toastElement);
  toast.show();
};
