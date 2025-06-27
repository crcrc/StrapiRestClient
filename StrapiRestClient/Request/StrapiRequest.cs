using StrapiRestClient.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrapiRestClient.Request
{
    /// <summary>
    /// Represents a request to the Strapi API.
    /// </summary>
    public class StrapiRequest
    {
        private readonly PopulateBuilder _populateRoot = new();

        private static readonly Dictionary<FilterType, string> FilterTypeMappings = new()
        {
            { FilterType.In, "$in" },
            { FilterType.NotIn, "$notIn" },
            { FilterType.EqualTo, "$eq" },
            { FilterType.NotEqualTo, "$ne" },
            { FilterType.LessThan, "$lt" },
            { FilterType.LessThanOrEqualTo, "$lte" },
            { FilterType.GreaterThan, "$gt" },
            { FilterType.GreaterThanOrEqualTo, "$gte" },
            { FilterType.Contains, "$contains" },
            { FilterType.DoesNotContain, "$notContains" },
            { FilterType.StartsWith, "$startsWith" },
            { FilterType.EndsWith, "$endsWith" },
            { FilterType.IsNull, "$null" },
            { FilterType.IsNotNull, "$notNull" }
        };

        /// <summary>
        /// Gets the HTTP method for the request.
        /// </summary>
        public RequestMethod Method { get; private set; }

        /// <summary>
        /// Gets the content type of the request.
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// Gets the path of the request.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the filters for the request.
        /// </summary>
        public List<RequestFilter> Filters { get; private set; }

        /// <summary>
        /// Gets the body of the request.
        /// </summary>
        public object? Body { get; private set; }

        /// <summary>
        /// Gets the populate object for the request.
        /// </summary>
        public object? PopulateObject
        {
            get
            {
                var obj = _populateRoot.ToObject();
                // Only return the populate object if it's not just a default "*" when no actual populate was requested.
                // This prevents adding ?populate=* to every request by default.
                if (obj is string s && s == "*" && _populateRoot.Children.Count == 0 && (_populateRoot.Fields == null || _populateRoot.Fields.Count == 0))
                {
                    return null;
                }
                return obj;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StrapiRequest"/> class.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="path">The path.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="body">The body.</param>
        public StrapiRequest(
            RequestMethod method,
            string contentType,
            string path = "",
            List<RequestFilter>? filters = null,
            object? body = null)
        {
            Method = method;
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Filters = filters ?? new List<RequestFilter>();
            Body = body;
        }

        /// <summary>
        /// Creates a new GET request.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <param name="path">The path.</param>
        /// <param name="filters">The filters.</param>
        /// <returns>A new <see cref="StrapiRequest"/> instance.</returns>
        public static StrapiRequest Get(string contentType, string path = "", List<RequestFilter>? filters = null)
        {
            return new StrapiRequest(RequestMethod.Get, contentType, path, filters);
        }

        /// <summary>
        /// Creates a new POST request.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <param name="body">The body.</param>
        /// <param name="path">The path.</param>
        /// <returns>A new <see cref="StrapiRequest"/> instance.</returns>
        public static StrapiRequest Post(string contentType, object body, string path = "")
        {
            return new StrapiRequest(RequestMethod.Post, contentType, path, body: body);
        }

        /// <summary>
        /// Creates a new PUT request.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <param name="body">The body.</param>
        /// <param name="path">The path.</param>
        /// <returns>A new <see cref="StrapiRequest"/> instance.</returns>
        public static StrapiRequest Put(string contentType, object body, string path = "")
        {
            return new StrapiRequest(RequestMethod.Put, contentType, path, body: body);
        }

        /// <summary>
        /// Creates a new DELETE request.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <param name="path">The path.</param>
        /// <returns>A new <see cref="StrapiRequest"/> instance.</returns>
        public static StrapiRequest Delete(string contentType, string path = "")
        {
            return new StrapiRequest(RequestMethod.Delete, contentType, path);
        }

        /// <summary>
        /// Adds a filter to the request.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        /// <returns>The current <see cref="StrapiRequest"/> instance.</returns>
        public StrapiRequest WithFilter(RequestFilter? filter)
        {
            if (filter != null)
            {
                Filters.Add(filter.Value);
            }
            return this;
        }

        /// <summary>
        /// Adds a filter to the request.
        /// </summary>
        /// <param name="type">The filter type.</param>
        /// <param name="field">The field to filter by.</param>
        /// <param name="value">The filter value.</param>
        /// <returns>The current <see cref="StrapiRequest"/> instance.</returns>
        public StrapiRequest WithFilter(FilterType type, string field, string value)
        {
            if (FilterTypeMappings.TryGetValue(type, out var filterString))
            {
                Filters.Add(new RequestFilter
                {
                    Type = filterString,
                    Field = field,
                    Value = value
                });
            }
            return this;
        }

        /// <summary>
        /// Sets the page number for the request.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <returns>The current <see cref="StrapiRequest"/> instance.</returns>
        public StrapiRequest WithPage(int pageNumber)
        {
            if (pageNumber <= 0)
                throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

            Filters.Add(new RequestFilter { Type = "pagination", Field = "page", Value = pageNumber.ToString() });
            return this;
        }

        /// <summary>
        /// Sets the page size for the request.
        /// </summary>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The current <see cref="StrapiRequest"/> instance.</returns>
        public StrapiRequest WithPageSize(int pageSize)
        {
            if (pageSize <= 0)
                throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

            Filters.Add(new RequestFilter { Type = "pagination", Field = "pageSize", Value = pageSize.ToString() });
            return this;
        }

        /// <summary>
        /// Adds a sort parameter to the request.
        /// </summary>
        /// <param name="field">The field to sort by.</param>
        /// <param name="direction">The sort direction.</param>
        /// <returns>The current <see cref="StrapiRequest"/> instance.</returns>
        public StrapiRequest WithSort(string field, SortDirection direction)
        {
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Field cannot be null or empty", nameof(field));

            var sortValue = direction == SortDirection.Ascending ? "asc" : "desc";
            Filters.Add(new RequestFilter { Type = "sort", Field = field, Value = sortValue });
            return this;
        }

        /// <summary>
        /// Adds a random sort parameter to the request.
        /// </summary>
        /// <returns>The current <see cref="StrapiRequest"/> instance.</returns>
        public StrapiRequest WithRandomSort()
        {
            Filters.Add(new RequestFilter { Type = "randomSort", Field = string.Empty, Value = "true" });
            return this;
        }

        /// <summary>
        /// Adds a populate parameter to the request.
        /// </summary>
        /// <param name="relation">The relation to populate.</param>
        /// <returns>The current <see cref="StrapiRequest"/> instance.</returns>
        public StrapiRequest WithPopulate(string relation)
        {
            if (string.IsNullOrWhiteSpace(relation))
                throw new ArgumentException("Relation cannot be null or empty", nameof(relation));

            _populateRoot.Populate(relation);
            return this;
        }

        /// <summary>
        /// Adds an equal filter to the request.
        /// </summary>
        /// <param name="field">The field to filter by.</param>
        /// <param name="value">The filter value.</param>
        /// <returns>The current <see cref="StrapiRequest"/> instance.</returns>
        public StrapiRequest WithEqual(string field, string value)
        {
            return WithFilter(FilterType.EqualTo, field, value);
        }

        /// <summary>
        /// Adds a select field parameter to the request.
        /// </summary>
        /// <param name="field">The field to select.</param>
        /// <returns>The current <see cref="StrapiRequest"/> instance.</returns>
        public StrapiRequest WithSelectField(string field)
        {
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Field cannot be null or empty", nameof(field));

            Filters.Add(new RequestFilter { Type = "fields", Field = field, Value = string.Empty });
            return this;
        }

        /// <summary>
        /// Adds select field parameters to the request.
        /// </summary>
        /// <param name="fields">The fields to select.</param>
        /// <returns>The current <see cref="StrapiRequest"/> instance.</returns>
        public StrapiRequest WithSelectFields(params string[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            foreach (var field in fields.Where(f => !string.IsNullOrWhiteSpace(f)))
            {
                WithSelectField(field);
            }
            return this;
        }

        /// <summary>
        /// Adds populate field parameters to the request.
        /// </summary>
        /// <param name="relation">The relation to populate.</param>
        /// <param name="fields">The fields to populate.</param>
        /// <returns>The current <see cref="StrapiRequest"/> instance.</returns>
        public StrapiRequest WithPopulateFields(string relation, params string[] fields)
        {
            if (string.IsNullOrWhiteSpace(relation))
                throw new ArgumentException("Relation cannot be null or empty", nameof(relation));

            if (fields == null || fields.Length == 0)
                throw new ArgumentException("Fields cannot be null or empty", nameof(fields));

            _populateRoot.Populate(relation).SelectFields(fields);
            return this;
        }

        /// <summary>
        /// Adds a populate all parameter to the request.
        /// </summary>
        /// <returns>The current <see cref="StrapiRequest"/> instance.</returns>
        public StrapiRequest WithPopulateAll()
        {
            _populateRoot.PopulateAll();
            return this;
        }

        /// <summary>
        /// Clears all filters from the request.
        /// </summary>
        /// <returns>The current <see cref="StrapiRequest"/> instance.</returns>
        public StrapiRequest ClearFilters()
        {
            Filters.Clear();
            return this;
        }
    }
}