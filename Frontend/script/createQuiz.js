document.addEventListener("DOMContentLoaded", function () {
  // Theme toggle
  document
    .getElementById("theme-toggle")
    .addEventListener("change", function () {
      if (this.checked) {
        document.body.classList.add("dark-mode");
      } else {
        document.body.classList.remove("dark-mode");
      }
    });

  // Fetch userId from local storage and set the hidden input
  const userId = localStorage.getItem("userId");
  document.getElementById("userId").value = userId;

  // Show or hide allowed users field based on quiz type
  document.getElementById("type").addEventListener("change", function () {
    const allowedUsersContainer = document.getElementById(
      "allowed-users-container"
    );
    if (this.value === "private") {
      allowedUsersContainer.classList.remove("d-none");
    } else {
      allowedUsersContainer.classList.add("d-none");
    }
  });

  // Add another question
  document
    .getElementById("add-question")
    .addEventListener("click", function () {
      const questionTemplate = document
        .querySelector(".question")
        .cloneNode(true);
      questionTemplate
        .querySelectorAll("input")
        .forEach((input) => (input.value = ""));
      document
        .getElementById("questions-container")
        .appendChild(questionTemplate);
    });

  // Submit form
  document
    .getElementById("quiz-form")
    .addEventListener("submit", function (event) {
      event.preventDefault();

      const startTime = new Date(document.getElementById("startTime").value);
      const endTime = new Date(document.getElementById("endTime").value);
      const currentTime = new Date();

      if (startTime < currentTime) {
        alert("Start time must be greater than or equal to the current time.");
        return; // Prevent form submission
      }

      if (endTime <= startTime) {
        alert("End time must be greater than start time.");
        return; // Prevent form submission
      }

      const quiz = {
        userId: document.getElementById("userId").value,
        topic: document.getElementById("topic").value,
        description: document.getElementById("description").value,
        duration: parseInt(document.getElementById("duration").value),
        startTime: startTime.toISOString(), // Ensure proper format
        endTime: endTime.toISOString(), // Ensure proper format
        type: document.getElementById("type").value,
        questions: [],
        allowedUsers: [],
      };

      document.querySelectorAll(".question").forEach((question) => {
        const questionText = question.querySelector(
          "[name='questionText']"
        ).value;
        const options = Array.from(
          question.querySelectorAll("[name='options']")
        ).map((option) => option.value);
        const correctAnswers = Array.from(
          question.querySelectorAll("[name='correctAnswers']")
        ).map((answer) => answer.value);

        quiz.questions.push({ questionText, options, correctAnswers });
      });

      if (quiz.type === "private") {
        quiz.allowedUsers = document
          .getElementById("allowedUsers")
          .value.split(",");
      }

      fetch("https://quizbackend.raghavendiran.cloud/api/Quiz/create-quiz", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`, // Assume the token is stored in localStorage
        },
        body: JSON.stringify(quiz),
      })
        .then((response) => response.json())
        .then((data) => {
          if (data.quizId) {
            alert(`Quiz created successfully! Quiz ID: ${data.quizId}`);
          } else {
            alert(`Error: ${data.message}`);
          }
        })
        .catch((error) => {
          console.error("Error:", error);
          alert("An error occurred while creating the quiz.");
        });
    });
});
