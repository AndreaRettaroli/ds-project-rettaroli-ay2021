using Amazon;
using Amazon.DynamoDBv2;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerlessApplication.AWS
{
    public class Injector
    {
        private static string regionCode;
        private static RegionEndpoint region;
        public Injector()
        {
            regionCode = Environment.GetEnvironmentVariable("AWS_DEFAULT_REGION") ?? "eu-west-1";
            region = Amazon.RegionEndpoint.GetBySystemName(regionCode);

        }
        public IAmazonS3 CreateS3()
        {
            return new AmazonS3Client(region);
        }
        public IAmazonDynamoDB CreateDynamoDB()
        {
            return new AmazonDynamoDBClient(region);
        }

    }
}
