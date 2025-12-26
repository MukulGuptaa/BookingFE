using TMPro;
using UnityEngine;
using Newtonsoft.Json;

public class RegisterSection : MonoBehaviour
{
    [SerializeField] public TMP_InputField username;
    [SerializeField] public TMP_InputField email;
    [SerializeField] public TMP_InputField phoneNumber;
    [SerializeField] public TMP_InputField password;
    [SerializeField] public TMP_Text infoText;

    public void RegisterButtonClicked()
    {
        if (string.IsNullOrEmpty(username.text) || 
            string.IsNullOrEmpty(email.text) || 
            string.IsNullOrEmpty(phoneNumber.text) || 
            string.IsNullOrEmpty(password.text))
        {
            infoText.text = "Please fill all fields";
            return;
        }

        infoText.text = "Registering...";

        RegisterRequest request = new RegisterRequest
        {
            username = username.text,
            email = email.text,
            phoneNumber = phoneNumber.text,
            password = password.text
        };

        APIManager.Instance.PostRequest<AuthResponse>("/auth/register", request, OnRegisterSuccess, OnRegisterError);
    }

    private void OnRegisterSuccess(AuthResponse response)
    {
        infoText.text = "Registration Successful!";
        UserData.Instance.SetUser(response._id, response.username, response.email, response.token);
        MainSceneManager.Instance.OnAuthSuccess();
    }

    private void OnRegisterError(string error)
    {
        infoText.text = $"Registration Failed: {error}";
    }

    private class RegisterRequest
    {
        public string username;
        public string email;
        public string phoneNumber;
        public string password;
    }
}
