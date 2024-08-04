// Constants
const IP = "https://quizbackend.raghavendiran.cloud";

// Utility functions
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

const redirectToLogin = (message) => {
  showToast(message, "error");
  setTimeout(() => {
    window.location.href = "../index.html";
  }, 1000);
};

const fetchProfile = async (userId, token) => {
  try {
    const apiUrl = `${IP}/api/Profile/view-profile?userId=${userId}`;
    const response = await fetch(apiUrl, {
      headers: {
        accept: "text/plain",
        Authorization: `Bearer ${token}`,
      },
    });
    return await response.json();
  } catch (error) {
    console.error("Error fetching profile:", error);
    throw error;
  }
};

// DOM Manipulation functions
const displayProfileInfo = (data) => {
  document.getElementById("email").textContent = data.email;
  document.getElementById("name").textContent = data.name;
};

const createAttemptItem = (attempt) => {
  const listItem = document.createElement("div");
  listItem.className = "card mb-3";
  listItem.innerHTML = `
    <div class="card-body text-center">
      <h5 class="card-title font-weight-bold">Attempt ID: ${
        attempt.attemptId
      } | Quiz ID: ${attempt.quizId}</h5>
      <p class="card-text"><strong>Score:</strong> <span class="${
        attempt.score >= 80 ? "text-success" : "text-danger"
      }">${attempt.score}</span> - <strong>Status:</strong> <span class="${
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
  return listItem;
};

const displayAttempts = (attempts) => {
  const attemptsList = document.getElementById("attempts-list");
  attempts.forEach((attempt) => {
    attemptsList.appendChild(createAttemptItem(attempt));
  });
};

const createQuizItem = (quiz) => {
  const quizItem = document.createElement("div");
  quizItem.className = "card mb-3 shadow-sm";

  quizItem.innerHTML = `
    <div class="card-body">
      <h5 class="card-title font-weight-bold">${quiz.topic}</h5>
      <p class="card-text"><strong>Duration:</strong> <span class="quiz-duration">${
        quiz.duration
      }</span> hours</p>
      <p class="card-text"><strong>Start Time:</strong> <span class="quiz-start-time">${new Date(
        quiz.startTime
      ).toLocaleString()}</span></p>
      <p class="card-text"><strong>End Time:</strong> <span class="quiz-end-time">${new Date(
        quiz.endTime
      ).toLocaleString()}</span></p>
      <button class="btn btn-primary edit-btn">Edit</button>
      <button class="btn btn-success update-btn" style="display: none;">Update</button>
    </div>
  `;

  const editBtn = quizItem.querySelector(".edit-btn");
  const updateBtn = quizItem.querySelector(".update-btn");
  const durationElem = quizItem.querySelector(".quiz-duration");
  const startTimeElem = quizItem.querySelector(".quiz-start-time");
  const endTimeElem = quizItem.querySelector(".quiz-end-time");

  editBtn.addEventListener("click", () => {
    durationElem.innerHTML = `<input type="number" class="form-control" value="${quiz.duration}">`;
    startTimeElem.innerHTML = `<input type="datetime-local" class="form-control" value="${new Date(
      quiz.startTime
    )
      .toISOString()
      .slice(0, 16)}">`;
    endTimeElem.innerHTML = `<input type="datetime-local" class="form-control" value="${new Date(
      quiz.endTime
    )
      .toISOString()
      .slice(0, 16)}">`;
    editBtn.style.display = "none";
    updateBtn.style.display = "inline-block";
  });

  updateBtn.addEventListener("click", async () => {
    const updatedQuiz = {
      ...quiz,
      duration: quizItem.querySelector(".quiz-duration input").value,
      startTime: new Date(
        quizItem.querySelector(".quiz-start-time input").value
      ).toISOString(),
      endTime: new Date(
        quizItem.querySelector(".quiz-end-time input").value
      ).toISOString(),
    };
    showToast("Quiz updated successfully", "success");
    durationElem.innerHTML = `${updatedQuiz.duration} hours`;
    startTimeElem.innerHTML = new Date(updatedQuiz.startTime).toLocaleString();
    endTimeElem.innerHTML = new Date(updatedQuiz.endTime).toLocaleString();
    editBtn.style.display = "inline-block";
    updateBtn.style.display = "none";
    // try {
    //   const response = await fetch("/update-quiz-endpoint", {
    //     method: "POST",
    //     headers: {
    //       "Content-Type": "application/json",
    //     },
    //     body: JSON.stringify(updatedQuiz),
    //   });

    //   if (response.ok) {

    //   } else {
    //     showToast("Failed to update quiz", "error");
    //     alert("Failed to update quiz");
    //   }
    // } catch (error) {
    //   alert("Error updating quiz");
    // }
  });

  return quizItem;
};

const displayQuizzes = (quizzes) => {
  const quizzesList = document.getElementById("quizzes-list");
  quizzes.forEach((quiz) => {
    quizzesList.appendChild(createQuizItem(quiz));
  });
};

// Event listeners
const addEventListeners = () => {
  document.getElementById("reset-password").addEventListener("click", () => {
    window.location.href = "../html/resetPassword.html";
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
    .addEventListener("click", () => {
      localStorage.clear();
      window.location.href = "./index.html";
    });
};

// Main function
const main = async () => {
  const isDark = localStorage.getItem("isDark") === "true";
  const themeToggle = document.getElementById("theme-toggle");
  if (isDark) {
    applyDarkMode(true);
    themeToggle.checked = true;
  }

  document
    .getElementById("theme-toggle")
    .addEventListener("change", toggleTheme);
  const userId = localStorage.getItem("userId");
  const token = localStorage.getItem("token");

  if (!userId || !token) {
    redirectToLogin("User ID or token not found. Please log in again.");
    setTimeout(() => {
      window.location.href = "/index.html";
    }, 1000);
  }

  try {
    const data = await fetchProfile(userId, token);
    if (data.errorcode) {
      showToast(`Error: ${data.message}`, "error");
    } else {
      displayProfileInfo(data);
      displayAttempts(data.attempts);
      displayQuizzes(data.quizzes);
    }
  } catch (error) {
    showToast("An error occurred while fetching profile.", "error");
  }

  document.getElementById("filter-select").value = "least-recent";
  document.getElementById("filter-select").dispatchEvent(new Event("change"));

  addEventListeners();
};

document.addEventListener("DOMContentLoaded", main);

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
