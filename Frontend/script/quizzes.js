const IP = "https://quizbackend.raghavendiran.cloud";

document.getElementById("theme-toggle").addEventListener("change", function () {
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

document.addEventListener("DOMContentLoaded", function () {
  // Check for user authorization
  const email = localStorage.getItem("email");
  const token = localStorage.getItem("token");
  const userId = localStorage.getItem("userId");

  if (!email || !token || !userId) {
    // Show unauthorized toast message
    const toast = new bootstrap.Toast(
      document.getElementById("unauthorizedToast")
    );
    toast.show();

    setTimeout(() => {
      window.location.href = "index.html"; // Redirect to index.html after showing the toast
    }, 3000); // Redirect after 3 seconds
    return; // Exit the function
  }

  fetchQuizzes();
});

function fetchQuizzes() {
  fetch(`${IP}/api/Quiz/quizzes`, {
    headers: {
      accept: "text/plain",
      Authorization:
        "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJuYmYiOjE3MjE4MjEwMzAsImV4cCI6MTcyMjQyNTgzMCwiaWF0IjoxNzIxODIxMDMwfQ.Zzf4LjLhQAVRJiutJRpu2H4NTsZNvnhvV8o8L9NZfCI",
    },
  })
    .then((response) => response.json())
    .then((data) => displayQuizzes(data))
    .catch((error) => console.error("Error fetching quizzes:", error));
}

function displayQuizzes(quizzes) {
  const quizList = document.getElementById("quiz-list");
  quizList.innerHTML = "";

  quizzes.forEach((quiz) => {
    const currentTime = new Date();
    if (new Date(quiz.endTime) < currentTime) return;

    const quizItem = document.createElement("div");
    quizItem.className = "list-group-item";

    const quizHeader = document.createElement("div");
    quizHeader.className = "list-group-item-action";
    quizHeader.innerText = `${quiz.topic} - ${quiz.description}`;
    quizHeader.addEventListener("click", (event) => {
      event.stopPropagation();
      toggleQuizDetails(quizItem, quiz);
      quizHeader.classList.toggle("active");
    });

    quizItem.appendChild(quizHeader);
    quizList.appendChild(quizItem);
  });
}

function toggleQuizDetails(quizItem, quiz) {
  let detailsDiv = quizItem.querySelector(".quiz-details");

  if (detailsDiv) {
    detailsDiv.remove();
  } else {
    detailsDiv = document.createElement("div");
    detailsDiv.className = "quiz-details mt-3";

    const detailsHTML = `
      <p><strong>Duration:</strong> ${
        quiz.duration > 60
          ? (quiz.duration / 60).toFixed(2) + " hr"
          : quiz.duration + " minutes"
      }</p>
      <p><strong>Start Time:</strong> ${new Date(
        quiz.startTime
      ).toLocaleString()}</p>
      <p><strong>End Time:</strong> ${new Date(
        quiz.endTime
      ).toLocaleString()}</p>
    `;

    if (quiz.type !== "private") {
      detailsDiv.innerHTML = `
        ${detailsHTML}
        <p><strong>Code:</strong> ${quiz.code}</p>
      `;
    } else {
      detailsDiv.innerHTML = `
        ${detailsHTML}
        <div class="input-group mb-3">
          <input type="text" class="form-control" placeholder="Enter Code" aria-label="Enter Code">
          <button class="btn btn-primary" type="button">Attend Test</button>
        </div>
      `;
    }

    const attendButton = document.createElement("button");
    attendButton.className = "btn btn-secondary mt-2";
    attendButton.innerText = "Attend Quiz";
    attendButton.onclick = () => {
      window.location.href = `attendQuiz.html?quizId=${quiz.id}`;
    };

    detailsDiv.appendChild(attendButton);
    quizItem.appendChild(detailsDiv);
  }
}
