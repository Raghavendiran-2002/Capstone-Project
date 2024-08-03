const IP = "https://quizbackend.raghavendiran.cloud";
//const IP = "http://127.0.0.1:8000";

document.addEventListener("DOMContentLoaded", function () {
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
  localStorage.removeItem("token");
  localStorage.removeItem("userId");
  localStorage.removeItem("email");
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

document.getElementById("login-form").addEventListener("submit", function (e) {
  e.preventDefault();
  const email = document.getElementById("email").value;
  const password = document.getElementById("password").value;
  const loginButton = document.querySelector("button[type='submit']");

  // Disable the button and show spinner
  loginButton.innerHTML = `<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Loggin In...`;
  loginButton.disabled = true;

  const data = {
    email: email,
    password: password,
  };

  fetch(`${IP}/api/Auth/login`, {
    method: "POST",
    headers: {
      accept: "text/plain",
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  })
    .then((response) => response.json())
    .then((data) => {
      // Re-enable the button and reset its content
      loginButton.innerHTML = "Login";
      loginButton.disabled = false;

      if (data.token) {
        localStorage.setItem("token", data.token);
        localStorage.setItem("userId", data.user.userId);
        localStorage.setItem("email", data.user.email);
        showToast("Login successful!", "success");
        window.location.href = "../html/quizzes.html";
      } else {
        showToast(data.message, "danger");
      }
    })
    .catch((error) => {
      // Re-enable the button and reset its content
      loginButton.innerHTML = "Login";
      loginButton.disabled = false;

      showToast("An error occurred. Please try again.", "danger");
      console.error("Error:", error);
    });
});

function showToast(message, type) {
  const toastContainer = document.querySelector(".toast-container"); // Updated to use class selector
  if (!toastContainer) {
    console.error("Toast container not found");
    return;
  }
  const toast = document.createElement("div");
  toast.className = `toast align-items-center text-bg-${type} border-0`;
  toast.setAttribute("role", "alert");
  toast.setAttribute("aria-live", "assertive");
  toast.setAttribute("aria-atomic", "true");

  const toastBody = document.createElement("div");
  toastBody.className = "d-flex";
  toastBody.innerHTML = `
        <div class="toast-body">
            ${message}
        </div>
        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
    `;

  toast.appendChild(toastBody);
  toastContainer.appendChild(toast);

  const bootstrapToast = new bootstrap.Toast(toast);
  bootstrapToast.show();
}
