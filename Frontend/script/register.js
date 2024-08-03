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

document
  .getElementById("register-form")
  .addEventListener("submit", function (event) {
    event.preventDefault();

    const registerButton = document.getElementById("register-button");
    const spinner = registerButton.querySelector(".spinner-border");

    // Show spinner
    registerButton.disabled = true;
    spinner.classList.remove("d-none");

    const name = document.getElementById("name").value;
    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;

    const data = {
      name: name,
      email: email,
      password: password,
    };

    fetch(`${IP}/api/Auth/register`, {
      method: "POST",
      headers: {
        Accept: "text/plain",
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    })
      .then((response) => response.json())
      .then((data) => {
        // Hide spinner
        registerButton.disabled = false;
        spinner.classList.add("d-none");

        if (data.token) {
          // Store token, userId, and email in local storage
          localStorage.setItem("token", data.token);
          localStorage.setItem("userId", data.user.userId); // Assuming userId is part of the response
          localStorage.setItem("email", email); // Store the email from the form

          showToast("success-toast");
          // Navigate to quizzes.html after showing the toast
          setTimeout(() => {
            window.location.href = "quizzes.html";
          }, 2000); // Adjust the delay as needed
        } else {
          document.getElementById("error-message").textContent = data.message;
          showToast("error-toast");
        }
      })
      .catch((error) => {
        // Hide spinner
        registerButton.disabled = false;
        spinner.classList.add("d-none");

        document.getElementById("error-message").textContent =
          "Registration failed!";
        showToast("error-toast");
      });
  });

function showToast(toastId) {
  const toastEl = document.getElementById(toastId);
  const toast = new bootstrap.Toast(toastEl);
  toast.show();
}
