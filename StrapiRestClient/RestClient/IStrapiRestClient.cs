using StrapiRestClient.Models;
using StrapiRestClient.Request;
using System.Threading.Tasks;

namespace StrapiRestClient.RestClient
{
    /// <summary>
    /// Defines the contract for a Strapi REST client.
    /// </summary>
    public interface IStrapiRestClient
    {
        /// <summary>
        /// Executes a Strapi API request and returns the response.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the successful response data into.</typeparam>
        /// <param name="request">The Strapi request to execute.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A <see cref="StrapiResponse{T}"/> containing the deserialized data or error information.</returns>
        Task<StrapiResponse<T>> ExecuteAsync<T>(StrapiRequest request, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Executes a request and returns the raw JSON response (useful for model generation with "Paste JSON as Classes").
        /// </summary>
        /// <param name="request">The request to execute.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the raw JSON string.</returns>
        Task<string> GetRawJsonAsync(StrapiRequest request, CancellationToken cancellationToken = default);
    }
}
