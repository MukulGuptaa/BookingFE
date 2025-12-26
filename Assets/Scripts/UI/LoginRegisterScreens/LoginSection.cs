using TMPro;
using UnityEngine;
using Newtonsoft.Json;

public class LoginSection : MonoBehaviour
{
    [SerializeField] public TMP_InputField phoneOrEmail;
    [SerializeField] public TMP_InputField password;
    [SerializeField] public TMP_Text infoText;

    public void LoginButtonClicked()
    {
        if (string.IsNullOrEmpty(phoneOrEmail.text) || string.IsNullOrEmpty(password.text))
        {
            infoText.text = "Please fill all fields";
            return;
        }

        infoText.text = "Logging in...";

        LoginRequest request = new LoginRequest();
        request.password = password.text;

        if (phoneOrEmail.text.Contains("@"))
        {
            request.email = phoneOrEmail.text;
        }
        else
        {
            request.phoneNumber = phoneOrEmail.text;
        }

        APIManager.Instance.PostRequest<AuthResponse>("/auth/login", request, OnLoginSuccess, OnLoginError);
    }

    private void OnLoginSuccess(AuthResponse response)
    {
        infoText.text = "Login Successful!";
        UserData.Instance.SetUser(response._id, response.username, response.email, response.token);
        MainSceneManager.Instance.OnAuthSuccess();
    }

    private void OnLoginError(string error)
    {
        infoText.text = $"Login Failed: {error}";
    }

    private class LoginRequest
    {
        public string email;
        public string phoneNumber;
        public string password;
    }
}
