/// <summary>
/// Represents an HTTP request error
/// </summary>
public class HttpNotFoundObjectException : IOException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpNotFoundObjectException"/> class.
    /// </summary>
    /// <param name="message">The message to associate with this exception.</param>
    /// <param name="statusCode">The HTTP status code to associate with this exception.</param>
    public HttpNotFoundObjectException(string message, int statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpNotFoundObjectException"/> class with the <see cref="StatusCode"/> set to 404 Not Found.
    /// </summary>
    /// <param name="message">The message to associate with this exception</param>
    public HttpNotFoundObjectException(string message)
        : base(message)
    {
        StatusCode = StatusCodes.Status404NotFound;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpNotFoundObjectException"/> class.
    /// </summary>
    /// <param name="message">The message to associate with this exception.</param>
    /// <param name="statusCode">The HTTP status code to associate with this exception.</param>
    /// <param name="innerException">The inner exception to associate with this exception</param>
    public HttpNotFoundObjectException(string message, int statusCode, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpNotFoundObjectException"/> class with the <see cref="StatusCode"/> set to 404 Not Found.
    /// </summary>
    /// <param name="message">The message to associate with this exception</param>
    /// <param name="innerException">The inner exception to associate with this exception</param>
    public HttpNotFoundObjectException(string message, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = StatusCodes.Status404NotFound;
    }

    /// <summary>
    /// Gets the HTTP status code for this exception.
    /// </summary>
    public int StatusCode { get; }
}
