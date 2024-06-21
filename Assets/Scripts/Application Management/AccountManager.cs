using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using TMPro;
using Firebase.Auth;
using Unity.VisualScripting;

public class AccountManager : MonoBehaviour
{
    public GameObject chooseLoginMethod, loginScreen, homeScreen, messagePanel, passwordWarning, usernameWarning, emailWarning,
        confirmPasswordWarning, confirmPasswordObjects, emailObjects, usernameObjects, signUpInsteadPanel;
    public InputField usernameInput, emailInput, passwordInput, confirmPasswordInput;
    public TMP_Text messageText, headerText;
    public string playerName, playerID, accessToken, email, password;
    public Button loginButton, signUpButton, returnToSignInTypeButton;
    private FirebaseAuth auth;
    private FirebaseUser user;

    private async void Start()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
            return;
        else
        {
            homeScreen.SetActive(false);
            chooseLoginMethod.SetActive(true);
        }
        await UnityServices.InitializeAsync();
        Debug.Log(UnityServices.State);
        SetupEvents();
        InitializeFirebase(); await SignInAnonymouslyWithUnityAsync();
    }

    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                playerName = user.DisplayName ?? "";
                email = user.Email ?? "";
            }
        }
    }

    private void OnDestroy()
    {
        if (auth == null)
            return;
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    private void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            playerID = AuthenticationService.Instance.PlayerId;
            accessToken = AuthenticationService.Instance.AccessToken;
        };
        AuthenticationService.Instance.SignInFailed += (err) => Message(err.Message);
        AuthenticationService.Instance.SignedOut += () => Message("You've signed out successfully.");
        AuthenticationService.Instance.Expired += () => Message("Time Out.");
    }
    public void CreateNewFirebaseAccountWithEmailAndPassword()
    {
        //do not optimise, the call also triggers warnings and in the future, query the db to check if account exists while typing
        bool pcheck = ValidPasswordCheck();
        bool mCheck = ValidEmailCheck();
        bool cCheck = ConfirmPasswordCheck();

        if (!pcheck || !mCheck || !cCheck)
            return;

        password = confirmPasswordInput.text;
        email = emailInput.text;
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Message("Sign Up was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Message("Sign Up encountered an error: " + task.Exception);
                return;
            }
        });
        Message("Sign up successful");
        loginScreen.SetActive(false);
        homeScreen.SetActive(true);
    }

    public void SignInExistingFirebaseAccountWithEmailAndPassword()
    {
        //do not optimise, the call also triggers warnings and in the future, query the db to check if account exists while typing
        bool pcheck = ValidPasswordCheck();
        bool mCheck = ValidEmailCheck();

        if (!pcheck || !mCheck )
            return;

        password = passwordInput.text;
        email = emailInput.text;
        AuthResult result = new();
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Message("Sign in was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Message("Sign in encountered an error: " + task.Exception);
                return;
            }
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("OK");
            }
        }); 
        Message("Sign in successful");
        loginScreen.SetActive(false);
        homeScreen.SetActive(true);
    }
    public void SignInAnonymouslyToFirebase()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Message("Guest sign in was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Message("Guest sign in encountered an error: " + task.Exception);
                return;
            }
        });
        Message("Guest sign in successful");
        chooseLoginMethod.SetActive(false);
        loginScreen.SetActive(false);
        homeScreen.SetActive(true);
    }
    private async Task SignInAnonymouslyWithUnityAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");
            playerID = AuthenticationService.Instance.PlayerId;
        }
        catch (AuthenticationException ex)
        {
            Message(ex.Message);
        }
        catch (RequestFailedException ex)
        {
            Message(ex.Message);
        }
    }
    private async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Message("Sign in is successful.");
        }
        catch (AuthenticationException ex)
        {
            Message(ex.Message);
        }
        catch (RequestFailedException ex)
        {
            Message(ex.Message);
        }
    }
    private async Task LinkWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.LinkWithUnityAsync(accessToken);
            Message("Link is successful.");
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            Message("This user is already linked with another account. Log in instead.");
        }

        catch (AuthenticationException ex)
        {
            Message(ex.Message);
        }
        catch (RequestFailedException ex)
        {
            Message(ex.Message);
        }
    }
    private async Task UnlinkUnityAsync(string idToken)
    {
        try
        {
            await AuthenticationService.Instance.UnlinkUnityAsync();
            Message("Unlink is successful.");
        }
        catch (AuthenticationException ex)
        {
            Message(ex.Message);
        }
        catch (RequestFailedException ex)
        {
            Message(ex.Message);
        }
    }
    private async Task SignUpWithUnityUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Message("SignUp is successful.");
        }
        catch (AuthenticationException ex)
        {
            Message(ex.Message);
        }
        catch (RequestFailedException ex)
        {
            Message(ex.Message);
        }
    }
    private async Task SignInWithUnityUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Message("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            Message(ex.Message);
        }
        catch (RequestFailedException ex)
        {
            Message(ex.Message);
        }
    }
    private async Task AddUsernamePasswordWithUnityAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.AddUsernamePasswordAsync(username, password);
            Message("Username and password added.");
        }
        catch (AuthenticationException ex)
        {
            Message(ex.Message);
        }
        catch (RequestFailedException ex)
        {
            Message(ex.Message);
        }
    }
    private async Task UpdatePasswordWithUnityAsync(string currentPassword, string newPassword)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePasswordAsync(currentPassword, newPassword);
            Message("Password updated.");
        }
        catch (AuthenticationException ex)
        {
            Message(ex.Message);
        }
        catch (RequestFailedException ex)
        {
            Message(ex.Message);
        }
    }
    private void Message(string message)
    {
        if (messagePanel == null)
            return;
        messagePanel.SetActive(true);
        messageText.text = message;
    }
    private bool ValidEmailCheck()
    {
        if (!emailInput.text.Contains("@") || !emailInput.text.Contains(".com"))
        {
            emailWarning.SetActive(true);
            return false;
        }
        return true;
    }
    private bool ValidPasswordCheck()
    {
        if (passwordInput.text.Length < 8)
        {
            passwordWarning.SetActive(true);
            return false;
        }
        return true;
    }
    private bool ConfirmPasswordCheck()
    {
        if (passwordInput.text != confirmPasswordInput.text)
        {
            confirmPasswordWarning.SetActive(true);
            return false;
        }
        return true;
    }
    public void RemoveMessage() => messagePanel.SetActive(false);
    public void RemoveWarning()
    {
        passwordWarning.SetActive(false);
        usernameWarning.SetActive(false);
        emailWarning.SetActive(false);
        confirmPasswordWarning.SetActive(false);
    }
    public void SignInSetup(bool showMail)
    {
        chooseLoginMethod.SetActive(false);
        loginScreen.SetActive(true);
        signUpButton.gameObject.SetActive(false);
        loginButton.gameObject.SetActive(true);
        emailObjects.SetActive(showMail);
        usernameObjects.SetActive(!showMail);
        confirmPasswordObjects.SetActive(false);
        headerText.text = "SIGN IN";
    }
    public void SignUp()
    {
        SignInSetup(true);
        signUpButton.gameObject.SetActive(true);
        loginButton.gameObject.SetActive(false);
        confirmPasswordObjects.SetActive(true);
        headerText.text = "SIGN UP";
    }
    public void GoBacKToChooseLogin()
    {
        chooseLoginMethod.SetActive(true);
        loginScreen.SetActive(false);
    }
    public void DisableSignUpInsteadPanel() => signUpInsteadPanel.SetActive(false);
    public void SignUpInstead()
    {
        signUpInsteadPanel.SetActive(false);
        SignUp();
    }

    public async void VoidSignInAnonymouslyAsync()
    {
        await SignInAnonymouslyWithUnityAsync();
    }
    public async void SignUpWithUsernameAndPasswordAsync()
    {
        if (confirmPasswordInput.text != passwordInput.text)
            return;
        await SignInWithUnityUsernamePasswordAsync(usernameInput.text, passwordInput.text);
    }
}
