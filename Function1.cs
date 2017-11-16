using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Text;
using Microsoft.ServiceBus.Messaging;

namespace FunctionApp3
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([EventHubTrigger("test", Connection ="EventHub")]EventData myEventHubMessage, TraceWriter log)
        {
            //log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");
            log.Info($"{Encoding.UTF8.GetString(myEventHubMessage.GetBytes())}");

            string accessKey = "";
            string secretKey = "";
            //AmazonS3Config config = new AmazonS3Config();

            AmazonS3Client s3Client = new AmazonS3Client(
                    accessKey,
                    secretKey,
                    RegionEndpoint.USWest2
        );

            TransferUtility utility = new TransferUtility(s3Client);

            string BucketName = "myalooma";
            string key = "test.json";

            //string path = "C:\\Users\\zhangr2\\test.json";
            //FileStream fs = File.OpenWrite(path);
            //Byte[] info =
            //    new UTF8Encoding(true).GetBytes(myEventHubMessage);

            //fs.Write(info, 0, info.Length);



            var memStream = new MemoryStream();

            GetObjectRequest request = new GetObjectRequest()
            {
                BucketName = "myalooma",
                Key = "test.json"
            };
            GetObjectResponse response = s3Client.GetObject(request);
            Stream responseStream = response.ResponseStream;
            responseStream.CopyTo(memStream);


            var streamWriter = new StreamWriter(memStream);
            streamWriter.WriteLine(Encoding.UTF8.GetString(myEventHubMessage.GetBytes()));

            //connect to storagy of Azure
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==");
            //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            //CloudBlobContainer container = blobClient.GetContainerReference("[Your container]");
            //var fileToUpload = new MemoryStream();


            streamWriter.Flush();
            memStream.Seek(0, SeekOrigin.Begin);
            
            utility.Upload(memStream, BucketName, key);

            //System.IO.File.WriteAllBytes("local.json", memStream.ToArray());
            //foreach (string message in myEventHubMessage)
            //{
            //    //CloudBlockBlob blockBlob = container.GetBlockBlobReference("message-[File Name]");

            //    utility.Upload(fileToUpload, BucketName, key);
            //}



        }
    }
}
