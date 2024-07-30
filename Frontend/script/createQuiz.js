document.addEventListener("DOMContentLoaded", function () {
  // Check for userId, token, and email in local storage
  const token = localStorage.getItem("token");
  const email = localStorage.getItem("email");
  const userId = localStorage.getItem("userId");

  if (!userId || !token || !email) {
    window.location.href = "/login.html"; // Redirect to login page
  }

  // Theme toggle
  document
    .getElementById("theme-toggle")
    .addEventListener("change", function () {
      const sunIcon = document.getElementById("sun-icon");
      const moonIcon = document.getElementById("moon-icon");

      if (this.checked) {
        document.body.classList.add("dark-mode");
        sunIcon.src = "../public/icon-sun-light.svg";
        moonIcon.src = "../public/icon-moon-dark.svg";
      } else {
        document.body.classList.remove("dark-mode");
        sunIcon.src = "../public/icon-sun-dark.svg";
        moonIcon.src = "../public/icon-moon-light.svg";
      }
    });

  // Check local storage for theme preference
  const isDarkMode = localStorage.getItem("darkMode") === "true";
  document.getElementById("theme-toggle").checked = isDarkMode;
  if (isDarkMode) {
    document.body.classList.add("dark-mode");
    document.getElementById("sun-icon").src = "../public/icon-sun-light.svg";
    document.getElementById("moon-icon").src = "../public/icon-moon-dark.svg";
  }

  // Save theme preference to local storage
  document
    .getElementById("theme-toggle")
    .addEventListener("change", function () {
      localStorage.setItem("darkMode", this.checked);
    });

  // Fetch userId from local storage and set the hidden input
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

      // Remove the correct answers input and replace it with multiple inputs
      const correctAnswersDiv =
        questionTemplate.querySelector(".correct-answers");
      correctAnswersDiv.innerHTML = ""; // Clear existing content

      // Create multiple input fields for correct answers
      for (let i = 0; i < 2; i++) {
        const input = document.createElement("input");
        input.type = "text";
        input.className = "form-control mb-2";
        input.name = "correctAnswers"; // Keep the same name for easy retrieval
        input.required = true;
        correctAnswersDiv.appendChild(input);
      }

      // Add the remove button only for questions added after the first one
      const removeButton = document.createElement("button");
      removeButton.type = "button";
      removeButton.className = "btn btn-danger remove-question";
      removeButton.textContent = "Remove Question";
      removeButton.addEventListener("click", function () {
        questionTemplate.remove(); // Remove the question template
      });

      questionTemplate.appendChild(correctAnswersDiv);
      questionTemplate.appendChild(removeButton); // Append the remove button
      document
        .getElementById("questions-container")
        .appendChild(questionTemplate);
    });

  // Submit form
  document
    .getElementById("quiz-form")
    .addEventListener("submit", function (event) {
      event.preventDefault();

      const topic = document.getElementById("topic").value;
      const description = document.getElementById("description").value;
      const duration = parseInt(document.getElementById("duration").value);
      const questions = document.querySelectorAll(".question");

      // Validation checks
      if (!topic) {
        showToast("Topic cannot be empty.", "error");
        return; // Prevent form submission
      }

      if (!description) {
        showToast("Description cannot be empty.", "error");
        return; // Prevent form submission
      }

      if (duration <= 0) {
        showToast("Duration must be greater than 0.", "error");
        return; // Prevent form submission
      }

      for (const question of questions) {
        const options = Array.from(
          question.querySelectorAll("[name='options']")
        ).map((option) => option.value);
        const correctAnswers = Array.from(
          question.querySelectorAll("[name='correctAnswers']")
        )
          .map((answer) => answer.value)
          .filter((value) => value);

        // Check if options and correct answers are empty
        if (options.some((option) => !option)) {
          showToast("Options cannot be empty.", "error");
          return; // Prevent form submission
        }

        if (correctAnswers.length === 0) {
          showToast("Correct answers cannot be empty.", "error");
          return; // Prevent form submission
        }

        // Check if correct answers are present in options
        if (!correctAnswers.every((answer) => options.includes(answer))) {
          showToast(
            "All correct answers must be present in the options list.",
            "error"
          );
          return; // Prevent form submission
        }
      }

      const startTime = new Date(document.getElementById("startTime").value);
      const endTime = new Date(document.getElementById("endTime").value);
      const currentTime = new Date();

      function showToast(message, type) {
        const toastBody = document
          .getElementById(type === "success" ? "success-toast" : "error-toast")
          .querySelector(".toast-body");
        toastBody.textContent = message;
        const toast = new bootstrap.Toast(
          document.getElementById(
            type === "success" ? "success-toast" : "error-toast"
          )
        );
        toast.show();
      }

      if (startTime < currentTime) {
        showToast(
          "Start time must be greater than or equal to the current time.",
          "error"
        );
        return; // Prevent form submission
      }

      if (endTime <= startTime) {
        showToast("End time must be greater than start time.", "error");
        return; // Prevent form submission
      }

      const quiz = {
        userId: parseInt(document.getElementById("userId").value), // Ensure userId is a number
        topic: document.getElementById("topic").value,
        description: document.getElementById("description").value,
        duration: parseInt(document.getElementById("duration").value),
        durationPerQuestion: document.getElementById("durationPerQuestion")
          .checked, // Get checkbox value
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
        )
          .map((answer) => answer.value)
          .filter((value) => value); // Filter out empty values

        quiz.questions.push({ questionText, options, correctAnswers });
      });

      if (quiz.type === "private") {
        quiz.allowedUsers = document
          .getElementById("allowedUsers")
          .value.split(",")
          .map((email) => email.trim()); // Trim whitespace from emails
      }

      // Show spinner
      const createQuizBtn = document.getElementById("create-quiz-btn");
      const spinner = createQuizBtn.querySelector(".spinner-border");
      createQuizBtn.disabled = true; // Disable button
      spinner.classList.remove("d-none"); // Show spinner

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
            showToast(
              `Quiz created successfully! Quiz ID: ${data.quizId}`,
              "success"
            );
          } else {
            showToast(`Error: ${data.message}`, "error");
          }
        })
        .catch((error) => {
          console.error("Error:", error);
          showToast("An error occurred while creating the quiz.", "error");
        })
        .finally(() => {
          // Hide spinner and enable button
          spinner.classList.add("d-none");
          createQuizBtn.disabled = false; // Re-enable button
        });
    });
});
