const IP = "https://quizbackend.raghavendiran.cloud";
//const IP = "http://127.0.0.1:8000";
document.addEventListener("DOMContentLoaded", () => {
  const userId = localStorage.getItem("userId"); // Get userId from localStorage
  const token = localStorage.getItem("token"); // Get token from localStorage

  if (!userId || !token) {
    showToast("User ID or token not found. Please log in again.", "error"); // Show toast instead of alert
    setTimeout(() => {
      window.location.href = "../html/index.html"; // Redirect to index.html after 1 second
    }, 1000);
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
        showToast(`Error: ${data.message}`, "error"); // Show toast instead of alert
      } else {
        document.getElementById("email").textContent = data.email;
        document.getElementById("name").textContent = data.name;

        // Display attempts
        const attemptsList = document.getElementById("attempts-list");
        data.attempts.forEach((attempt) => {
          const listItem = document.createElement("div");
          listItem.className = "card mb-3";
          listItem.innerHTML = `
            <div class="card-body text-center">
              <h5 class="card-title font-weight-bold">Attempt ID: ${
                attempt.attemptId
              } | Quiz ID: ${attempt.quizId}</h5>
              <p class="card-text"><strong>Score:</strong> <span class="${
                attempt.score >= 80 ? "text-success" : "text-danger"
              }">${
            attempt.score
          }</span> - <strong>Status:</strong> <span class="${
            attempt.score >= 80 ? "text-success" : "text-danger"
          }">${attempt.score >= 80 ? "Pass" : "Fail"}</span></p>
              <p class="card-text"><strong>Completed At:</strong> <span class="text-muted">${new Date(
                attempt.completedAt
              ).toLocaleString()}</span></p>
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
          quizItem.className = "card mb-3 shadow-sm"; // Added shadow for depth
          quizItem.innerHTML = `
            <div class="card-body">
              <h5 class="card-title font-weight-bold">${
                quiz.topic
              }</h5> <!-- Bold title -->
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
              <ul class="list-unstyled"> <!-- Changed to list-unstyled for cleaner look -->
                ${quiz.questions
                  .map(
                    (question) => `
                  <li class="mb-3"> <!-- Added margin for spacing -->
                    <p><strong>Q${question.questionId}:</strong> ${
                      question.questionText
                    }</p>
                    <ul>
                      ${question.options
                        .map(
                          (option) => `
                        <li>
                          ${option.optionText} ${
                            option.isAnswer
                              ? "<span class='text-success'>(Correct Answer)</span>"
                              : ""
                          }
                        </li>
                      `
                        )
                        .join("")}
                    </ul>
                  </li>
                `
                  )
                  .join("")}
              </ul>
            </div>
          `;
          quizzesList.appendChild(quizItem);
        });
      }
    })
    .catch((error) => {
      console.error("Error fetching profile:", error);
    });

  // Add this block to apply the 'least-recent' filter on load
  const filterSelect = document.getElementById("filter-select");
  filterSelect.value = "least-recent"; // Set the filter to 'least-recent'
  filterSelect.dispatchEvent(new Event("change")); // Trigger change event to apply filter

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

  document
    .getElementById("filter-select")
    .addEventListener("change", function () {
      const selectedValue = this.value;
      const attemptsList = document.getElementById("attempts-list");
      const attempts = Array.from(attemptsList.children);

      // Sort attempts based on selected filter
      attempts.sort((a, b) => {
        const aDate = new Date(
          a.querySelector(".card-text span.text-muted").textContent
        );
        const bDate = new Date(
          b.querySelector(".card-text span.text-muted").textContent
        );
        return selectedValue === "most-recent" ? bDate - aDate : aDate - bDate;
      });

      // Clear and re-append sorted attempts
      attemptsList.innerHTML = "";
      attempts.forEach((attempt) => attemptsList.appendChild(attempt));
    });

  document
    .querySelector("button[onclick='logout()']")
    .addEventListener("click", function () {
      localStorage.clear(); // Clear local storage
      window.location.href = "../html/index.html"; // Redirect to index.html
    });

  function showToast(message, type) {
    const toastElement =
      type === "error"
        ? document.getElementById("error-toast")
        : document.getElementById("success-toast");
    const toastBody = toastElement.querySelector(".toast-body");
    toastBody.textContent = message;
    const toast = new bootstrap.Toast(toastElement);
    toast.show();
  }
});
