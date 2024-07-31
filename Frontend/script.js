const IP = "https://quizbackend.raghavendiran.cloud";

document.addEventListener("DOMContentLoaded", function () {
  checkAuthorization();
  fetchQuizzes();
});

document.getElementById("theme-toggle").addEventListener("change", toggleTheme);

function toggleTheme() {
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
}

function checkAuthorization() {
  const email = localStorage.getItem("email");
  const token = localStorage.getItem("token");
  const userId = localStorage.getItem("userId");

  if (!email || !token || !userId) {
    showToast("unauthorizedToast");
    setTimeout(() => (window.location.href = "index.html"), 3000);
  }
}

function fetchQuizzes() {
  fetch(`${IP}/api/Quiz/quizzes`, {
    headers: {
      accept: "text/plain",
      Authorization: "Bearer your_jwt_token",
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

    const card = createQuizCard(quiz);
    quizList.appendChild(card);
  });
}

function createQuizCard(quiz) {
  const cardDiv = document.createElement("div");
  cardDiv.className = "card m-2";
  cardDiv.style.width = "18rem";

  const imgElement = document.createElement("img");
  imgElement.className = "card-img-top";
  imgElement.src = quiz.imageURL || "https://via.placeholder.com/150";
  imgElement.alt = "Quiz Image";

  const cardBodyDiv = document.createElement("div");
  cardBodyDiv.className = "card-body";

  const cardTitleElement = document.createElement("h5");
  cardTitleElement.className = "card-title";
  cardTitleElement.textContent = quiz.topic;

  const cardTextElement = document.createElement("p");
  cardTextElement.className = "card-text";
  cardTextElement.textContent = quiz.description;

  const detailsList = createDetailsList(quiz);
  const btnElement = createAttendButton(quiz);

  cardBodyDiv.appendChild(cardTitleElement);
  cardBodyDiv.appendChild(cardTextElement);
  cardDiv.appendChild(imgElement);
  cardDiv.appendChild(cardBodyDiv);
  cardDiv.appendChild(detailsList);
  cardDiv.appendChild(btnElement);

  return cardDiv;
}

function createDetailsList(quiz) {
  const detailsList = document.createElement("ul");
  detailsList.className = "list-group list-group-flush";

  const durationItem = document.createElement("li");
  durationItem.className = "list-group-item";
  durationItem.textContent = `Duration: ${quiz.duration} min`;

  const startTimeItem = document.createElement("li");
  startTimeItem.className = "list-group-item";
  startTimeItem.textContent = `Start Time: ${new Date(
    quiz.startTime
  ).toLocaleString()}`;

  const endTimeItem = document.createElement("li");
  endTimeItem.className = "list-group-item";
  endTimeItem.textContent = `End Time: ${new Date(
    quiz.endTime
  ).toLocaleString()}`;

  const typeItem = document.createElement("li");
  typeItem.className = "list-group-item";
  typeItem.textContent = `Type: ${quiz.type}`;

  const codeItem = document.createElement("li");
  codeItem.className = "list-group-item";
  codeItem.textContent = `Code: ${quiz.code}`;

  detailsList.append(
    durationItem,
    startTimeItem,
    endTimeItem,
    typeItem,
    codeItem
  );
  return detailsList;
}

function createAttendButton(quiz) {
  const btnElement = document.createElement("a");
  btnElement.className = "btn btn-primary";
  btnElement.href = "attendQuiz.html";
  btnElement.textContent = "Attend Quiz";

  return btnElement;
}

function showToast(toastId) {
  const toast = new bootstrap.Toast(document.getElementById(toastId));
  toast.show();
}
