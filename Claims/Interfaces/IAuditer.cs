namespace Claims.Interfaces
{
    /// <summary>
    /// Defines methods for recording audit events related to claims and covers in response to HTTP requests.
    /// </summary>
    public interface IAuditer
    {
        /// <summary>
        /// Asynchronously audits a claim with the specified identifier and HTTP request type.
        /// </summary>
        /// <param name="id">The unique identifier of the claim to audit. Cannot be null or empty.</param>
        /// <param name="httpRequestType">The HTTP request method (such as "GET", "POST", etc.) associated with the claim audit. Cannot be null or
        /// empty.</param>
        /// <returns>A task that represents the asynchronous audit operation.</returns>
        Task AuditClaimAsync(string id, string httpRequestType);

        /// <summary>
        /// Asynchronously audits a cover with the specified identifier and HTTP request type. 
        /// </summary>
        /// <param name="id">The unique identifier of the cover to audit. Cannot be null or empty.</param>
        /// <param name="httpRequestType">The HTTP request method (such as "GET", "POST", etc.) associated with the claim audit. Cannot be null or
        /// empty.</param>
        /// <returns>A task that represents the asynchronous audit operation.</returns>
        Task AuditCoverAsync(string id, string httpRequestType);
    }
}
