using System.Net;

namespace StrapiRestClient.Models
{
    /// <summary>
    /// Represents a clean, simple result from a Strapi API call with all essential information.
    /// </summary>
    /// <typeparam name="T">The type of data returned from the API call.</typeparam>
    public class StrapiResult<T>
    {
        /// <summary>
        /// Gets or sets the data returned from the API call. 
        /// For collections, this will be the unwrapped collection (e.g., List&lt;Article&gt;).
        /// For single items, this will be the item itself (e.g., Article).
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the API call was successful (HTTP 2xx status).
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code returned by the API.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets a simple error message if the API call failed.
        /// This provides a quick way to access error information without navigating nested error objects.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets detailed error information from Strapi if the API call failed.
        /// This contains the full error object with status, name, message, and details.
        /// </summary>
        public StrapiError? Error { get; set; }

        /// <summary>
        /// Gets or sets metadata returned by Strapi, including pagination information for collections.
        /// This will be null for single item requests.
        /// </summary>
        public Meta? Meta { get; set; }

        /// <summary>
        /// Gets the total number of items available (from pagination metadata).
        /// Returns null if no pagination information is available.
        /// </summary>
        public int? TotalCount => Meta?.Pagination?.Total;

        /// <summary>
        /// Gets the current page number (from pagination metadata).
        /// Returns null if no pagination information is available.
        /// </summary>
        public int? CurrentPage => Meta?.Pagination?.Page;

        /// <summary>
        /// Gets the page size (from pagination metadata).
        /// Returns null if no pagination information is available.
        /// </summary>
        public int? PageSize => Meta?.Pagination?.PageSize;

        /// <summary>
        /// Gets the total number of pages (from pagination metadata).
        /// Returns null if no pagination information is available.
        /// </summary>
        public int? PageCount => Meta?.Pagination?.PageCount;
    }
}