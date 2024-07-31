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
        subject = req_body.get('subject')
        body = req_body.get('body')
        sender = "raghavendiran222222@gmail.com"
        recipients = req_body.get('recipients')
        password = "fuvu iktj oynf vymc"

        if not (subject and body and  recipients):
            return func.HttpResponse(
                "One or more required fields are missing.",
                status_code=400
            )

        send_email(subject, body, sender, recipients, password)
        return func.HttpResponse(json.dumps({'status' : 200, 'isSend' : True}), status_code=200)
    except Exception as e:
        logging.error(f"Exception: {str(e)}")
        return func.HttpResponse(json.dumps({'status' : 500, 'isSend' : False}), status_code=500)

def send_email(subject, body, sender, recipients, password):
    msg = MIMEText(body)
    msg['Subject'] = subject
    msg['From'] = sender
    msg['To'] = ', '.join(recipients)
    with smtplib.SMTP_SSL('smtp.gmail.com', 465) as smtp_server:
        smtp_server.login(sender, password)
        smtp_server.sendmail(sender, recipients, msg.as_string())
    logging.info("Message sent!")