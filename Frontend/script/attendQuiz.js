const IP = "https://quizbackend.raghavendiran.cloud";
const bearer =
  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJuYmYiOjE3MjIyMjU3OTQsImV4cCI6MTcyMjgzMDU5NCwiaWF0IjoxNzIyMjI1Nzk0fQ.jR1x1_c95UOPTRVtSytdXNTuHdkeL5SG4jMYt70bxdo";

document.getElementById("theme-toggle").addEventListener("change", function () {
  if (this.checked) {
    document.body.classList.add("dark-mode");
  } else {
    document.body.classList.remove("dark-mode");
  }
});

document.getElementById("start-quiz").addEventListener("click", function () {
  const quizCode = document.getElementById("quiz-code").value;
  //  const email = localStorage.getItem("email");
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

function startQuiz(data) {
  quizData = data;
  document.getElementById("quiz-details").classList.add("d-none");
  document.getElementById("quiz-container").classList.remove("d-none");

  const timerElement = document.getElementById("timer");
  let duration = data.duration * 60; // duration in seconds

  const interval = setInterval(() => {
    let minutes = Math.floor(duration / 60);
    let seconds = duration % 60;

    timerElement.textContent = `${minutes}:${
      seconds < 10 ? "0" : ""
    }${seconds}`;
    if (--duration < 0) {
      clearInterval(interval);
      submitQuiz(data.quizId, data.startTime);
    }
  }, 1000);

  showQuestion(currentQuestionIndex);
}

function showQuestion(index) {
  const quizContent = document.getElementById("quiz-content");
  quizContent.innerHTML = "";

  const question = quizData.questionDto[index];
  const questionDiv = document.createElement("div");
  questionDiv.classList.add("question");
  questionDiv.innerHTML = `
    <h3>${question.questionText}</h3>
    ${question.options
      .map(
        (option) => `
          <div>
              <input type="radio" name="question-${question.questionId}" value="${option.optionText}">
              <label>${option.optionText}</label>
          </div>
      `
      )
      .join("")}
  `;
  quizContent.appendChild(questionDiv);

  const buttonContainer = document.createElement("div");
  buttonContainer.classList.add("d-flex", "justify-content-between", "mt-3");

  if (index > 0) {
    const prevButton = document.createElement("button");
    prevButton.textContent = "Previous";
    prevButton.classList.add("btn", "btn-secondary");
    prevButton.addEventListener("click", () => {
      saveAnswer(index);
      showQuestion(index - 1);
    });
    buttonContainer.appendChild(prevButton);
  }

  if (index < quizData.questionDto.length - 1) {
    const nextButton = document.createElement("button");
    nextButton.textContent = "Next";
    nextButton.classList.add("btn", "btn-primary");
    nextButton.addEventListener("click", () => {
      saveAnswer(index);
      showQuestion(index + 1);
    });
    buttonContainer.appendChild(nextButton);
  } else {
    const submitButton = document.createElement("button");
    submitButton.textContent = "Submit";
    submitButton.classList.add("btn", "btn-primary");
    submitButton.addEventListener("click", () => {
      saveAnswer(index);
      submitQuiz(quizData.quizId, quizData.startTime);
    });
    buttonContainer.appendChild(submitButton);
  }

  quizContent.appendChild(buttonContainer);
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
      if (data.status)
        alert(
          `Quiz submitted successfully! ${data.status} + score: ${data.score}`
        );
      window.location.reload();
    })
    .catch((error) => console.error("Error:", error));
}
