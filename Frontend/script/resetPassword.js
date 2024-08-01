const IP = "https://quizbackend.raghavendiran.cloud";

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