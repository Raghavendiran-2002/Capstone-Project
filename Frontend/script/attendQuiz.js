const IP = "https://quizbackend.raghavendiran.cloud";
//const IP = "http://127.0.0.1:8000";
document.addEventListener("DOMContentLoaded", init);

// Initialize event listeners
function initEventListeners() {
  document
    .getElementById("theme-toggle")
    .addEventListener("change", toggleTheme);
  document
    .getElementById("start-quiz")
    .addEventListener("click", startQuizHandler);
}

// Toggle theme function
function toggleTheme() {
  const sunIcon = document.getElementById("sun-icon");
  const moonIcon = document.getElementById("moon-icon");
  if (this.checked) {
    document.body.classList.add("dark-mode");
    sunIcon.src = "../public/icon-sun-light.svg";
    moonIcon.src = "../public/icon-moon-light.svg";
    document.querySelectorAll(".question h3, .question p").forEach((el) => {
      el.classList.add("light-text");
    });
  } else {
    document.body.classList.remove("dark-mode");
    sunIcon.src = "../public/icon-sun-dark.svg";
    moonIcon.src = "../public/icon-moon-dark.svg";
    document.querySelectorAll(".question h3, .question p").forEach((el) => {
      el.classList.remove("light-text");
    });
  }
}

// Start quiz handler
function startQuizHandler() {
  window.addEventListener("blur", handleBlur);
  const quizCode = localStorage.getItem("quizCode");
  var email = localStorage.getItem("email");
  var quizId = localStorage.getItem("quizId");

  if (quizCode && email) {
    const startButton = this;
    setLoadingState(startButton);
    fetchQuizData(quizCode, email, quizId, startButton);
  } else {
    showToast("Please enter the quiz code.", "error");
  }
}
function init() {
  var token = localStorage.getItem("token");
  var quizid = localStorage.getItem("quizId");
  if (quizid === null || quizid === "") {
    showToast("Please select a quiz to continue.", "error"); // Updated toast message
    setTimeout(() => {
      window.location.href = "../html/quizzes.html"; // Redirect to login page after 2 seconds
    }, 2000);
  }
  if (token === null || token === "") {
    showToast("Unauthorized access. Please log in.", "error"); // Added toast for unauthorized access
    setTimeout(() => {
      window.location.href = "../html/login.html"; // Redirect to login page after 2 seconds
    }, 2000);
  }

  // {{ edit_1 }}
  var quizCode = localStorage.getItem("quizCode");
  var quizCodeInput = document.getElementById("quiz-code");
  if (quizCode) {
    quizCodeInput.classList.add("d-none"); // Hide quiz code input if quizCode is available
  } else {
    quizCodeInput.classList.remove("d-none"); // Show quiz code input if quizCode is not available
  }
  // {{ edit_2 }}
}
// Set loading state for the button
function setLoadingState(button) {
  button.innerHTML = `<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Starting...`;
  button.disabled = true; // Disable the button
}

// Fetch quiz data from the server
function fetchQuizData(quizCode, email, quizId, startButton) {
  var token = localStorage.getItem("token");
  fetch(`${IP}/api/Quiz/attend-quiz`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      code: quizCode,
      quizId: parseInt(quizId),
      email: email,
    }),
  })
    .then((response) => response.json())
    .then((data) => {
      if (data.quizId) {
        startQuiz(data);
      } else {
        showToast(data.message, "error");
        if (data.message === "User not allowed to attend this quiz") {
          setTimeout(() => {
            window.location.href = "../html/quizzes.html";
          }, 1000);
        }
        resetButton(startButton);
      }
    })
    .catch((error) => {
      showToast(error, "error");
      resetButton(startButton);
    });
}

// Reset button to its original state
function resetButton(button) {
  button.innerHTML = "Start Quiz";
  button.disabled = false;
}

// Start the quiz
let quizData;
let currentQuestionIndex = 0;
let warningCount = 0;
let questionInterval; // Declare questionInterval globally

function startQuiz(data) {
  document.getElementById("quiz-instructions").classList.add("d-none");
  quizData = data;
  var startTime = new Date().toISOString();
  setupBackButtonWarning(startTime);
  document.getElementById("quiz-details").classList.add("d-none");
  document.getElementById("quiz-container").classList.remove("d-none");
  startTimer(startTime);
  showQuestion(currentQuestionIndex, startTime, data.durationPerQuestion);
}

// Setup back button warning
function setupBackButtonWarning(startTime) {
  history.pushState(null, null, location.href);
  window.onpopstate = function () {
    warningCount++;
    if (warningCount < 3) {
      showToast(
        `Warning ${warningCount}: Pressing the back button will terminate the quiz.`,
        "error"
      );
      history.pushState(null, null, location.href); // Prevent going back
    } else {
      showToast(
        "You have pressed the back button too many times. The quiz will now be terminated.",
        "error"
      );
      submitQuiz(quizData.quizId, startTime); // Terminate the quiz
    }
  };
}

// Start the timer
function startTimer(startTime) {
  const progressBar = document.getElementById("progress-bar");
  const timerElement = document.getElementById("timer");
  let duration = quizData.duration * 60;
  let totalTime = duration;

  const interval = setInterval(() => {
    duration--;
    const progress = ((totalTime - duration) / totalTime) * 100;
    progressBar.style.width = `${progress}%`;
    progressBar.setAttribute("aria-valuenow", progress);

    const minutes = Math.floor(duration / 60);
    const seconds = duration % 60;
    timerElement.textContent = `Time Left : ${minutes}:${
      seconds < 10 ? "0" : ""
    }${seconds}`;

    if (duration <= 0) {
      clearInterval(interval);
      submitQuiz(quizData.quizId, startTime);
    }
  }, 1000);
}

// Show question
function showQuestion(index, startTime, durationPerQuestion) {
  if (durationPerQuestion) {
    resetTimer();
  }
  const quizContent = document.getElementById("quiz-content");
  quizContent.innerHTML = "";

  const question = quizData.questionDto[index];
  const questionDiv = document.createElement("div");
  questionDiv.classList.add("question");

  questionDiv.innerHTML = `
    <h3 class="mb-4">Question ${index + 1} of ${
    quizData.questionDto.length
  }</h3>
    <p class="mb-4">${question.questionText}</p>
    <div class="list-group">
      ${question.options
        .map(
          (option, i) => `
        <label class="list-group-item d-flex align-items-center">
          <input type="radio" name="question-${question.questionId}" value="${
            option.optionText
          }" class="me-2">
          <span>${String.fromCharCode(65 + i)}. ${option.optionText}</span>
        </label>
      `
        )
        .join("")}
    </div>
  `;

  const buttonContainer = document.createElement("div");
  buttonContainer.classList.add("d-flex", "justify-content-end", "mt-4");

  if (index < quizData.questionDto.length - 1) {
    const nextButton = createButton("Next", () => {
      saveAnswer(index);
      showQuestion(index + 1, startTime, durationPerQuestion);
    });
    buttonContainer.appendChild(nextButton);
  } else {
    const submitButton = createButton("Submit", () => {
      saveAnswer(index);
      submitButton.innerHTML = `<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Submitting...`;
      submitButton.disabled = true; // Disable the button
      submitQuiz(quizData.quizId, startTime);
    });
    buttonContainer.appendChild(submitButton);
  }

  questionDiv.appendChild(buttonContainer);
  quizContent.appendChild(questionDiv);
}

// Create a button
function createButton(text, onClick) {
  const button = document.createElement("button");
  button.textContent = text;
  button.classList.add("btn", "btn-primary");
  button.addEventListener("click", onClick);
  return button;
}
function handleBlur() {
  warningCount++;
  showToast(
    `Warning ${warningCount}: You have clicked out of the browser. Please stay focused on the quiz.`,
    "error"
  );
  if (warningCount == 3) {
    showToast("Last warning", "error");
  }
  if (warningCount >= 4) {
    showToast(
      "You have violated the rules. The quiz will be terminated.",
      "error"
    );
    window.location.href = "../html/quizzes.html";
  }

  window.history.pushState(null, null, window.location.href); // Disable back button
}

// Reset timer for the current question
function resetTimer() {
  clearInterval(questionInterval); // Clear any existing interval
  let questionDuration = quizData.duration * 60; // duration per question in seconds
  const timerElement = document.getElementById("timer");
  const progressBar = document.getElementById("progress-bar");
  const totalTime = questionDuration; // Store the total time for progress calculation

  questionInterval = setInterval(() => {
    const progress = ((totalTime - questionDuration) / totalTime) * 100;
    progressBar.style.width = `${progress}%`;
    progressBar.setAttribute("aria-valuenow", progress);

    const minutes = Math.floor(questionDuration / 60);
    const seconds = questionDuration % 60;
    timerElement.textContent = `${minutes}:${
      seconds < 10 ? "0" : ""
    }${seconds}`;

    questionDuration--; // Decrement the duration

    if (questionDuration < 0) {
      clearInterval(questionInterval);
      saveAnswer(currentQuestionIndex); // Save answer before moving to the next question
      showQuestion(
        currentQuestionIndex + 1,
        startTime,
        quizData.durationPerQuestion
      ); // Move to the next question when time is up
    }
  }, 1000);
}

// Save answer
function saveAnswer(index) {
  const question = quizData.questionDto[index];
  const selectedAnswers = Array.from(
    document.querySelectorAll(
      `input[name="question-${question.questionId}"]:checked`
    )
  ).map((input) => input.value);
  quizData.questionDto[index].selectedAnswer = selectedAnswers; // Store multiple answers
}

// Submit quiz
function submitQuiz(quizId, startTime) {
  window.removeEventListener("blur", handleBlur);
  const email = localStorage.getItem("email");
  const endTime = new Date().toISOString();

  const answers = quizData.questionDto.map((question) => ({
    questionId: question.questionId,
    selectedAnswers: question.selectedAnswer,
  }));
  var token = localStorage.getItem("token");
  fetch(`${IP}/api/Quiz/complete-quiz`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      quizId: quizId,
      emailId: email,
      startTime: startTime,
      endTime: endTime,
      answers: answers,
    }),
  })
    .then((response) => response.json())
    .then((data) => {
      if (data.status) {
        const certUrl = data.certUrl; // Assuming certUrl is part of the response
        showToastCompleteQuiz(
          `Quiz submitted successfully! ${data.status} + score: ${data.score}`,
          data.status,
          certUrl
        );
      }
    })
    .catch((error) => {
      console.error("Error:", error);
      showToast("An error occurred while submitting the quiz.", "error");
    });
}

function showToastCompleteQuiz(message, type, certUrl) {
  const modalMessage = document.getElementById("modal-message");
  modalMessage.textContent = message; // Set the message in the modal
  const quizModal = new bootstrap.Modal(document.getElementById("quizModal"));

  const understoodButton = document.getElementById("modal-understood");
  if (type === "pass") {
    understoodButton.textContent = "Download Certificate";
    understoodButton.onclick = () => window.open(certUrl, "_blank"); // Open certificate URL
  } else {
    understoodButton.textContent = "Retry";
    understoodButton.onclick = () => {
      location.reload();
    };
  }

  quizModal.show(); // Show the modal
}

// Show toast messages
function showToast(message, type) {
  const toastBody = document.getElementById("error-message");
  const successToast = document.getElementById("success-toast");
  const errorToast = document.getElementById("error-toast");

  if (type === "success") {
    successToast.querySelector(".toast-body").textContent = message;
    const toast = new bootstrap.Toast(successToast);
    toast.show();
  } else {
    toastBody.textContent = message;
    const toast = new bootstrap.Toast(errorToast);
    toast.show();
  }
}

// Initialize the application
initEventListeners();
