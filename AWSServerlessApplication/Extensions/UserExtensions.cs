using Amazon.DynamoDBv2.Model;
using AWSServerlessApplication.Models;
using AWSServerlessApplication.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using AWSServerlessApplication.Extensions;

namespace AWSServerlessApplication.Extensions
{
    public static class UserExtensions
    {
        public static DynamoDBUser ToDynamoDBUser(this Dictionary<string, AttributeValue> item)
        {
            var user = new DynamoDBUser
            {
                Id = item.GetString(UsersTable.Id),
                Name = item.GetString(UsersTable.Name),
                Surname = item.GetString(UsersTable.Surname),
                Email = item.GetString(UsersTable.Email),
                DisplayName = item.GetString(UsersTable.DisplayName),
                ConfirmedAt = item.GetDateTimeOrNull(UsersTable.ConfirmedAt),
                ImageUrl = item.GetString(UsersTable.Image),
                Deleted = item.GetDateTimeOrNull(UsersTable.Deleted),
            };
            return user;
        }
        public static Dictionary<string, AttributeValue> ToAttributeMap(this DynamoDBUser user)
        {
            var item = new Dictionary<string, AttributeValue>();

            item.Add(UsersTable.Id, new AttributeValue(user.Id));
            item.Add(UsersTable.Name, new AttributeValue(user.Name), condition: !string.IsNullOrEmpty(user.Name));
            item.Add(UsersTable.Surname, new AttributeValue(user.Surname), condition: !string.IsNullOrEmpty(user.Surname));
            item.Add(UsersTable.Email, new AttributeValue(user.Email), condition: !string.IsNullOrEmpty(user.Email));
            item.Add(UsersTable.DisplayName, new AttributeValue(user.Name.ToLower() + "_" + user.Surname.ToLower()), condition: !string.IsNullOrEmpty(user.Name) && !string.IsNullOrEmpty(user.Surname));
            item.Add(UsersTable.ConfirmedAt, new AttributeValue(user.ConfirmedAt.ToString()), condition: !string.IsNullOrEmpty(user.ConfirmedAt.ToString()));
            item.Add(UsersTable.Image, new AttributeValue(user.ImageUrl), condition: !string.IsNullOrEmpty(user.ImageUrl));
            item.Add(UsersTable.Deleted, new AttributeValue(user.Deleted.ToString()), condition: user.Deleted != null);

            return item;
        }

        public static User ToUser(this DynamoDBUser dynamoDBuser) => new User
        {
            Id = dynamoDBuser.Id,
            Name = dynamoDBuser.Name,
            Surname = dynamoDBuser.Surname,
            Email = dynamoDBuser.Email,
            DisplayName=dynamoDBuser.DisplayName,
            ConfirmedAt = dynamoDBuser.ConfirmedAt,
            ImageUrl = dynamoDBuser.ImageUrl,
            Deleted = dynamoDBuser.Deleted,
        };
        public static void FillUpdate(this DynamoDBUser newUser,User currentUser)
        {
            newUser.ConfirmedAt = currentUser.ConfirmedAt;
            newUser.Email = currentUser.Email;
        }
    }
}
