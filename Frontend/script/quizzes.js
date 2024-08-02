const IP = "https://quizbackend.raghavendiran.cloud";
//const IP = "http://127.0.0.1:8000";

document.getElementById("theme-toggle").addEventListener("change", toggleTheme);
document.addEventListener("DOMContentLoaded", init);

function init() {
  localStorage.removeItem("quizId");
  localStorage.removeItem("quizCode");
  checkAuthorization();
  fetchQuizzes("all", "all");
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
  const toastElement = document.getElementById("unauthorizedToast");
  if (!toastElement) return; // Check if the toast element exists

  const toast = new bootstrap.Toast(toastElement);
  toast.show();
  setTimeout(() => {
    window.location.href = "../html/login.html"; // Redirect to index.html after showing the toast
  }, 3000); // Redirect after 3 seconds
}

function filterQuizzes() {
  const filterValue = document.getElementById("quiz-filter").value;
  const timeFilterValue = document.getElementById("quiz-filter-time").value;

  // Fetch quizzes again based on the selected filter
  fetchQuizzes(filterValue, timeFilterValue);
}

function fetchQuizzes(typeFilter, timeFilter) {
  const token = localStorage.getItem("token");
  const quizList = document.getElementById("quiz-list");

  fetch(`${IP}/api/Quiz/quizzes`, {
    headers: {
      accept: "text/plain",
      Authorization: `Bearer ${token}`,
    },
  })
    .then((response) => response.json())
    .then((data) => {
      // Filter quizzes based on the selected filters
      const filteredQuizzes = data.filter((quiz) => {
        const currentTime = new Date();
        const startTime = new Date(quiz.startTime);
        const endTime = new Date(quiz.endTime);

        const timeFilterCondition =
          timeFilter === "live"
            ? startTime <= currentTime && endTime >= currentTime
            : timeFilter === "upcoming"
            ? startTime > currentTime
            : true;

        const typeFilterCondition =
          typeFilter === "all" ? true : quiz.type === typeFilter;

        return timeFilterCondition && typeFilterCondition;
      });

      displayQuizzes(filteredQuizzes);
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
  const btnElement = createAttendButton(quiz);

  cardDiv.appendChild(imgElement);
  cardDiv.appendChild(cardBodyDiv);
  cardDiv.appendChild(detailsList);
  cardDiv.appendChild(btnElement);

  return cardDiv;
}

function createImageElement(imageURL) {
  const imgElement = document.createElement("img");
  imgElement.className = "card-img-top";

  // Set a placeholder image if the original fails to load
  imgElement.src = imageURL || "https://via.placeholder.com/150";
  imgElement.alt = "LOGO";

  imgElement.onerror = () => {
    imgElement.src = "https://via.placeholder.com/150"; // Fallback to placeholder on error
  };

  return imgElement;
}

function createCardBody(quiz) {
  const cardBodyDiv = document.createElement("div");
  cardBodyDiv.className = "card-body";

  const cardTitleElement = document.createElement("h5");
  cardTitleElement.className = "card-title";
  cardTitleElement.textContent = `${quiz.topic}`;

  const labelSpan = document.createElement("span");
  const currentTime = new Date();
  const startTime = new Date(quiz.startTime);
  const endTime = new Date(quiz.endTime);

  if (startTime <= currentTime && endTime >= currentTime) {
    labelSpan.textContent = " (Live)";
    labelSpan.style.color = "green";
  } else if (startTime > currentTime) {
    labelSpan.textContent = " (Upcoming)";
    labelSpan.style.color = "red";
  }

  cardTitleElement.appendChild(labelSpan);

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

  detailsList.appendChild(durationItem);
  detailsList.appendChild(startTimeItem);
  detailsList.appendChild(endTimeItem);
  detailsList.appendChild(typeItem);

  return detailsList;
}

function createListItem(text) {
  const listItem = document.createElement("li");
  listItem.className = "list-group-item";
  listItem.textContent = text;
  return listItem;
}

function createAttendButton(quiz) {
  const btnElement = document.createElement("a");
  btnElement.className = "btn btn-primary";
  btnElement.href = `../html/attendQuiz.html?quizId=${quiz.quizId}`;
  btnElement.onclick = () => {
    if (quiz.type === "public") localStorage.setItem("quizCode", quiz.code);

    localStorage.setItem("quizId", quiz.quizId);
  };
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
      ? `${detailsHTML}`
      : `${detailsHTML}<div class="input-group mb-3"><input type="text" class="form-control" placeholder="Enter Code" aria-label="Enter Code"><button class="btn btn-primary" type="button">Attend Test</button></div>`;

  const attendButton = document.createElement("button");
  attendButton.className = "btn btn-secondary mt-2";
  attendButton.innerText = "Attend Quiz";
  attendButton.onclick = () => {
    window.location.href = `../html/attendQuiz.html?quizId=${quiz.quizId}`;
  };

  detailsDiv.appendChild(attendButton);
  return detailsDiv;
}

function logout() {
  localStorage.removeItem("token");
  window.location.href = "../html/login.html";
}
