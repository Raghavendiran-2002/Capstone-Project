const IP ="https://quizbackend.raghavendiran.cloud"

document.getElementById("theme-toggle").addEventListener("change", function () {
  if (this.checked) {
    document.body.classList.add("dark-mode");
  } else {
    document.body.classList.remove("dark-mode");
  }
});

document.getElementById("start-quiz").addEventListener("click", function () {
  const quizCode = document.getElementById("quiz-code").value;
  const email = "abcd@example.com";
  //const email = localStorage.getItem("email");
  //735889
  if (quizCode && email) {
    fetch(`${IP}/api/Quiz/attend-quiz`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJuYmYiOjE3MjE4MjEwMzAsImV4cCI6MTcyMjQyNTgzMCwiaWF0IjoxNzIxODIxMDMwfQ.Zzf4LjLhQAVRJiutJRpu2H4NTsZNvnhvV8o8L9NZfCI",
      },
      body: JSON.stringify({
        code: quizCode,
        quizId: 3, // Assuming the code is the quiz ID
        email: "abcd@example.com",
      }),
    })
      .then((response) => response.json())
      .then((data) => {
        console.log(data)
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

function startQuiz(data) {
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
      submitQuiz(duration);
    }
  }, 1000);

  const quizContent = document.getElementById("quiz-content");
  quizContent.innerHTML = "";
  data.questionDto.forEach((question, index) => {
    const questionDiv = document.createElement("div");
    questionDiv.classList.add("question");
    questionDiv.innerHTML = `
              <h3>${question.questionText}</h3>
              ${question.options
                .map(
                  (option) => `
                  <div>
                      <input type="radio" name="question-${index}" value="${option.optionText}">
                      <label>${option.optionText}</label>
                  </div>
              `
                )
                .join("")}
          `;
    quizContent.appendChild(questionDiv);
  });

  const submitButton = document.createElement("button");
  submitButton.textContent = "Submit";
  submitButton.classList.add("btn", "btn-primary", "mt-3");
  submitButton.addEventListener("click", submitQuiz);
  quizContent.appendChild(submitButton);
}

function submitQuiz(duration) {
  const email = "abcd@example.com";
  //const email = localStorage.getItem("email");
  const quizId = document.getElementById("quiz-code").value;
  const startTime = new Date().toISOString();
  const endTime = new Date(
    new Date().getTime() + duration * 60 * 1000
  ).toISOString();

  const answers = Array.from(document.querySelectorAll(".question")).map(
    (questionDiv, index) => {
      const selectedAnswer = questionDiv.querySelector(
        'input[type="radio"]:checked'
      );
      return {
        questionId: index,
        selectedAnswers: [selectedAnswer ? selectedAnswer.value : ""],
      };
    }
  );

  fetch(`${IP}/api/submit-test`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: "Bearer YOUR_JWT_TOKEN",
    },
    body: JSON.stringify({
      quizId: parseInt(quizId),
      emailId: email,
      startTime: startTime,
      endTime: endTime,
      answers: answers,
    }),
  })
    .then((response) => response.json())
    .then((data) => {
      alert("Quiz submitted successfully!");
      window.location.reload();
    })
    .catch((error) => console.error("Error:", error));
}
