const IP ="https://quizbackend.raghavendiran.cloud"

document.getElementById("theme-toggle").addEventListener("change", function () {
  if (this.checked) {
    document.body.classList.add("dark-mode");
  } else {
    document.body.classList.remove("dark-mode");
  }
});

document
  .getElementById("register-form")
  .addEventListener("submit", function (event) {
    event.preventDefault();

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
        if (data.token) {
          showToast("success-toast");
        } else {
          document.getElementById("error-message").textContent = data.message;
          showToast("error-toast");
        }
      })
      .catch((error) => {
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
