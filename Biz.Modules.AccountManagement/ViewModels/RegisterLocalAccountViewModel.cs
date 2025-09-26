namespace Biz.Modules.AccountManagement.ViewModels
{
    public class RegisterLocalAccountViewModel : PageViewModelBase
    {
        //     #region Fields
        //     private readonly Dictionary<string, string> errorMessages = new Dictionary<string, string>();
        //     #endregion
        //
        //     #region Properties
        //     public string Username
        //     {   
        //         get => username;
        //         set
        //         {
        //             if (username != value)
        //             {
        //                 username = value;
        //                 OnPropertyChanged();
        //                 ValidateUsername();
        //                 OnPropertyChanged(nameof(CanRegister));
        //                 OnPropertyChanged(nameof(HasUsernameError));
        //             }
        //         }
        //     }
        //
        //     public string Password
        //     {
        //         get => password;
        //         set
        //         {
        //             if (password != value)
        //             {
        //                 password = value;
        //                 OnPropertyChanged();
        //                 ValidatePassword();
        //                 // If confirm password is already filled, validate it again
        //                 if (!string.IsNullOrEmpty(confirmPassword))
        //                 {
        //                     ValidateConfirmPassword();
        //                 }
        //                 OnPropertyChanged(nameof(CanRegister));
        //                 OnPropertyChanged(nameof(HasPasswordError));
        //             }
        //         }
        //     }
        //
        //     public string ConfirmPassword
        //     {
        //         get => confirmPassword;
        //         set
        //         {
        //             if (confirmPassword != value)
        //             {
        //                 confirmPassword = value;
        //                 OnPropertyChanged();
        //                 ValidateConfirmPassword();
        //                 OnPropertyChanged(nameof(CanRegister));
        //                 OnPropertyChanged(nameof(HasConfirmPasswordError));
        //             }
        //         }
        //     }
        //
        //     public bool IsProcessing
        //     {
        //         get => isProcessing;
        //         set
        //         {
        //             if (isProcessing != value)
        //             {
        //                 isProcessing = value;
        //                 OnPropertyChanged();
        //                 OnPropertyChanged(nameof(CanRegister));
        //                 (RegisterCommand as RelayCommand)?.RaiseCanExecuteChanged();
        //                 (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
        //             }
        //         }
        //     }
        //
        //     public Dictionary<string, string> ErrorMessages => errorMessages;
        //
        //     public bool HasUsernameError => errorMessages.ContainsKey(nameof(Username)) && !string.IsNullOrEmpty(errorMessages[nameof(Username)]);
        //     public bool HasPasswordError => errorMessages.ContainsKey(nameof(Password)) && !string.IsNullOrEmpty(errorMessages[nameof(Password)]);
        //     public bool HasConfirmPasswordError => errorMessages.ContainsKey(nameof(ConfirmPassword)) && !string.IsNullOrEmpty(errorMessages[nameof(ConfirmPassword)]);
        //
        //     public bool CanRegister => !IsProcessing && 
        //                             !HasUsernameError && !string.IsNullOrEmpty(Username) &&
        //                             !HasPasswordError && !string.IsNullOrEmpty(Password) &&
        //                             !HasConfirmPasswordError && !string.IsNullOrEmpty(ConfirmPassword);
        //     #endregion
        //
        //     #region Commands
        //     public ICommand RegisterCommand { get; }
        //     public ICommand CancelCommand { get; }
        //     #endregion
        //
        //     #region Constructor
        //     public RegisterLocalAccountViewModel()
        //     {
        //         RegisterCommand = new RelayCommand(async () => await RegisterAsync(), () => CanRegister);
        //         CancelCommand = new RelayCommand(ClearForm, () => !IsProcessing);
        //     }
        //     #endregion
        //
        //     #region Methods
        //     private void ValidateUsername()
        //     {
        //         // Clear previous error
        //         errorMessages.Remove(nameof(Username));
        //
        //         if (string.IsNullOrWhiteSpace(Username))
        //         {
        //             errorMessages[nameof(Username)] = "Username is required";
        //             return;
        //         }
        //
        //         if (Username.Length < 3)
        //         {
        //             errorMessages[nameof(Username)] = "Username must be at least 3 characters long";
        //             return;
        //         }
        //
        //         if (Username.Length > 50)
        //         {
        //             errorMessages[nameof(Username)] = "Username cannot exceed 50 characters";
        //             return;
        //         }
        //
        //         if (!Regex.IsMatch(Username, @"^[a-zA-Z0-9._-]+$"))
        //         {
        //             errorMessages[nameof(Username)] = "Username can only contain letters, numbers, dots, hyphens, and underscores";
        //             return;
        //         }
        //
        //         // You could add a check for username availability here if backend service is available
        //     }
        //
        //     private void ValidatePassword()
        //     {
        //         // Clear previous error
        //         errorMessages.Remove(nameof(Password));
        //
        //         if (string.IsNullOrWhiteSpace(Password))
        //         {
        //             errorMessages[nameof(Password)] = "Password is required";
        //             return;
        //         }
        //
        //         if (Password.Length < 8)
        //         {
        //             errorMessages[nameof(Password)] = "Password must be at least 8 characters long";
        //             return;
        //         }
        //
        //         if (!Regex.IsMatch(Password, @"[A-Z]"))
        //         {
        //             errorMessages[nameof(Password)] = "Password must contain at least one uppercase letter";
        //             return;
        //         }
        //
        //         if (!Regex.IsMatch(Password, @"[a-z]"))
        //         {
        //             errorMessages[nameof(Password)] = "Password must contain at least one lowercase letter";
        //             return;
        //         }
        //
        //         if (!Regex.IsMatch(Password, @"[0-9]"))
        //         {
        //             errorMessages[nameof(Password)] = "Password must contain at least one digit";
        //             return;
        //         }
        //
        //         if (!Regex.IsMatch(Password, @"[^a-zA-Z0-9]"))
        //         {
        //             errorMessages[nameof(Password)] = "Password must contain at least one special character";
        //             return;
        //         }
        //     }
        //
        //     private void ValidateConfirmPassword()
        //     {
        //         // Clear previous error
        //         errorMessages.Remove(nameof(ConfirmPassword));
        //
        //         if (string.IsNullOrWhiteSpace(ConfirmPassword))
        //         {
        //             errorMessages[nameof(ConfirmPassword)] = "Please confirm your password";
        //             return;
        //         }
        //
        //         if (ConfirmPassword != Password)
        //         {
        //             errorMessages[nameof(ConfirmPassword)] = "Passwords do not match";
        //             return;
        //         }
        //     }
        //
        //     private async Task RegisterAsync()
        //     {
        //         if (!CanRegister)
        //             return;
        //
        //         try
        //         {
        //             IsProcessing = true;
        //
        //             // TODO: Implement actual registration logic here
        //             // Example: await _accountService.RegisterAsync(Username, Password);
        //
        //             // Simulate API call delay
        //             await Task.Delay(1500);
        //
        //             // TODO: On successful registration, handle navigation or show success message
        //             ClearForm();
        //         }
        //         catch (Exception ex)
        //         {
        //             // TODO: Handle registration errors properly
        //             System.Diagnostics.Debug.WriteLine($"Registration error: {ex.Message}");
        //         }
        //         finally
        //         {
        //             IsProcessing = false;
        //         }
        //     }
        //
        //     private void ClearForm()
        //     {
        //         Username = string.Empty;
        //         Password = string.Empty;
        //         ConfirmPassword = string.Empty;
        //         errorMessages.Clear();
        //         OnPropertyChanged(nameof(ErrorMessages));
        //         OnPropertyChanged(nameof(HasUsernameError));
        //         OnPropertyChanged(nameof(HasPasswordError));
        //         OnPropertyChanged(nameof(HasConfirmPasswordError));
        //     }
        //     #endregion
        //
        //     #region INotifyPropertyChanged
        //     public event PropertyChangedEventHandler PropertyChanged;
        //
        //     protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //     {
        //         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //     }
        //     #endregion
        //
        //     #region IDataErrorInfo
        //     public string Error => string.Empty;
        //
        //     public string this[string columnName]
        //     {
        //         get
        //         {
        //             // Ensure validation runs when binding engine requests validation
        //             switch (columnName)
        //             {
        //                 case nameof(Username):
        //                     ValidateUsername();
        //                     break;
        //                 case nameof(Password):
        //                     ValidatePassword();
        //                     break;
        //                 case nameof(ConfirmPassword):
        //                     ValidateConfirmPassword();
        //                     break;
        //             }
        //
        //             return errorMessages.TryGetValue(columnName, out string error) ? error : string.Empty;
        //         }
        //     }
        //     #endregion
        // }
        //
        // #region Helper Command Classes
        // public class RelayCommand : ICommand
        // {
        //     private readonly Action execute;
        //     private readonly Func<bool> canExecute;
        //
        //     public RelayCommand(Action execute, Func<bool> canExecute = null)
        //     {
        //         this.execute = execute ?? throw new ArgumentChecker(nameof(execute));
        //         this.canExecute = canExecute;
        //     }
        //
        //     public bool CanExecute(object parameter) => canExecute?.Invoke() ?? true;
        //
        //     public void Execute(object parameter) => execute();
        //
        //     public event EventHandler CanExecuteChanged;
        //
        //     public void RaiseCanExecuteChanged() => 
        //         CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        // }
        //
        // public class RelayCommand<T> : ICommand
        // {
        //     private readonly Action<T> execute;
        //     private readonly Func<T, bool> canExecute;
        //
        //     public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        //     {
        //         this.execute = execute ?? throw new ArgumentChecker(nameof(execute));
        //         this.canExecute = canExecute;
        //     }
        //
        //     public bool CanExecute(object parameter) => 
        //         canExecute?.Invoke(parameter is T typedParameter ? typedParameter : default) ?? true;
        //
        //     public void Execute(object parameter) => 
        //         execute(parameter is T typedParameter ? typedParameter : default);
        //
        //     public event EventHandler CanExecuteChanged;
        //
        //     public void RaiseCanExecuteChanged() => 
        //         CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        // }
        // #endregion

        public override string Area => "Account";
    }
}
