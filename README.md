# ed-match-data-dropper
Lambda function that inserts CSVs dropped in S3 to a database

# Dependencies

## Amazon S3 Bucket
name: match-data-bucket  
region: eu-west-2  

public access blocked  
enabled versioning to support event notifications

# Triggers

## Amazon S3 Bucket
name: match-data-bucket  
region: eu-west-2  

event type: all object create events  
suffix: .csv  