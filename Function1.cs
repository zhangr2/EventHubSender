using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Text;
using Microsoft.ServiceBus.Messaging;
using System.IO;
using Newtonsoft.Json.Linq;
using Amazon.S3.IO;

namespace FunctionApp4
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([EventHubTrigger("zhangrepc", Connection = "EventHub")]EventData[] myEventHubMessage, TraceWriter log)
        {
            log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");

            string accessKey = Environment.GetEnvironmentVariable("accessKey");
            string secretKey = Environment.GetEnvironmentVariable("secretKey");

            AmazonS3Client s3Client = new AmazonS3Client(
                   accessKey,
                   secretKey,
                   RegionEndpoint.USWest2);

            TransferUtility utility = new TransferUtility(s3Client);

            String now = DateTime.UtcNow.ToString();

            String BucketName = "myalooma";
            String key = now+".json";


            var memStream = new MemoryStream();

            

            S3FileInfo s3FileInfo = new S3FileInfo(s3Client, BucketName, key);
            if (!s3FileInfo.Exists)
            {
                PutObjectRequest pr = new PutObjectRequest();
                pr.BucketName = BucketName;
                pr.Key = key;
                s3Client.PutObject(pr);
            }

            GetObjectRequest request = new GetObjectRequest()
            {
                BucketName = "myalooma",
                Key = key
            };


            GetObjectResponse response = s3Client.GetObject(request);
            Stream responseStream = response.ResponseStream;
            responseStream.CopyTo(memStream);


            var streamWriter = new StreamWriter(memStream);
            //log.Info($"{Encoding.UTF8.GetString(myEventHubMessage.GetBytes())}");
            //log.Info($"{myEventHubMessage.Offset}");
            //var jsonBody = Encoding.UTF8.GetString(myEventHubMessage.GetBytes());
            //var myPoco = JObject.Parse(jsonBody);


            foreach(EventData ed in myEventHubMessage)
            {
                log.Info($"{Encoding.UTF8.GetString(ed.GetBytes())}");
                var jsonBody = Encoding.UTF8.GetString(ed.GetBytes()); ;
                streamWriter.WriteLine(jsonBody);
            }

            //streamWriter.WriteLine(jsonBody);
            streamWriter.Flush();
            memStream.Seek(0, SeekOrigin.Begin);

            utility.Upload(memStream, BucketName, key);







        }
    }
}
