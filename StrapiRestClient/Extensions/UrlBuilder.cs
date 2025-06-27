using StrapiRestClient.Request;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrapiRestClient.Extensions
{
    /// <summary>
    /// A utility class for building URLs for Strapi API requests.
    /// </summary>
    public static class UrlBuilder
    {
        /// <summary>
        /// Creates a URL for a Strapi API request.
        /// </summary>
        /// <param name="endpoint">The base endpoint of the Strapi API.</param>
        /// <param name="request">The Strapi request object.</param>
        /// <returns>A fully constructed URL for the Strapi API request.</returns>
        public static string Create(string endpoint, StrapiRequest request)
        {
            var baseUrl = $"{endpoint.TrimEnd('/')}/{request.ContentType}{request.Path}";
            var queryParams = new List<string>();

            // Handle populate parameters
            var populateObject = request.PopulateObject;
            if (populateObject != null)
            {
                var populateParams = new List<string>();
                FlattenPopulate(populateObject, "populate", populateParams);
                queryParams.AddRange(populateParams);
            }

            // Process filters
            ProcessFilters(request.Filters, queryParams);

            return queryParams.Any()
                ? $"{baseUrl}?{string.Join("&", queryParams)}"
                : baseUrl;
        }

        private static readonly Dictionary<string, Func<RequestFilter, int, string>> FilterFormatters = new()
        {
            { "sort", (filter, index) => $"sort[{index}]={filter.Field}:{filter.Value}" },
            { "fields", (filter, _) => $"fields[]={filter.Field}" },
            { "pagination", (filter, _) => $"pagination[{filter.Field}]={filter.Value}" },
            { "randomSort", (filter, _) => $"randomSort={filter.Value}" },
            { "$in", (filter, _) => $"{BuildNestedFilterKey(filter.Field, filter.Type)}={filter.Value}" },
            { "$notIn", (filter, _) => $"{BuildNestedFilterKey(filter.Field, filter.Type)}={filter.Value}" },
            { "$null", (filter, _) => BuildNestedFilterKey(filter.Field, filter.Type) },
            { "$notNull", (filter, _) => BuildNestedFilterKey(filter.Field, filter.Type) },
            { "$eq", (filter, _) => $"filters[{filter.Field}][{filter.Type}]={filter.Value}" },
        };

        private static void ProcessFilters(IEnumerable<RequestFilter> filters, List<string> queryParams)
        {
            int sortIndex = 0;

            foreach (var filter in filters)
            {
                if (FilterFormatters.TryGetValue(filter.Type, out var formatter))
                {
                    queryParams.Add(formatter(filter, filter.Type == "sort" ? sortIndex++ : 0));
                }
                else
                {
                    var defaultKey = BuildNestedFilterKey(filter.Field, filter.Type);
                    queryParams.Add($"{defaultKey}={filter.Value}");
                }
            }
        }

        private static void FlattenPopulate(object obj, string prefix, List<string> queryParams)
        {
            switch (obj)
            {
                case string str when str == "*":
                    queryParams.Add($"{prefix}=*");
                    break;

                case Dictionary<string, object> dictionary:
                    foreach (var (key, value) in dictionary)
                    {
                        if (key == "fields" && value is List<string> fieldsList)
                        {
                            for (int i = 0; i < fieldsList.Count; i++)
                            {
                                queryParams.Add($"{prefix}[fields][{i}]={fieldsList[i]}");
                            }
                        }
                        else
                        {
                            FlattenPopulate(value, $"{prefix}[{key}]", queryParams);
                        }
                    }
                    break;
            }
        }

        private static string BuildNestedFilterKey(string fieldPath, string operation)
        {
            var fieldParts = string.Join("][", fieldPath.Split('.'));
            return $"filters[{fieldParts}][{operation}]";
        }
    }
}