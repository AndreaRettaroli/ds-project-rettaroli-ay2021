using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AWSServerlessApplication.Extensions;

namespace AWSServerlessApplication.Utils
{
    public static class DynamoDBUtils
    {
        public const string TranslationsKey = "translations";
        public const int BatchGetRequestLimit = 100;

        public static string GetString(this Dictionary<string, AttributeValue> item, string key)
        {
            return item.TryGetValueOrDefault(key, null)?.S;
        }

        public static string GetStringOrEmpty(this Dictionary<string, AttributeValue> item, string key)
        {
            return item.TryGetValueOrDefault(key, null)?.S ?? string.Empty;
        }

        public static string GetLocalizedStringOrDefault(this Dictionary<string, AttributeValue> item, string key, string lang)
        {
            if (string.IsNullOrEmpty(lang)) return item.GetString(key);
            var map = item.GetMap(TranslationsKey)?.GetMap(lang);
            return map?.GetString(key) ?? item.GetString(key);
        }

        public static Dictionary<string, Dictionary<string, string>> GetTranslations(this Dictionary<string, AttributeValue> item)
        {
            return item.GetMap(TranslationsKey)?.ToDictionary(e => e.Key, e => e.Value.M.ToDictionary(x => x.Key, x => x.Value.S));
        }

        public static List<string> GetStringSet(this Dictionary<string, AttributeValue> item, string key)
        {
            return item.TryGetValueOrDefault(key, null)?.SS ?? new List<string>(0);
        }

        public static List<string> GetStringSetOrNull(this Dictionary<string, AttributeValue> item, string key)
        {
            return item.TryGetValueOrDefault(key, null)?.SS ?? null;
        }

        public static List<int> GetNumberSetOrNull(this Dictionary<string, AttributeValue> item, string key)
        {
            return item.TryGetValueOrDefault(key, null)?.NS?.Select(i => i.ToInt()).ToList();
        }

        public static List<AttributeValue> GetList(this Dictionary<string, AttributeValue> item, string key)
        {
            return item.TryGetValueOrDefault(key, null)?.L;

        }

        public static List<string> GetStringList(this Dictionary<string, AttributeValue> item, string key)
        {
            return item.TryGetValueOrDefault(key, null)?.L.Select(i => i.S).ToList();
        }

        public static Dictionary<string, AttributeValue> GetMap(this Dictionary<string, AttributeValue> item, string key)
        {
            var value = item.TryGetValueOrDefault(key, null);
            return value != null && value.IsMSet ? value.M : null;
        }

        public static bool GetBool(this Dictionary<string, AttributeValue> item, string key)
        {
            return item.TryGetValueOrDefault(key, null)?.BOOL ?? false;
        }

        public static DateTime GetDateTimeOrNow(this Dictionary<string, AttributeValue> item, string key)
        {
            var value = item.TryGetValueOrDefault(key, null)?.S ?? string.Empty;
            if (DateTime.TryParse(value, out DateTime date))
            {
                return date;
            }
            return DateTime.UtcNow;
        }

        public static DateTime? GetDateTimeOrNull(this Dictionary<string, AttributeValue> item, string key)
        {
            var value = item.TryGetValueOrDefault(key, null)?.S ?? string.Empty;
            if (DateTime.TryParse(value, out DateTime date))
            {
                return date;
            }
            return null;
        }

        public static int? GetIntOrNull(this Dictionary<string, AttributeValue> item, string key)
        {
            return item.TryGetValueOrDefault(key, null)?.N.ToInt();
        }

        public static int GetInt(this Dictionary<string, AttributeValue> item, string key)
        {
            return item.TryGetValueOrDefault(key, null)?.N.ToInt() ?? 0;
        }

        public static int GetIntOrNegative(this Dictionary<string, AttributeValue> item, string key)
        {
            return item.TryGetValueOrDefault(key, null)?.N.ToInt(-1) ?? -1;
        }

        public static double GetDouble(this Dictionary<string, AttributeValue> item, string key)
        {
            return double.TryParse(item.TryGetValueOrDefault(key, null)?.N, NumberStyles.Any, CultureInfo.InvariantCulture, out double value) ? value : 0;
        }

        public static double? GetDoubleOrNull(this Dictionary<string, AttributeValue> item, string key)
        {
            return double.TryParse(item.TryGetValueOrDefault(key, null)?.N, NumberStyles.Any, CultureInfo.InvariantCulture, out double value) ? value : (double?)null;
        }

        public static long GetLongrOrZero(this Dictionary<string, AttributeValue> item, string key)
        {
            return item.TryGetValueOrDefault(key, null)?.N.ToLong(0) ?? 0;
        }

        public static async Task<List<Dictionary<string, AttributeValue>>> QueryAllAsync(this IAmazonDynamoDB dynamoDB, QueryRequest request)
        {
            var items = new List<Dictionary<string, AttributeValue>>();
            QueryResponse response;
            do
            {
                response = await dynamoDB.QueryAsync(request);
                items.AddRange(response.Items);
                request.ExclusiveStartKey = response.LastEvaluatedKey;
            } while (response.LastEvaluatedKey?.Count > 0);
            return items;
        }

        public static async Task<List<Dictionary<string, AttributeValue>>> ScanAllAsync(this IAmazonDynamoDB dynamoDB, ScanRequest request)
        {
            var items = new List<Dictionary<string, AttributeValue>>();
            ScanResponse response;
            do
            {
                response = await dynamoDB.ScanAsync(request);
                items.AddRange(response.Items);
                request.ExclusiveStartKey = response.LastEvaluatedKey;
            } while (response.LastEvaluatedKey?.Count > 0);

            return items;
        }

        public static async Task<List<Dictionary<string, AttributeValue>>> QueryAsync(
            this IAmazonDynamoDB dynamoDB,
            QueryRequest request,
            int limit,
            Dictionary<string, AttributeValue> key = null)
        {
            var items = new List<Dictionary<string, AttributeValue>>();
            request.ExclusiveStartKey = key;
            QueryResponse response;
            do
            {
                response = await dynamoDB.QueryAsync(request);
                items.AddRange(response.Items);
                request.ExclusiveStartKey = response.LastEvaluatedKey;
            } while (response.LastEvaluatedKey?.Count > 0 && items.Count < limit);

            return items.Take(limit).ToList();
        }

        public static async Task<List<Dictionary<string, AttributeValue>>> ScanAsync(
            this IAmazonDynamoDB dynamoDB,
            ScanRequest request,
            int limit,
            Dictionary<string, AttributeValue> key = null)
        {
            var items = new List<Dictionary<string, AttributeValue>>();
            request.ExclusiveStartKey = key;
            ScanResponse response;
            do
            {
                response = await dynamoDB.ScanAsync(request);
                items.AddRange(response.Items);
                request.ExclusiveStartKey = response.LastEvaluatedKey;
            } while (response.LastEvaluatedKey?.Count > 0 && items.Count < limit);
            return items.Take(limit).ToList();
        }

        public static async Task<List<Dictionary<string, AttributeValue>>> BatchGetItems(
            this IAmazonDynamoDB dynamoDB,
            string tableName,
            string keyName,
            IEnumerable<string> keyValues,
            IEnumerable<string> attributes = null)
        {
            if (keyValues == null || keyValues.Count() == 0) return new List<Dictionary<string, AttributeValue>>(0);

            var items = new List<Dictionary<string, AttributeValue>>();
            var chuncks = keyValues
                            .Distinct()
                            .Select(v => GenerateKey(keyName, v))
                            .ToList()
                            .ChunkBy(BatchGetRequestLimit);
            var tasks = new List<Task<BatchGetItemResponse>>(chuncks.Count);
            string projectionExpression = string.Empty;
            Dictionary<string, string> names = null;
            if (attributes?.Count() > 0)
            {
                names = attributes.Distinct().ToDictionary(s => $"#{s}", s => s);
                projectionExpression = string.Join(", ", names.Keys);
            }
            foreach (var chunk in chuncks)
            {
                var request = new BatchGetItemRequest
                {
                    RequestItems = new Dictionary<string, KeysAndAttributes>
                    {
                        { tableName, new KeysAndAttributes { Keys = chunk }}
                    }
                };
                if (!string.IsNullOrEmpty(projectionExpression))
                {
                    request.RequestItems[tableName].ProjectionExpression = projectionExpression;
                    request.RequestItems[tableName].ExpressionAttributeNames = names;
                }
                var task = dynamoDB.BatchGetItemAsync(request);
                tasks.Add(task);
            }
            var batchResponseList = await Task.WhenAll(tasks);
            return batchResponseList.SelectMany(r => r.Responses[tableName]).ToList();
        }

        public static Dictionary<string, AttributeValue> GenerateKey(string key, string value)
        {
            return string.IsNullOrEmpty(value) ? null : new Dictionary<string, AttributeValue> { { key, new AttributeValue(value) } };
        }

        public static Dictionary<string, AttributeValue> ToDynamoDBKey(this string value, string key)
        {
            return string.IsNullOrEmpty(value) ? null : new Dictionary<string, AttributeValue> { { key, new AttributeValue(value) } };
        }
    }
}