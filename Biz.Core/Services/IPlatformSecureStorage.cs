namespace Biz.Core.Services;

public interface ISafeStorage
{
    /// <summary>
    /// Gets and decrypts the value for a given key.
    /// </summary>
    /// <param name="key">The key to retrieve the value for.</param>
    /// <returns>The decrypted string value or <see langword="null"/> if a value was not found.</returns>
    Task<string?> GetAsync(string key);

    /// <summary>
    /// Sets and encrypts a value for a given key.
    /// </summary>
    /// <param name="key">The key to set the value for.</param>
    /// <param name="value">Value to set.</param>
    /// <returns>A <see cref="Task"/> object with the current status of the asynchronous operation.</returns>
    Task SetAsync(string key, string value);

    /// <summary>
    /// Removes a key and its associated value if it exists.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    bool Remove(string key);

    /// <summary>
    /// Removes all of the stored encrypted key/value pairs.
    /// </summary>
    void RemoveAll();
}