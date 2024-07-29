const IP = "https://quizbackend.raghavendiran.cloud";
const bearer =
  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJuYmYiOjE3MjIyMjU3OTQsImV4cCI6MTcyMjgzMDU5NCwiaWF0IjoxNzIyMjI1Nzk0fQ.jR1x1_c95UOPTRVtSytdXNTuHdkeL5SG4jMYt70bxdo";

document.getElementById("theme-toggle").addEventListener("change", function () {
  if (this.checked) {
    document.body.classList.add("dark-mode");
    // Change question text color to light
    document.querySelectorAll(".question h3, .question p").forEach((el) => {
      el.classList.add("light-text");
    });
  } else {
    document.body.classList.remove("dark-mode");
    // Revert question text color to default
    document.querySelectorAll(".question h3, .question p").forEach((el) => {
      el.classList.remove("light-text");
    });
  }
});

document.getElementById("start-quiz").addEventListener("click", function () {
  const quizCode = document.getElementById("quiz-code").value;
  // const email = localStorage.getItem("email");
  const email = "user@example.com";
  if (quizCode && email) {
    fetch(`${IP}/api/Quiz/attend-quiz`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${bearer}`,
      },
      body: JSON.stringify({
        code: quizCode,
        quizId: 9, // Assuming the code is the quiz ID
        email: email,
      }),
    })
      .then((response) => response.json())
      .then((data) => {
        if (data.quizId) {
          startQuiz(data);
        } else {
          alert("Invalid quiz code. Please try again.");
        }
      })
      .catch((error) => console.error("Error:", error));
  } else {
    alert(
      "Please enter the quiz code and ensure your email is saved in localStorage."
    );
  }
});

let quizData;
let currentQuestionIndex = 0;
let warningCount = 0;
function startQuiz(data) {
  // Disable the back button
  var startTime = new Date().toISOString();
  history.pushState(null, null, location.href);
  window.onpopstate = function () {
    warningCount++;
    if (warningCount < 3) {
      alert(
        `Warning ${warningCount}: Pressing the back button will terminate the quiz.`
      );
      history.pushState(null, null, location.href); // Prevent going back
    } else {
      alert(
        "You have pressed the back button too many times. The quiz will now be terminated."
      );
      submitQuiz(quizData.quizId, startTime); // Terminate the quiz
    }
  };

  quizData = data;
  document.getElementById("quiz-details").classList.add("d-none");
  document.getElementById("quiz-container").classList.remove("d-none");

  const progressBar = document.getElementById("progress-bar");
  const timerElement = document.getElementById("timer"); // Get the timer element
  let duration = data.duration * 60; // duration in seconds
  let totalTime = duration;

  const interval = setInterval(() => {
    duration--;
    const progress = ((totalTime - duration) / totalTime) * 100;
    progressBar.style.width = `${progress}%`;
    progressBar.setAttribute("aria-valuenow", progress);

    const minutes = Math.floor(duration / 60);
    const seconds = duration % 60;
    timerElement.textContent = `${minutes}:${
      seconds < 10 ? "0" : ""
    }${seconds}`;

    if (duration <= 0) {
      clearInterval(interval);
      submitQuiz(data.quizId, startTime);
    }
  }, 1000);

  // Detect when the user clicks out of the browser
  window.addEventListener("blur", handleBlur);
  showQuestion(currentQuestionIndex, startTime);
}

function handleBlur() {
  warningCount++;
  alert(
    `Warning ${warningCount}: You have clicked out of the browser. Please stay focused on the quiz.`
  );
  if (warningCount == 3) {
    alert("Last warniong");
    // submitQuiz(quizData.quizId, quizData.startTime);
  }
  if (warningCount >= 4) {
    alert("You have violated the rules. The quiz will be terminated.");
    window.location.href = "../html/quizzes.html";
  }
}

function showQuestion(index, startTime) {
  const quizContent = document.getElementById("quiz-content");
  quizContent.innerHTML = "";

  const question = quizData.questionDto[index];
  const questionDiv = document.createElement("div");
  questionDiv.classList.add("question");

  // Create the HTML structure for the question and options
  questionDiv.innerHTML = `
    <h3 class="mb-4">Question ${index + 1} of ${
    quizData.questionDto.length
  }</h3>
    <p class="mb-4">${question.questionText}</p>
    <div class="list-group">
      ${question.options
        .map(
          (option, i) => `
        <label class="list-group-item">
          <input type="radio" name="question-${question.questionId}" value="${
            option.optionText
          }">
          ${String.fromCharCode(65 + i)}. ${option.optionText}
        </label>
      `
        )
        .join("")}
    </div>
  `;

  // Append the next or submit button
  const buttonContainer = document.createElement("div");
  buttonContainer.classList.add("d-flex", "justify-content-end", "mt-4");

  if (index < quizData.questionDto.length - 1) {
    const nextButton = document.createElement("button");
    nextButton.textContent = "Next";
    nextButton.classList.add("btn", "btn-primary");
    nextButton.addEventListener("click", () => {
      saveAnswer(index);
      showQuestion(index + 1, startTime);
    });
    buttonContainer.appendChild(nextButton);
  } else {
    const submitButton = document.createElement("button");
    submitButton.textContent = "Submit";
    submitButton.classList.add("btn", "btn-primary");
    submitButton.addEventListener("click", () => {
      saveAnswer(index);
      submitQuiz(quizData.quizId, startTime);
    });
    buttonContainer.appendChild(submitButton);
  }

  questionDiv.appendChild(buttonContainer);
  quizContent.appendChild(questionDiv);
}

function saveAnswer(index) {
  const question = quizData.questionDto[index];
  const selectedAnswer = document.querySelector(
    `input[name="question-${question.questionId}"]:checked`
  );

  quizData.questionDto[index].selectedAnswer = selectedAnswer
    ? selectedAnswer.value
    : "";
}

function submitQuiz(quizId, startTime) {
  window.removeEventListener("blur", handleBlur);
  const email = localStorage.getItem("email");
  const endTime = new Date().toISOString();

  const answers = quizData.questionDto.map((question) => {
    return {
      questionId: question.questionId,
      selectedAnswers: [question.selectedAnswer || ""],
    };
  });

  fetch(`${IP}/api/Quiz/complete-quiz`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${bearer}`,
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
        alert(
          `Quiz submitted successfully! ${data.status} + score: ${data.score}`
        );
        window.location.href = "../html/quizzes.html";
      }
    })
    .catch((error) => console.error("Error:", error));
}
