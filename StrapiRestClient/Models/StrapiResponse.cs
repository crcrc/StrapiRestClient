using System.Net;

namespace StrapiRestClient.Models
{
    /// <summary>
    /// Represents a response from the Strapi API, encapsulating either data or an error.
    /// </summary>
    /// <typeparam name="T">The type of the data expected in a successful response.</typeparam>
    public class StrapiResponse<T>
    {
        /// <summary>
        /// Gets or sets the deserialized data from a successful response.
        /// This will be null if the request was not successful.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets or sets the error information from an unsuccessful response.
        /// This will be null if the request was successful.
        /// </summary>
        public StrapiError? Error { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code of the response.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets a value indicating whether the request was successful (HTTP status code 2xx).
        /// </summary>
        public bool IsSuccess => (int)StatusCode >= 200 && (int)StatusCode <= 299;
    }
}
