# ed-match-data-dropper
Lambda function that loads CSVs dropped in S3 to a database  
It is indicated as the drag and drop module in the Sports Predictions [architecture diagram](https://drive.google.com/file/d/1CE7FKSq8LaS-KvQZAYlxMyNxhMJoBQsX/view?usp=sharing)

# File Structure

- src - Code for the application's Lambda function.
- events - Invocation events that you can use to invoke the function.
- test - Unit tests for the application code. 
- template.yaml - A template that defines the application's AWS resources.
- samconfig.toml - A config that drives the deployment

# SAM template
The application uses several AWS resources, including the Lambda functions and an S3 Bucket for uploading CSVs.  
It contains definitions for registering the Lambda to an existing VPC in order to use an existing database.

# Deployment
To build and deploy the application, run the following in your shell:  

```bash
sam build
sam deploy --guided
```

The AWS CLI must be configured with a valid profile. 
The `MYSQL_CONNECTION_STRING` environment variable must also be set manually.

# Debugging
To debug the application locally use the AWS Toolkit for your favourite IDE.  

e.g [Rider](https://docs.aws.amazon.com/toolkit-for-jetbrains/latest/userguide/setup-toolkit.html)  

Or invoke the `sam local invoke` command with the sample S3Event.
             
 ```bash
sam build
sam local invoke MatchDataDropper --event events/event.json
 ```

## Logging
The application logs under its own log group in CloudWatch

## Cleanup
The stack can be deleted from CloudFormation