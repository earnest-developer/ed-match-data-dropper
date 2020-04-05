using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3.Util;
using MySql.Data.MySqlClient;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace MatchData.Dropper
{
    public class Function
    {
        public async Task FunctionHandler(S3Event command, ILambdaContext context)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));

            LambdaLogger.Log("Lambda invocation begins");

            S3EventNotification.S3Entity s3Event = command.Records.FirstOrDefault()?.S3;

            if (s3Event is null)
            {
                LambdaLogger.Log("S3 event has no records");
                return;
            }

            string s3ObjectUri = $"'s3://{s3Event.Bucket.Name.Trim()}/{s3Event.Object.Key.Trim()}'";

            LambdaLogger.Log($"Loading {s3ObjectUri} in match_data");

            using (var connection = await CreateConnection())
            {
                string sql = $@"
                -- MatchData.Dropper Function.FunctionHandler
                LOAD DATA FROM S3 {s3ObjectUri}
                INTO TABLE match_data.football_match_data
                FIELDS TERMINATED BY ','
                LINES TERMINATED BY '\n'
                IGNORE 1 ROWS
                (division,
                @match_date,
                @ignore_time,
                home_team,
                away_team,
                ft_hg,
                ft_ag,
                ft_r,
                ht_hg,
                ht_ag,
                ht_r)
                SET match_date = STR_TO_DATE(@match_date, '%d/%m/%Y');";

                MySqlCommand cmd = new MySqlCommand(sql, connection);

                int rowsAffected = cmd.ExecuteNonQuery(); // Load data does not support parameters

                connection.Close();

                LambdaLogger.Log($"Loaded {rowsAffected.ToString()} rows in match_data");
            }
        }

        private async Task<MySqlConnection> CreateConnection()
        {
            string connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
            MySqlConnection connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }

        // TODO: In case we ever need to pull the csv itself
        // private async Task<string> GetCsvFileFromBucket(string bucketName, string objectKey)
        // {
        //     try
        //     {
        //         GetObjectRequest request = new GetObjectRequest
        //         {
        //             BucketName = bucketName,
        //             Key = objectKey
        //         };
        //
        //         using (GetObjectResponse response = await S3Client.GetObjectAsync(request))
        //         using (Stream responseStream = response.ResponseStream)
        //         using (StreamReader reader = new StreamReader(responseStream))
        //         {
        //             return await reader.ReadToEndAsync();
        //         }
        //     }
        //     catch (AmazonS3Exception e)
        //     {
        //         LambdaLogger.Log($"Error encountered ***. Message:'{e.Message}' when reading an object");
        //     }
        //     catch (Exception e)
        //     {
        //         LambdaLogger.Log(
        //             $"Error getting object {objectKey} from bucket {bucketName}. Make sure they exist and your bucket is in the same region as this function.");
        //         LambdaLogger.Log(e.Message);
        //         LambdaLogger.Log(e.StackTrace);
        //         throw;
        //     }
        //
        //     return default;
        // }
        
        // private IAmazonS3 S3Client { get; }
        //
        // /// <summary>
        // /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        // /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        // /// region the Lambda function is executed in.
        // /// </summary>
        // public Function()
        // {
        //     S3Client = new AmazonS3Client();
        // }

    }
}