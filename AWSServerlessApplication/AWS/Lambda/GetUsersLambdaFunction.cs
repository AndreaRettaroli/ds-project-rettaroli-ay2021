using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using AWSServerlessApplication.AWS;
using AWSServerlessApplication.Extensions;
using AWSServerlessApplication.Models;
using AWSServerlessApplication.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace AWS.Lambda
{
    public class GetUsersLambdaFunction
    {
        private ILambdaContext _lambdaContext;
        private static readonly string _tableName = "dev_users";

        public async Task<List<User>> HandlerAsync(ILambdaContext context)
        {
            try
            {
                Console.WriteLine("ok");
                var bucketName = Environment.GetEnvironmentVariable("ContentBucketName");
                var injector = new Injector();
                _lambdaContext = context;
                var client = injector.CreateDynamoDB();
                var request = new ScanRequest { TableName = _tableName };
                request.FilterExpression = "attribute_exists(#confirmedAt)";
                request.ExpressionAttributeNames.Add("#confirmedAt", UsersTable.ConfirmedAt);
                request.FilterExpression += " AND attribute_not_exists(#deleted)";
                request.ExpressionAttributeNames.Add("#deleted", UsersTable.Deleted);
                List<User> usersList = new List<User>();
                var items = await client.ScanAllAsync(request);
                Console.WriteLine(items);
                if (items.Count > 0)
                    items.ForEach(x => usersList.Add(x.ToDynamoDBUser().ToUser()));
                Console.WriteLine(items.Count + " primo elemento " + items.FirstOrDefault());
                Console.WriteLine(usersList.Count + " primo elemento " + usersList.FirstOrDefault());
                Console.WriteLine("finito");
                return usersList;
            }
            catch (Exception e ){
                Console.WriteLine(e.Message);
                return null;
            }

        }
    }
}
