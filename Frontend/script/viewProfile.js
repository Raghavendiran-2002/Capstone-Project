const IP = "https://quizbackend.raghavendiran.cloud";
//const IP = "http://127.0.0.1:8000";
document.addEventListener("DOMContentLoaded", () => {
  const userId = localStorage.getItem("userId"); // Get userId from localStorage
  const token = localStorage.getItem("token"); // Get token from localStorage

  if (!userId || !token) {
    alert("User ID or token not found. Please log in again.");
    return; // Exit if userId or token is not available
  }

  const apiUrl = `${IP}/api/Profile/view-profile?userId=${userId}`;

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

        // Display attempts
        const attemptsList = document.getElementById("attempts-list");
        data.attempts.forEach((attempt) => {
          const listItem = document.createElement("div");
          listItem.className = "card mb-3";
          listItem.innerHTML = `
            <div class="card-body">
              <h5 class="card-title">Quiz ID: ${attempt.quizId}</h5>
              <p class="card-text"><strong>Score:</strong> ${attempt.score}</p>
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

        // Display quizzes
        const quizzesList = document.getElementById("quizzes-list");
        data.quizzes.forEach((quiz) => {
          const quizItem = document.createElement("div");
          quizItem.className = "card mb-3";
          quizItem.innerHTML = `
            <div class="card-body">
              <h5 class="card-title">${quiz.topic}</h5>
              <p class="card-text"><strong>Description:</strong> ${
                quiz.description
              }</p>
              <p class="card-text"><strong>Duration:</strong> ${
                quiz.duration
              } hours</p>
              <p class="card-text"><strong>Start Time:</strong> ${new Date(
                quiz.startTime
              ).toLocaleString()}</p>
              <p class="card-text"><strong>End Time:</strong> ${new Date(
                quiz.endTime
              ).toLocaleString()}</p>
              <h6 class="card-subtitle mb-2 text-muted">Questions:</h6>
              ${quiz.questions
                .map(
                  (question) => `
                <div class="mb-2">
                  <p><strong>Q${question.questionId}:</strong> ${
                    question.questionText
                  }</p>
                  <ul>
                    ${question.options
                      .map(
                        (option) => `
                      <li>
                        ${option.optionText} ${
                          option.isAnswer ? "(Correct Answer)" : ""
                        }
                      </li>
                    `
                      )
                      .join("")}
                  </ul>
                </div>
              `
                )
                .join("")}
            </div>
          `;
          quizzesList.appendChild(quizItem);
        });
      }
    })
    .catch((error) => {
      console.error("Error fetching profile:", error);
    });

  document
    .getElementById("reset-password")
    .addEventListener("click", function () {
      window.location.href = "../html/resetPassword.html";
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
