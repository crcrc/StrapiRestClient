using StrapiRestClient.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace StrapiRestClient.Request
{
    /// <summary>
    /// Main query request class for Strapi 5 API
    /// </summary>
    public class StrapiQueryRequest
    {
        public RequestMethod Method { get; set; } = RequestMethod.Get;
        public object Body { get; set; }

        // <summary>
        /// The Strapi collection/content type to query (e.g., "articles", "blog-posts", "products")
        /// </summary>
        public string DocumentType { get; set; }

        /// <summary>
        /// Filter the response using various operators
        /// </summary>
        public Dictionary<string, object> Filters { get; set; } = new();

        /// <summary>
        /// Select a specific locale
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Select the Draft & Publish status (published, draft)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Populate relations, components, or dynamic zones
        /// </summary>
        public Dictionary<string, PopulateOptions> Populate { get; set; } = new();

        /// <summary>
        /// Simple populate all option (populate=*) - populates everything 1 level deep
        /// When set to true, this overrides the Populate dictionary
        /// </summary>
        public bool PopulateAll { get; set; } = false;

        /// <summary>
        /// Select only specific fields to display
        /// </summary>
        public string[] Fields { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Sort the response (e.g., "title:asc", "createdAt:desc")
        /// </summary>
        public string[] Sort { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Page through entries
        /// </summary>
        public PaginationOptions Pagination { get; set; } = new();

        /// <summary>
        /// Constructor requiring document type
        /// </summary>
        /// <param name="documentType">The Strapi collection/content type (e.g., "articles", "blog-posts")</param>
        public StrapiQueryRequest(string documentType)
        {
            if (string.IsNullOrWhiteSpace(documentType))
                throw new ArgumentException("Document type cannot be null or empty", nameof(documentType));

            DocumentType = documentType;
        }

        /// <summary>
        /// Parameterless constructor for object initializer syntax (DocumentType must be set)
        /// </summary>
        public StrapiQueryRequest()
        {
        }
    }

    /// <summary>
    /// Options for populating relations
    /// </summary>
    public class PopulateOptions
    {
        /// <summary>
        /// Specific fields to populate
        /// </summary>
        public string[] Fields { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Nested populate options
        /// </summary>
        public Dictionary<string, PopulateOptions> Populate { get; set; } = new();

        /// <summary>
        /// Filters for the populated relation
        /// </summary>
        public Dictionary<string, object> Filters { get; set; } = new();

        /// <summary>
        /// Sort for the populated relation
        /// </summary>
        public string[] Sort { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Pagination for the populated relation
        /// </summary>
        public PaginationOptions Pagination { get; set; } = new();

        /// <summary>
        /// For dynamic zones and components - use "*" to populate all components
        /// </summary>
        public string PopulateAll { get; set; }

        /// <summary>
        /// Helper property to set populate all to "*"
        /// </summary>
        public bool PopulateAllComponents
        {
            set { if (value) PopulateAll = "*"; }
        }
    }

    /// <summary>
    /// Pagination options
    /// </summary>
    public class PaginationOptions
    {
        /// <summary>
        /// Number of entries per page
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// Page number (1-based)
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// Starting entry number
        /// </summary>
        public int? Start { get; set; }

        /// <summary>
        /// Number of entries to return
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Whether to include total count
        /// </summary>
        public bool? WithCount { get; set; }
    }

    /// <summary>
    /// Filter operators for Strapi queries
    /// </summary>
    public static class FilterOperators
    {
        public const string Equal = "$eq";
        public const string EqualCaseInsensitive = "$eqi";
        public const string NotEqual = "$ne";
        public const string NotEqualCaseInsensitive = "$nei";
        public const string LessThan = "$lt";
        public const string LessThanOrEqual = "$lte";
        public const string GreaterThan = "$gt";
        public const string GreaterThanOrEqual = "$gte";
        public const string In = "$in";
        public const string NotIn = "$notIn";
        public const string Contains = "$contains";
        public const string ContainsCaseInsensitive = "$containsi";
        public const string NotContains = "$notContains";
        public const string NotContainsCaseInsensitive = "$notContainsi";
        public const string StartsWith = "$startsWith";
        public const string StartsWithCaseInsensitive = "$startsWithi";
        public const string EndsWith = "$endsWith";
        public const string EndsWithCaseInsensitive = "$endsWithi";
        public const string Null = "$null";
        public const string NotNull = "$notNull";
        public const string Between = "$between";
        public const string And = "$and";
        public const string Or = "$or";
        public const string Not = "$not";
    }

    /// <summary>
    /// Helper class for building filter conditions
    /// </summary>
    public static class FilterBuilder
    {
        public static Dictionary<string, object> Equal(object value) =>
            new() { [FilterOperators.Equal] = value };

        public static Dictionary<string, object> EqualCaseInsensitive(string value) =>
            new() { [FilterOperators.EqualCaseInsensitive] = value };

        public static Dictionary<string, object> NotEqual(object value) =>
            new() { [FilterOperators.NotEqual] = value };

        public static Dictionary<string, object> LessThan(object value) =>
            new() { [FilterOperators.LessThan] = value };

        public static Dictionary<string, object> LessThanOrEqual(object value) =>
            new() { [FilterOperators.LessThanOrEqual] = value };

        public static Dictionary<string, object> GreaterThan(object value) =>
            new() { [FilterOperators.GreaterThan] = value };

        public static Dictionary<string, object> GreaterThanOrEqual(object value) =>
            new() { [FilterOperators.GreaterThanOrEqual] = value };

        public static Dictionary<string, object> In(params object[] values) =>
            new() { [FilterOperators.In] = values };

        public static Dictionary<string, object> NotIn(params object[] values) =>
            new() { [FilterOperators.NotIn] = values };

        public static Dictionary<string, object> Contains(string value) =>
            new() { [FilterOperators.Contains] = value };

        public static Dictionary<string, object> ContainsCaseInsensitive(string value) =>
            new() { [FilterOperators.ContainsCaseInsensitive] = value };

        public static Dictionary<string, object> StartsWith(string value) =>
            new() { [FilterOperators.StartsWith] = value };

        public static Dictionary<string, object> EndsWith(string value) =>
            new() { [FilterOperators.EndsWith] = value };

        public static Dictionary<string, object> IsNull() =>
            new() { [FilterOperators.Null] = true };

        public static Dictionary<string, object> IsNotNull() =>
            new() { [FilterOperators.NotNull] = true };

        public static Dictionary<string, object> Between(object start, object end) =>
            new() { [FilterOperators.Between] = new[] { start, end } };
    }

    /// <summary>
    /// Main serializer class for converting StrapiQueryRequest to query string
    /// </summary>
    public static class StrapiQuerySerializer
    {
        /// <summary>
        /// Serialize a StrapiQueryRequest to a query string
        /// </summary>
        /// <param name="request">The query request to serialize</param>
        /// <returns>URL-encoded query string</returns>
        public static string Serialize(StrapiQueryRequest request)
        {
            if (request == null)
                return string.Empty;

            var parameters = new List<string>();

            // Handle filters
            SerializeFilters(request.Filters, "filters", parameters);

            // Handle locale
            if (!string.IsNullOrEmpty(request.Locale))
            {
                parameters.Add($"locale={HttpUtility.UrlEncode(request.Locale)}");
            }

            // Handle status
            if (!string.IsNullOrEmpty(request.Status))
            {
                parameters.Add($"status={HttpUtility.UrlEncode(request.Status)}");
            }

            // Handle populate
            if (request.PopulateAll)
            {
                // Simple populate=* (populate everything 1 level deep)
                parameters.Add("populate=*");
            }
            else
            {
                // Complex populate with specific options
                SerializePopulate(request.Populate, "populate", parameters);
            }

            // Handle fields
            SerializeArray(request.Fields, "fields", parameters);

            // Handle sort
            SerializeArray(request.Sort, "sort", parameters);

            // Handle pagination
            SerializePagination(request.Pagination, "pagination", parameters);

            return string.Join("&", parameters);
        }

        /// <summary>
        /// Generate the complete API URL with base URL
        /// </summary>
        /// <param name="request">The query request</param>
        /// <param name="baseUrl">Base URL (e.g., "http://localhost:1337" or "https://api.mysite.com")</param>
        /// <returns>Complete API URL</returns>
        public static string ToUrl(StrapiQueryRequest request, string baseUrl = "http://localhost:1337")
        {
            if (request == null || string.IsNullOrWhiteSpace(request.DocumentType))
                throw new ArgumentException("Request and DocumentType are required");

            var queryString = Serialize(request);
            var url = $"{baseUrl.TrimEnd('/')}/{request.DocumentType}";

            return string.IsNullOrEmpty(queryString) ? url : $"{url}?{queryString}";
        }

        private static void SerializeFilters(Dictionary<string, object> filters, string prefix, List<string> parameters)
        {
            if (filters == null || !filters.Any())
                return;

            foreach (var filter in filters)
            {
                var key = $"{prefix}[{filter.Key}]";
                SerializeFilterValue(filter.Value, key, parameters);
            }
        }

        private static void SerializeFilterValue(object value, string key, List<string> parameters)
        {
            if (value == null)
                return;

            if (value is Dictionary<string, object> operators)
            {
                foreach (var op in operators)
                {
                    var operatorKey = $"{key}[{op.Key}]";
                    if (op.Value is Array array)
                    {
                        SerializeArray(array.Cast<object>().ToArray(), operatorKey, parameters);
                    }
                    else if (op.Value is Dictionary<string, object> nestedDict)
                    {
                        // Handle nested dictionaries recursively (e.g., FilterBuilder results)
                        SerializeFilterValue(nestedDict, operatorKey, parameters);
                    }
                    else
                    {
                        parameters.Add($"{operatorKey}={HttpUtility.UrlEncode(op.Value?.ToString() ?? "")}");
                    }
                }
            }
            else
            {
                parameters.Add($"{key}={HttpUtility.UrlEncode(value.ToString())}");
            }
        }

        private static void SerializePopulate(Dictionary<string, PopulateOptions> populate, string prefix, List<string> parameters)
        {
            if (populate == null || !populate.Any())
                return;

            foreach (var pop in populate)
            {
                var key = $"{prefix}[{pop.Key}]";
                SerializePopulateOptions(pop.Value, key, parameters);
            }
        }

        private static void SerializePopulateOptions(PopulateOptions options, string prefix, List<string> parameters)
        {
            if (options == null)
                return;

            // Handle PopulateAll for dynamic zones/components (e.g., populate[blocks][populate]=*)
            if (!string.IsNullOrEmpty(options.PopulateAll))
            {
                parameters.Add($"{prefix}[populate]={HttpUtility.UrlEncode(options.PopulateAll)}");
                return; // When using PopulateAll, other options are typically not used
            }

            // Handle fields
            if (options.Fields?.Any() == true)
            {
                SerializeArray(options.Fields, $"{prefix}[fields]", parameters);
            }

            // Handle nested populate
            if (options.Populate?.Any() == true)
            {
                SerializePopulate(options.Populate, $"{prefix}[populate]", parameters);
            }

            // Handle filters
            if (options.Filters?.Any() == true)
            {
                SerializeFilters(options.Filters, $"{prefix}[filters]", parameters);
            }

            // Handle sort
            if (options.Sort?.Any() == true)
            {
                SerializeArray(options.Sort, $"{prefix}[sort]", parameters);
            }

            // Handle pagination
            if (options.Pagination != null)
            {
                SerializePagination(options.Pagination, $"{prefix}[pagination]", parameters);
            }
        }

        private static void SerializeArray(object[] array, string prefix, List<string> parameters)
        {
            if (array == null || !array.Any())
                return;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null)
                {
                    parameters.Add($"{prefix}[{i}]={HttpUtility.UrlEncode(array[i].ToString())}");
                }
            }
        }

        private static void SerializePagination(PaginationOptions pagination, string prefix, List<string> parameters)
        {
            if (pagination == null)
                return;

            if (pagination.PageSize.HasValue)
            {
                parameters.Add($"{prefix}[pageSize]={pagination.PageSize.Value}");
            }

            if (pagination.Page.HasValue)
            {
                parameters.Add($"{prefix}[page]={pagination.Page.Value}");
            }

            if (pagination.Start.HasValue)
            {
                parameters.Add($"{prefix}[start]={pagination.Start.Value}");
            }

            if (pagination.Limit.HasValue)
            {
                parameters.Add($"{prefix}[limit]={pagination.Limit.Value}");
            }

            if (pagination.WithCount.HasValue)
            {
                parameters.Add($"{prefix}[withCount]={pagination.WithCount.Value.ToString().ToLower()}");
            }
        }
    }

    /// <summary>
    /// Extension methods for easier query building
    /// </summary>
    public static class StrapiQueryExtensions
    {
        /// <summary>
        /// Convert the query request to a query string
        /// </summary>
        public static string ToQueryString(this StrapiQueryRequest request)
        {
            return StrapiQuerySerializer.Serialize(request);
        }

        /// <summary>
        /// Convert the query request to a complete API URL
        /// </summary>
        /// <param name="request">The query request</param>
        /// <param name="baseUrl">Base URL (default: http://localhost:1337)</param>
        /// <returns>Complete API URL</returns>
        public static string ToUrl(this StrapiQueryRequest request, string baseUrl = "http://localhost:1337")
        {
            return StrapiQuerySerializer.ToUrl(request, baseUrl);
        }

        /// <summary>
        /// Add a filter to the request
        /// </summary>
        public static StrapiQueryRequest AddFilter(this StrapiQueryRequest request, string field, object value)
        {
            request.Filters[field] = value;
            return request;
        }

        /// <summary>
        /// Add a nested filter for relations (e.g., author.name, category.slug)
        /// </summary>
        public static StrapiQueryRequest AddRelationFilter(this StrapiQueryRequest request, string relation, string field, object value)
        {
            if (!request.Filters.ContainsKey(relation))
            {
                request.Filters[relation] = new Dictionary<string, object>();
            }

            if (request.Filters[relation] is Dictionary<string, object> relationFilters)
            {
                relationFilters[field] = value;
            }
            else
            {
                // If it's not a dictionary, replace it with one
                request.Filters[relation] = new Dictionary<string, object> { [field] = value };
            }

            return request;
        }

        /// <summary>
        /// Add multiple filters for a relation at once
        /// </summary>
        public static StrapiQueryRequest AddRelationFilters(this StrapiQueryRequest request, string relation, Dictionary<string, object> filters)
        {
            if (!request.Filters.ContainsKey(relation))
            {
                request.Filters[relation] = new Dictionary<string, object>();
            }

            if (request.Filters[relation] is Dictionary<string, object> relationFilters)
            {
                foreach (var filter in filters)
                {
                    relationFilters[filter.Key] = filter.Value;
                }
            }
            else
            {
                request.Filters[relation] = new Dictionary<string, object>(filters);
            }

            return request;
        }

        /// <summary>
        /// Add a populate option to the request
        /// </summary>
        public static StrapiQueryRequest AddPopulate(this StrapiQueryRequest request, string relation, PopulateOptions options)
        {
            request.Populate[relation] = options;
            return request;
        }

        /// <summary>
        /// Add simple populate (just the relation name)
        /// </summary>
        public static StrapiQueryRequest AddPopulate(this StrapiQueryRequest request, string relation)
        {
            request.Populate[relation] = new PopulateOptions();
            return request;
        }

        /// <summary>
        /// Add populate for dynamic zones/components (populate all)
        /// </summary>
        public static StrapiQueryRequest AddPopulateAll(this StrapiQueryRequest request, string relation)
        {
            request.Populate[relation] = new PopulateOptions { PopulateAll = "*" };
            return request;
        }

        /// <summary>
        /// Set fields to select
        /// </summary>
        public static StrapiQueryRequest WithFields(this StrapiQueryRequest request, params string[] fields)
        {
            request.Fields = fields;
            return request;
        }

        /// <summary>
        /// Set sort order
        /// </summary>
        public static StrapiQueryRequest WithSort(this StrapiQueryRequest request, params string[] sort)
        {
            request.Sort = sort;
            return request;
        }

        /// <summary>
        /// Set pagination
        /// </summary>
        public static StrapiQueryRequest WithPagination(this StrapiQueryRequest request, int? page = null, int? pageSize = null)
        {
            request.Pagination = new PaginationOptions
            {
                Page = page,
                PageSize = pageSize
            };
            return request;
        }

        /// <summary>
        /// Set locale
        /// </summary>
        public static StrapiQueryRequest WithLocale(this StrapiQueryRequest request, string locale)
        {
            request.Locale = locale;
            return request;
        }

        /// <summary>
        /// Set status
        /// </summary>
        public static StrapiQueryRequest WithStatus(this StrapiQueryRequest request, string status)
        {
            request.Status = status;
            return request;
        }

        /// <summary>
        /// Set document type (if not set in constructor)
        /// </summary>
        public static StrapiQueryRequest WithDocumentType(this StrapiQueryRequest request, string documentType)
        {
            request.DocumentType = documentType;
            return request;
        }

        /// <summary>
        /// Enable populate all (populate=*) - populates everything 1 level deep
        /// </summary>
        public static StrapiQueryRequest WithPopulateAll(this StrapiQueryRequest request)
        {
            request.PopulateAll = true;
            return request;
        }
    }
}
