# QuizSolution

## Overview

QuizSolution is a web application designed to facilitate quizzes and manage user interactions. This README provides an overview of the project, setup instructions, and environment configuration.

## Environment Configuration

The application requires specific environment variables to function correctly. Below are the necessary variables and their descriptions:

- **SQL_SERVER_CONNECTION_STRING**: Connection string for the SQL Server database.
- **JWT_USER_SECRET**: Secret key used for signing JSON Web Tokens (JWT).
- **SEND_INVITATION_URL**: URL endpoint for sending email invitations.
- **GenerateCertificateURL**: URL endpoint for generating certificates (currently empty).
- **BASE_URL_SEND_INVITATION**: Base URL for sending invitations (currently empty).
- **BASE_URL_GENERATE_CERTIFICATE**: Base URL for generating certificates (currently empty).
- **GENERATE_CERTIFICATE_URL**: URL for generating certificates (currently empty).

## Setup Instructions

1. Clone the repository:

   ```bash
   git clone <repository-url>
   cd QuizSolution
   ```

2. Create a `.env` file in the root directory and populate it with the required environment variables.

3. Run the application using Docker Compose:

   ```bash
   docker-compose up
   ```
