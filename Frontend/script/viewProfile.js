document.addEventListener("DOMContentLoaded", () => {
  const userId = localStorage.getItem("userId"); // Get userId from localStorage
  const token = localStorage.getItem("token"); // Get token from localStorage

  if (!userId || !token) {
    alert("User ID or token not found. Please log in again.");
    return; // Exit if userId or token is not available
  }

  const apiUrl = `https://quizbackend.raghavendiran.cloud/api/Profile/view-profile?userId=${userId}`;

  fetch(apiUrl, {
    headers: {
      accept: "text/plain",
      Authorization: `Bearer ${token}`,
    },
  })
    .then((response) => response.json())
    .then((data) => {
      if (data.errorcode) {
        alert(`Error: ${data.message}`);
      } else {
        document.getElementById("email").textContent = data.email;
        document.getElementById("name").textContent = data.name;
        const attemptsList = document.getElementById("attempts-list");
        data.attempts.forEach((attempt) => {
          const listItem = document.createElement("div"); // Changed to div for card style
          listItem.className = "card mb-3"; // Bootstrap card class
          listItem.innerHTML = `
                    <div class="card-body">
                      <h5 class="card-title">Quiz ID: ${attempt.quizId}</h5>
                      <p class="card-text"><strong>Score:</strong> ${
                        attempt.score
                      }</p>
                      <p class="card-text"><strong>Completed At:</strong> ${new Date(
                        attempt.completedAt
                      ).toLocaleString()}</p>
                      ${
                        attempt.certificate
                          ? `<a href="${attempt.certificate.url}" class="btn btn-primary" download>Download Certificate</a>`
                          : ""
                      }
                    </div>
                `;
          attemptsList.appendChild(listItem);
        });
      }
    })
    .catch((error) => {
      console.error("Error fetching profile:", error);
    });

  document
    .getElementById("theme-toggle")
    .addEventListener("change", function () {
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
    });
});
