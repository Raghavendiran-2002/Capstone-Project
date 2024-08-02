const IP = "https://quizbackend.raghavendiran.cloud";
//const IP = "http://127.0.0.1:8000";
document.addEventListener("DOMContentLoaded", function () {
  // Remove token, userId, and email from local storage

  if (localStorage.getItem(isDark)) {
    document.body.classList.add("dark-mode");
    sunIcon.src = "../public/icon-sun-light.svg";
    moonIcon.src = "../public/icon-moon-light.svg";
  } else {
    document.body.classList.remove("dark-mode");
    sunIcon.src = "../public/icon-sun-dark.svg";
    moonIcon.src = "../public/icon-moon-dark.svg";
  }

  localStorage.removeItem("token");
  localStorage.removeItem("userId");
  localStorage.removeItem("email");
});

document.getElementById("theme-toggle").addEventListener("change", function () {
  const sunIcon = document.getElementById("sun-icon");
  const moonIcon = document.getElementById("moon-icon");

  if (this.checked) {
    localStorage.setItem("isDark", true);
    document.body.classList.add("dark-mode");
    sunIcon.src = "../public/icon-sun-light.svg";
    moonIcon.src = "../public/icon-moon-light.svg";
  } else {
    localStorage.setItem("isDark", false);
    document.body.classList.remove("dark-mode");
    sunIcon.src = "../public/icon-sun-dark.svg";
    moonIcon.src = "../public/icon-moon-dark.svg";
  }
});

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
          localStorage.setItem("userId", data.userId); // Assuming userId is part of the response
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
