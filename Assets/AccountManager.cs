using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
public class AccountManager : MonoBehaviour
{
    public GameObject loginScreen, signUpInsteadPanel, messagePanel, passwordWarning, usernameWarning;
    public InputField usernameInput, passwordInput;
    public TMP_Text messageText, headerText;
    public string playerName, playerID, accessToken, email, password;
    public Button loginButton, signUpButton, guestButton;
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        Debug.Log(UnityServices.State);
        SetupEvents();
    }
    public void RemoveMessage() => messagePanel.SetActive(false);
    public void DisableSignUp()
    {
        signUpButton.gameObject.SetActive(false);
        loginButton.gameObject.SetActive(true);
        headerText.text = "LOGIN";
        signUpInsteadPanel.SetActive(false);
    }
    public void RemoveWarning()
    {
        passwordWarning.SetActive(false);
        usernameWarning.SetActive(false);
    }

    public void SignUp()
    {
        signUpInsteadPanel.SetActive(false);
        signUpButton.gameObject.SetActive(true);
        loginButton.gameObject.SetActive(false);
        headerText.text = "SIGN UP";
    }
    public void ToggleSignUpSignIn()
    {
        if(signUpButton.gameObject.activeInHierarchy)
            DisableSignUp();
        else
            SignUp();
    }
    public async void VoidSignInAnonymouslyAsync()
    {
        await SignInAnonymouslyAsync();
    }    
    public async void SignInWithUsernameAndPasswordAsync()
    {
        bool shouldReturn = false;
        if (!usernameInput.text.Contains("@") || !usernameInput.text.Contains(".com"))
        {
            shouldReturn = true;
            usernameWarning.SetActive(true);
        }    
        if (passwordInput.text.Length < 8)
        {
            shouldReturn = true;
            passwordWarning.SetActive(true);    
        }
        if(shouldReturn)
            return;
        await SignInWithUsernamePasswordAsync(usernameInput.text, passwordInput.text);
    }
    public async void SignUpWithUsernameAndPasswordAsync()
    {
        bool shouldReturn = false;
        if (usernameInput.text.Length == 0)
        {
            shouldReturn = true;
            usernameWarning.SetActive(true);
        }
        if (passwordInput.text.Length < 8)
        {
            shouldReturn = true;
            passwordWarning.SetActive(true);
        }
        if (shouldReturn)
            return;
        await SignUpWithUsernamePasswordAsync(usernameInput.text, passwordInput.text);
    }
    private void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            //playerName = AuthenticationService.Instance.GetPlayerNameAsync().Result;
            playerID = AuthenticationService.Instance.PlayerId;
            accessToken = AuthenticationService.Instance.AccessToken;
        };
        AuthenticationService.Instance.SignInFailed += (err) => Message(err.Message);
        AuthenticationService.Instance.SignedOut += () => Message("You've signed out successfully.");
        AuthenticationService.Instance.Expired += () => Message("Time Out.");
    }

    private async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Message("Sign in anonymously succeeded!");
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
    private async Task SignUpWithUsernamePasswordAsync(string username, string password)
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
    private async Task SignInWithUsernamePasswordAsync(string username, string password)
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
    private async Task AddUsernamePasswordAsync(string username, string password)
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
    private async Task UpdatePasswordAsync(string currentPassword, string newPassword)
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
        messagePanel.SetActive(true);
        messageText.text = message;
    }
}
