using Amazon.DynamoDBv2.Model;
using AWSServerlessApplication.Extensions;
using AWSServerlessApplication.Models;
using AWSServerlessApplication.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerlessApplication.Services.DynamoDBRequests
{
    public static class UserRequest
    {
        private static readonly string _tableName = UsersTable.TableName;

        public static PutItemRequest Insert(DynamoDBUser user)
        {
            return new PutItemRequest
            {
                TableName = _tableName,
                Item = user.ToAttributeMap()
            };
        }
        public static UpdateItemRequest Confirm(string id, DateTime now)
        {
            return new UpdateItemRequest
            {
                TableName = _tableName,
                Key = id.ToDynamoDBKey(UsersTable.Id),
                UpdateExpression = "SET #confirmedAt = :v_confirmedAt",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {"#confirmedAt", UsersTable.ConfirmedAt}
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":v_confirmedAt", new AttributeValue(now.ToString(Constants.DateTimeFormat)) }
                }
            };
        }
        public static UpdateItemRequest DeleteAsync(string id) => new UpdateItemRequest
        {
            TableName = _tableName,
            Key = DynamoDBUtils.GenerateKey(UsersTable.Id, id),
            UpdateExpression = "SET #d = :v_del",
            ConditionExpression = "attribute_exists(#i)",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#i", UsersTable.Id },
                { "#d", UsersTable.Deleted }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":v_del", new AttributeValue(DateTimeExtensions.CestNow.ToString(Constants.DateTimeFormat)) }
            },
            ReturnValues = "UPDATED_NEW"
        };

    }
}
