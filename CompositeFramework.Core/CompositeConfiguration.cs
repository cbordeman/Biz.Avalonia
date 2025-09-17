// namespace CompositeFramework.Core;
//
// public class CompositConfiguration
// {
//     public string ConnectionString { get; }
//     public int TimeoutSeconds { get; }
//     public bool EnableLogging { get; }
//     public IReadOnlyList<string> PluginPaths { get; }
//
//     internal CompositConfiguration(
//         string connectionString,
//         int timeoutSeconds,
//         bool enableLogging,
//         List<string> pluginPaths)
//     {
//         ConnectionString = connectionString;
//         TimeoutSeconds = timeoutSeconds;
//         EnableLogging = enableLogging;
//         PluginPaths = pluginPaths.AsReadOnly();
//     }
// }