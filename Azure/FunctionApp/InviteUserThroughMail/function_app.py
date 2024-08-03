import azure.functions as func
import datetime
import json
import logging
import smtplib
from email.mime.text import MIMEText

app = func.FunctionApp()

@app.route(route="SendEmailFunction", auth_level=func.AuthLevel.FUNCTION)
def SendEmailFunction(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')

    try:
        req_body = req.get_json()
        
        # Extract necessary fields from the request body
        recipients = req_body.get('recipients')
        usertype = req_body.get('usertype')
        quizId = req_body.get('quizId')
        quizCode = req_body.get('quizCode')

        # Validate required fields
        if not (recipients and usertype and quizId and quizCode):
            return func.HttpResponse(
                "One or more required fields are missing.",
                status_code=400
            )

        # Email body templates
        newuser = f"""Dear {recipients[0]},
        We are excited to invite you to participate in an exclusive private quiz! 
        This quiz is specially designed to test your knowledge and provide a fun and engaging experience.
        \nQuiz Details:
        \nQuiz ID: {quizId}
        \nQuiz Code: {quizCode}
        \nDefault Password: pass@123
        \nInstructions to Join:
        1. Visit our quiz platform at quiz.raghavendiran.cloud.
        2. Reset the password at quiz.raghavendiran.cloud/email={recipients[0]}
        3. Enter the Quiz ID and Quiz Code provided above.
        4. Follow the on-screen instructions to begin the quiz.
        \nWe hope you enjoy the quiz and look forward to seeing your results.
        \nIf you have any questions or need further assistance, please do not hesitate to contact us.
        \nBest regards,
        \nQuiz"""

        olduser = f"""Dear {recipients[0]},
        We are excited to invite you to participate in an exclusive private quiz! 
        This quiz is specially designed to test your knowledge and provide a fun and engaging experience.
        \nQuiz Details:
        \nQuiz ID: {quizId}
        \nQuiz Code: {quizCode}
        \nInstructions to Join:
        1. Visit our quiz platform at quiz.raghavendiran.cloud.
        2. Enter the Quiz ID and Quiz Code provided above.
        3. Follow the on-screen instructions to begin the quiz.
        \nWe hope you enjoy the quiz and look forward to seeing your results.
        \nIf you have any questions or need further assistance, please do not hesitate to contact us.
        \nBest regards,
        \nQuiz"""

        # Determine the email body based on user type
        body = newuser if usertype == "new" else olduser
        
        # Email subject
        subject = "Invitation to Participate in a Private Quiz"
        
        # Sender email and password
        sender = "raghavendiran@gmail.com"
        password = "cdfayxc dfdasehzw pnmbvfdgsl jusdgsdtd"

        # Send email
        send_email(subject, body, sender, recipients, password)

        return func.HttpResponse(json.dumps({'status' : 200, 'isSend' : True}), status_code=200)
    except Exception as e:
        logging.error(f"Exception: {str(e)}")
        return func.HttpResponse(json.dumps({'status' : 500, 'error':{str(e)} ,'isSend' : False}), status_code=500)

def send_email(subject, body, sender, recipients, password):
    # Create the email message
    msg = MIMEText(body)
    msg['Subject'] = subject
    msg['From'] = sender
    msg['To'] = ', '.join(recipients)
    
    # Send the email using SMTP server
    with smtplib.SMTP_SSL('smtp.gmail.com', 465) as smtp_server:
        smtp_server.login(sender, password)
        smtp_server.sendmail(sender, recipients, msg.as_string())
    
    logging.info("Message sent!")

