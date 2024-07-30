document.addEventListener("DOMContentLoaded", () => {
  const userId = 1; // replace with the actual user ID
  const apiUrl = `https://quizbackend.raghavendiran.cloud/api/Profile/view-profile?userId=${userId}`;
  const token =
    "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJuYmYiOjE3MjIyMjU3OTQsImV4cCI6MTcyMjgzMDU5NCwiaWF0IjoxNzIyMjI1Nzk0fQ.jR1x1_c95UOPTRVtSytdXNTuHdkeL5SG4jMYt70bxdo"; // replace with the actual token

  fetch(apiUrl, {
    headers: {
      accept: "text/plain",
      Authorization: token,
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
          const listItem = document.createElement("li");
          listItem.className = "list-group-item";
          listItem.innerHTML = `
                    <p><strong>Quiz ID:</strong> ${attempt.quizId}</p>
                    <p><strong>Score:</strong> ${attempt.score}</p>
                    <p><strong>Completed At:</strong> ${new Date(
                      attempt.completedAt
                    ).toLocaleString()}</p>
                    ${
                      attempt.certificate
                        ? `<a href="${attempt.certificate.url}" class="btn btn-primary" download>Download Certificate</a>`
                        : ""
                    }
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
