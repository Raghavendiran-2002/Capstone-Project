const IP ="https://quizbackend.raghavendiran.cloud"
document.getElementById("theme-toggle").addEventListener("change", function () {
  if (this.checked) {
    document.body.classList.add("dark-mode");
  } else {
    document.body.classList.remove("dark-mode");
  }
});

document.addEventListener("DOMContentLoaded", function () { 
  fetchQuizzes();
});

function fetchQuizzes () {
  fetch(
    `${IP}/api/Quiz/quizzes`,
    {
      headers: {
        accept: "text/plain",
        Authorization:
          "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJuYmYiOjE3MjE4MjEwMzAsImV4cCI6MTcyMjQyNTgzMCwiaWF0IjoxNzIxODIxMDMwfQ.Zzf4LjLhQAVRJiutJRpu2H4NTsZNvnhvV8o8L9NZfCI",
      },
    }
  )
    .then((response) => response.json())
    .then((data) => displayQuizzes(data))
    .catch((error) => console.error("Error fetching quizzes:", error));
}

function displayQuizzes(quizzes) {
  const quizList = document.getElementById("quiz-list");
  quizList.innerHTML = "";

  quizzes.forEach((quiz) => {
    const quizItem = document.createElement("a");
    quizItem.href = "#";
    quizItem.className = "list-group-item list-group-item-action";
    quizItem.innerText = `${quiz.topic} - ${quiz.description}`;

    quizList.appendChild(quizItem);
  });
}
