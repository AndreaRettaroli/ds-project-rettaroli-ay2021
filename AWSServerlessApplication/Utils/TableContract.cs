
using System;

namespace AWSServerlessApplication.Utils
{
    public struct UsersTable
    {
        public static string TableName = $"{Constants.TableNamePrefix}users";
        public static string Id = "Id";
        public static string Name = "name";
        public static string Surname = "surname";
        public static string DisplayName = "displayName";
        public static string Email = "email";
        public static string ConfirmedAt = "confirmedAt";
        public static string Image = "image";
        public static string JobTitleId = "jobTitleId";
        public static string HourlyCost = "hourlyCost";
        public static string SeniorSkillsIds = "seniorSkillsIds";
        public static string IntermediateSkillsIds = "intermediateSkillsIds";
        public static string JuniorSkillsIds = "juniorSkillsIds";
        public static string Deleted = "deleted";
    }
    public struct Constants
    {
        public const string TableNamePrefixKey = "TableNamePrefix";
        public static string TableNamePrefix = Environment.GetEnvironmentVariable(TableNamePrefixKey) ?? Startup.Configuration?[TableNamePrefixKey];
        public static string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
        public const int DefaultPageSize = 3;
    }
}
