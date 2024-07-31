const IP = "https://quizbackend.raghavendiran.cloud";
document.addEventListener("DOMContentLoaded", () => {
  document
    .getElementById("theme-toggle")
    .addEventListener("change", function () {
      if (this.checked) {
        document.body.classList.add("dark-mode");
      } else {
        document.body.classList.remove("dark-mode");
      }
    });

  const quizForm = document.getElementById("quizForm");
  const typeSelect = document.getElementById("type");
  const allowedUsersContainer = document.getElementById(
    "allowedUsersContainer"
  );
  const allowedUsersList = document.getElementById("allowedUsersList");
  const addAllowedUserButton = document.getElementById("addAllowedUser");
  const questionsContainer = document.getElementById("questionsContainer");
  const addQuestionButton = document.getElementById("addQuestion");
  let questionIndex = 1;

  const userId = localStorage.getItem("userId"); // Fetch userId from local storage

  typeSelect.addEventListener("change", function () {
    if (this.value === "private") {
      allowedUsersContainer.style.display = "block";
    } else {
      allowedUsersContainer.style.display = "none";
    }
  });

  addAllowedUserButton.addEventListener("click", () => {
    const input = document.createElement("input");
    input.type = "email";
    input.className = "form-control mb-2 allowedUser";
    input.placeholder = "User Email";
    allowedUsersList.appendChild(input);
  });

  addQuestionButton.addEventListener("click", () => {
    const questionDiv = document.createElement("div");
    questionDiv.className = "question mb-4";
    questionDiv.dataset.questionIndex = questionIndex;

    questionDiv.innerHTML = `
            <div class="mb-3">
                <label for="questionText_${questionIndex}" class="form-label">Question Text</label>
                <input type="text" class="form-control questionText" id="questionText_${questionIndex}" required>
            </div>
            <div class="mb-3">
                <label for="options_${questionIndex}" class="form-label">Options</label>
                <div class="optionsList" id="options_${questionIndex}">
                    ${createOptionInput()}
                    ${createOptionInput()}
                    ${createOptionInput()}
                    ${createOptionInput()}
                </div>
                <button type="button" class="btn btn-secondary mt-2 addOption">Add Option</button>
            </div>
            <div class="mb-3">
                <label for="correctAnswers_${questionIndex}" class="form-label">Correct Answers</label>
                <input type="text" class="form-control correctAnswers" id="correctAnswers_${questionIndex}" placeholder="Correct Answer(s)" required>
            </div>
            <button type="button" class="btn btn-danger removeQuestion">Remove Question</button>
        `;

    questionsContainer.appendChild(questionDiv);
    questionIndex++;
  });

  quizForm.addEventListener("click", (event) => {
    if (event.target.classList.contains("addOption")) {
      const optionsList = event.target.previousElementSibling;
      optionsList.insertAdjacentHTML(
        "beforeend",
        createOptionInputWithRemoveBtn()
      );
    } else if (event.target.classList.contains("removeOption")) {
      event.target.parentElement.remove();
    } else if (event.target.classList.contains("removeQuestion")) {
      event.target.closest(".question").remove();
    }
  });

  quizForm.addEventListener("submit", async (event) => {
    event.preventDefault();

    const requiredFields = [
      "topic",
      "description",
      "imageURL",
      "duration",
      "startTime",
      "endTime",
      "type",
    ];
    let isValid = true;

    // Check each required field in order
    for (const fieldId of requiredFields) {
      const field = document.getElementById(fieldId);
      if (!field.value) {
        isValid = false;
        showToast(
          `Please fill out the ${field.labels[0].innerText} field.`,
          "danger"
        );
        field.focus(); // Set focus to the first invalid field
        break; // Stop checking further fields
      }
    }

    if (!isValid) return; // Stop submission if validation fails

    // Show spinner
    const submitButton = document.getElementById("submitButton");
    const spinner = submitButton.querySelector(".spinner-border");
    submitButton.disabled = true; // Disable button
    spinner.style.display = "inline-block"; // Show spinner

    const data = {
      userId: parseInt(userId), // Use userId from local storage
      topic: document.getElementById("topic").value,
      description: document.getElementById("description").value,
      imageURL: document.getElementById("imageURL").value,
      duration: parseInt(document.getElementById("duration").value),
      durationPerQuestion: document.getElementById("durationPerQuestion")
        .checked,
      startTime: document.getElementById("startTime").value,
      endTime: document.getElementById("endTime").value,
      type: document.getElementById("type").value,
      questions: [],
      allowedUsers: [],
    };

    document.querySelectorAll(".question").forEach((questionDiv) => {
      const questionIndex = questionDiv.dataset.questionIndex;
      const questionText = questionDiv.querySelector(".questionText").value;
      const options = Array.from(questionDiv.querySelectorAll(".option")).map(
        (input) => input.value
      );
      const correctAnswers = questionDiv
        .querySelector(".correctAnswers")
        .value.split(",");

      data.questions.push({
        questionText,
        options,
        correctAnswers,
      });
    });

    document.querySelectorAll(".allowedUser").forEach((input) => {
      data.allowedUsers.push(input.value);
    });

    try {
      const token = localStorage.getItem("token"); // Load token from local storage
      const response = await fetch(`${IP}/api/Quiz/create-quiz`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`, // Use the token from local storage
        },
        body: JSON.stringify(data),
      });

      // Hide spinner
      spinner.style.display = "none"; // Hide spinner
      submitButton.disabled = false; // Re-enable button

      if (response.ok) {
        showToast("Quiz created successfully!", "success");
        // Show additional toast message indicating the quiz has been uploaded
        showToast("Your quiz has been uploaded!", "success");

        // Clear the form
        quizForm.reset(); // Reset the form fields

        // Clear dynamically added questions and allowed users
        questionsContainer.innerHTML = ""; // Clear questions
        allowedUsersList.innerHTML = ""; // Clear allowed users
        questionIndex = 1; // Reset question index
      } else {
        const errorText = await response.text();
        showToast(`Error: ${errorText}`, "danger");
      }
    } catch (error) {
      // Hide spinner
      spinner.style.display = "none"; // Hide spinner
      submitButton.disabled = false; // Re-enable button
      showToast(`Error: ${error.message}`, "danger");
    }
  });
  function createOptionInputWithRemoveBtn() {
    return `
            <div class="input-group mb-2">
                <input type="text" class="form-control option" placeholder="Option" required>
                <button type="button" class="btn btn-danger removeOption">Remove</button>
            </div>
        `;
  }
  function createOptionInput() {
    return `
            <div class="input-group mb-2">
                <input type="text" class="form-control option" placeholder="Option" required>
                
            </div>
        `;
  }

  function showToast(message, type) {
    const toastElement = document.getElementById("toast");
    const toastBody = toastElement.querySelector(".toast-body");
    toastBody.textContent = message;

    toastElement.classList.remove("bg-success", "bg-danger");
    toastElement.classList.add(`bg-${type}`);
    const toast = new bootstrap.Toast(toastElement);
    toast.show();
  }
});
