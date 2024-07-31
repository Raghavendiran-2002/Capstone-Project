const IP = "https://quizbackend.raghavendiran.cloud";

document.getElementById("theme-toggle").addEventListener("change", toggleTheme);
document.addEventListener("DOMContentLoaded", init);

function init() {
  checkAuthorization();
  fetchQuizzes();
}

function toggleTheme() {
  const sunIcon = document.getElementById("sun-icon");
  const moonIcon = document.getElementById("moon-icon");
  const isDarkMode = this.checked;

  document.body.classList.toggle("dark-mode", isDarkMode);
  sunIcon.src = isDarkMode
    ? "../public/icon-sun-light.svg"
    : "../public/icon-sun-dark.svg";
  moonIcon.src = isDarkMode
    ? "../public/icon-moon-light.svg"
    : "../public/icon-moon-dark.svg";
}

function checkAuthorization() {
  const email = localStorage.getItem("email");
  const token = localStorage.getItem("token");

  if (!email || !token) {
    showUnauthorizedToast();
    return;
  }
}

function showUnauthorizedToast() {
  const toast = new bootstrap.Toast(
    document.getElementById("unauthorizedToast")
  );
  toast.show();
  setTimeout(() => {
    window.location.href = "index.html"; // Redirect to index.html after showing the toast
  }, 3000); // Redirect after 3 seconds
}

function fetchQuizzes() {
  const quizList = document.getElementById("quiz-list");

  fetch(`${IP}/api/Quiz/quizzes`, {
    headers: {
      accept: "text/plain",
      Authorization:
        "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJuYmYiOjE3MjE4MjEwMzAsImV4cCI6MTcyMjQyNTgzMCwiaWF0IjoxNzIxODIxMDMwfQ.Zzf4LjLhQAVRJiutJRpu2H4NTsZNvnhvV8o8L9NZfCI",
    },
  })
    .then((response) => response.json())
    .then((data) => {
      displayQuizzes(data);
    })
    .catch((error) => {
      quizList.innerHTML =
        "<p>Error loading quizzes. Please try again later.</p>"; // Error message
      console.error("Error fetching quizzes:", error);
    });
}

function displayQuizzes(quizzes) {
  const quizList = document.getElementById("quiz-list");
  quizList.innerHTML = ""; // Clear existing content

  // Add placeholder cards
  for (let i = 0; i < 3; i++) {
    // Adjust the number of placeholders as needed
    const placeholderCard = createPlaceholderCard();
    quizList.appendChild(placeholderCard);
  }

  // Remove placeholders after data is fetched
  setTimeout(() => {
    quizList.innerHTML = ""; // Clear placeholders
    quizzes.forEach((quiz) => {
      if (isQuizExpired(quiz)) return;
      const cardDiv = createQuizCard(quiz);
      quizList.appendChild(cardDiv);
    });
  }, 2000); // Simulate loading time (adjust as needed)
}

function createPlaceholderCard() {
  const cardDiv = document.createElement("div");
  cardDiv.className = "card";
  cardDiv.setAttribute("aria-hidden", "true");

  const imgElement = document.createElement("img");
  imgElement.className = "card-img-top placeholder";
  imgElement.src = "https://via.placeholder.com/150"; // Placeholder image
  imgElement.alt = "Loading...";

  const cardBodyDiv = document.createElement("div");
  cardBodyDiv.className = "card-body";

  const cardTitleElement = document.createElement("h5");
  cardTitleElement.className = "card-title placeholder-glow";
  cardTitleElement.innerHTML = '<span class="placeholder col-6"></span>';

  const cardTextElement = document.createElement("p");
  cardTextElement.className = "card-text placeholder-glow";
  cardTextElement.innerHTML = `
    <span class="placeholder col-7"></span>
    <span class="placeholder col-4"></span>
    <span class="placeholder col-4"></span>
    <span class="placeholder col-6"></span>
    <span class="placeholder col-8"></span>
  `;

  const btnElement = document.createElement("a");
  btnElement.className = "btn btn-primary disabled placeholder col-6";
  btnElement.setAttribute("aria-disabled", "true");

  cardBodyDiv.appendChild(cardTitleElement);
  cardBodyDiv.appendChild(cardTextElement);
  cardBodyDiv.appendChild(btnElement);
  cardDiv.appendChild(imgElement);
  cardDiv.appendChild(cardBodyDiv);

  return cardDiv;
}

function isQuizExpired(quiz) {
  const currentTime = new Date();
  return new Date(quiz.endTime) < currentTime;
}

function createQuizCard(quiz) {
  const cardDiv = document.createElement("div");
  cardDiv.className = "card";
  cardDiv.style.width = "18rem";
  cardDiv.style.marginBottom = "10px"; // Optional: Add margin between cards

  const imgElement = createImageElement(quiz.imageURL);
  const cardBodyDiv = createCardBody(quiz);
  const detailsList = createDetailsList(quiz);
  const btnElement = createAttendButton();

  cardDiv.appendChild(imgElement);
  cardDiv.appendChild(cardBodyDiv);
  cardDiv.appendChild(detailsList);
  cardDiv.appendChild(btnElement);

  return cardDiv;
}

function createImageElement(imageURL) {
  const imgElement = document.createElement("img");
  imgElement.className = "card-img-top";
  imgElement.src = imageURL || "https://via.placeholder.com/150";
  imgElement.alt = "LOGO";
  return imgElement;
}

function createCardBody(quiz) {
  const cardBodyDiv = document.createElement("div");
  cardBodyDiv.className = "card-body";

  const cardTitleElement = document.createElement("h5");
  cardTitleElement.className = "card-title placeholder-glow";
  cardTitleElement.textContent = `${quiz.topic}`;

  const cardTextElement = document.createElement("p");
  cardTextElement.className = "card-text";
  cardTextElement.textContent = `${quiz.description}`;

  cardBodyDiv.appendChild(cardTitleElement);
  cardBodyDiv.appendChild(cardTextElement);
  return cardBodyDiv;
}

function createDetailsList(quiz) {
  const detailsList = document.createElement("ul");
  detailsList.className = "list-group list-group-flush";

  const durationItem = createListItem(`Duration: ${quiz.duration} min`);
  const startTimeItem = createListItem(
    `Start Time: ${new Date(quiz.startTime).toLocaleString()}`
  );
  const endTimeItem = createListItem(
    `End Time: ${new Date(quiz.endTime).toLocaleString()}`
  );
  const typeItem = createListItem(`Type: ${quiz.type}`);
  const codeItem = createListItem(`Code: ${quiz.code}`);

  detailsList.appendChild(durationItem);
  detailsList.appendChild(startTimeItem);
  detailsList.appendChild(endTimeItem);
  detailsList.appendChild(typeItem);
  detailsList.appendChild(codeItem);

  return detailsList;
}

function createListItem(text) {
  const listItem = document.createElement("li");
  listItem.className = "list-group-item";
  listItem.textContent = text;
  return listItem;
}

function createAttendButton() {
  const btnElement = document.createElement("a");
  btnElement.className = "btn btn-primary";
  btnElement.href = "http://127.0.0.1:5500/html/attendQuiz.html";
  btnElement.textContent = "Attend Quiz";
  return btnElement;
}

function toggleQuizDetails(quizItem, quiz) {
  let detailsDiv = quizItem.querySelector(".quiz-details");

  if (detailsDiv) {
    detailsDiv.remove();
  } else {
    detailsDiv = createQuizDetails(quiz);
    quizItem.appendChild(detailsDiv);
  }
}

function createQuizDetails(quiz) {
  const detailsDiv = document.createElement("div");
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
    <p><strong>End Time:</strong> ${new Date(quiz.endTime).toLocaleString()}</p>
  `;

  detailsDiv.innerHTML =
    quiz.type !== "private"
      ? `${detailsHTML}<p><strong>Code:</strong> ${quiz.code}</p>`
      : `${detailsHTML}<div class="input-group mb-3"><input type="text" class="form-control" placeholder="Enter Code" aria-label="Enter Code"><button class="btn btn-primary" type="button">Attend Test</button></div>`;

  const attendButton = document.createElement("button");
  attendButton.className = "btn btn-secondary mt-2";
  attendButton.innerText = "Attend Quiz";
  attendButton.onclick = () => {
    window.location.href = `attendQuiz.html?quizId=${quiz.id}`;
  };

  detailsDiv.appendChild(attendButton);
  return detailsDiv;
}

function logout() {
  localStorage.removeItem("token");
  window.location.href = "login.html";
}
