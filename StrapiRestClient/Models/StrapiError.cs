namespace StrapiRestClient.Models
{
    /// <summary>
    /// Represents an error returned by the Strapi API.
    /// </summary>
    public class StrapiError
    {
        /// <summary>
        /// Gets or sets the HTTP status code of the error.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the name of the error.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets additional details about the error.
        /// </summary>
        public object? Details { get; set; }
    }

    /// <summary>
    /// Represents a response containing a Strapi error.
    /// </summary>
    public class StrapiErrorResponse
    {
        /// <summary>
        /// Gets or sets the Strapi error.
        /// </summary>
        public StrapiError? Error { get; set; }
    }
}
