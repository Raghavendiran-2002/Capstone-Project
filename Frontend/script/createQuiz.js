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
    }
  });

  quizForm.addEventListener("submit", async (event) => {
    event.preventDefault();

    const data = {
      userId: parseInt(document.getElementById("userId").value),
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
      const response = await fetch(`${IP}/api/Quiz/create-quiz`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization:
            "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJuYmYiOjE3MjIzOTc0MzIsImV4cCI6MTcyMzAwMjIzMiwiaWF0IjoxNzIyMzk3NDMyfQ._qvaTgkFFUlmSPKJcfUNmykrSxIJc_V-cIgyVsaDr-E",
        },
        body: JSON.stringify(data),
      });

      if (response.ok) {
        showToast("Quiz created successfully!", "success");
      } else {
        const errorText = await response.text();
        showToast(`Error: ${errorText}`, "danger");
      }
    } catch (error) {
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
