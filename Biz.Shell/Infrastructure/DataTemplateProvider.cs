// using Avalonia;
// using Avalonia.Controls;
// using Avalonia.Controls.Shapes;
// using Avalonia.Controls.Templates;
// using Avalonia.Data;
// using Avalonia.Media;
//
// namespace Biz.Shell.Infrastructure;
//
// // Path-Data taken from here: https://icons.getbootstrap.com
// public static class DataTemplateProvider<T> where T: class
// {
//     // This FuncDataTemplate can be static, as it will not change over time.
//     public static FuncDataTemplate<T> GenderDataTemplate { get; }
//         = new FuncDataTemplate<T>(
//             // Check if we have a valid object and return true if it is valid. 
//             model => (T? model) is not null,
//
//             // Avalonia will provide the Person automatically as the functions parameter.
//             BuildGenderPresenter);
//
//
//     // This private function will return a control that represents our persons sex as a gender symbol.
//     private static Control BuildGenderPresenter(Type modelType)
//     {
//         return Locator.Current.Resolve(modelType);
//     }
// }