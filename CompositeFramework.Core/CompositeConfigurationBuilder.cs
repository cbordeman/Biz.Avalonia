// namespace CompositeFramework.Core;
//
// // Fluent builder for CompositConfiguration
// public class CompositConfigurationBuilder
// {
//     private string connectionString;
//     private int timeoutSeconds = 30; // default value
//     private bool enableLogging = false; // default value
//     private List<string> pluginPaths = new();
//     private Func<Type, object> serviceResolver;
//     
//     public CompositConfigurationBuilder SetConnectionString(string connStr)
//     {
//         connectionString = connStr;
//         return this;
//     }
//     
//     public CompositConfigurationBuilder SetTimeout(int seconds)
//     {
//         timeoutSeconds = seconds;
//         return this;
//     }
//
//     public CompositConfigurationBuilder EnableLogging(bool enabled = true)
//     {
//         enableLogging = enabled;
//         return this;
//     }
//
//     public CompositConfigurationBuilder AddPluginPath(string path)
//     {
//         pluginPaths.Add(path);
//         return this;
//     }
//
//     public CompositConfiguration Build()
//     {
//         if (string.IsNullOrWhiteSpace(connectionString))
//             throw new InvalidOperationException("ConnectionString is required");
//
//         return new CompositConfiguration(
//             connectionString,
//             timeoutSeconds,
//             enableLogging,
//             pluginPaths);
//     }
// }
