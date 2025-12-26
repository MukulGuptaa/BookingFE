using UnityEngine;
using Newtonsoft.Json;

public class LoginRegisterScreen : MonoBehaviour
{
    [SerializeField] private LoginSection loginSection;
    [SerializeField] private RegisterSection registerSection;

    
    public static LoginRegisterScreen Instance;
    
    private void Start()
    {
        Instance = this;
        ChooseRegister();
    }

    public void ChooseLogin()
    {
        registerSection.gameObject.SetActive(false);
        loginSection.gameObject.SetActive(true);
    }

    public void ChooseRegister()
    {
        registerSection.gameObject.SetActive(true);
        loginSection.gameObject.SetActive(false);
    }
}

// Shared Response Class
public class AuthResponse
{
    [JsonProperty("_id")]
    public string _id;
    public string username;
    public string email;
    public string token;
}
