using UnityEngine;

public class UserData
{
    private const string PREF_USER_ID = "USER_ID";
    private const string PREF_USERNAME = "USERNAME";
    private const string PREF_EMAIL = "EMAIL";
    private const string PREF_TOKEN = "TOKEN";

    private static UserData _instance;
    public static UserData Instance
    {
        get
        {
            if (_instance == null)
                _instance = new UserData();
            return _instance;
        }
    }

    public string UserId { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string Token { get; private set; }

    // Private constructor to ensure singleton usage
    private UserData() 
    {
        LoadData();
    }

    public void SetUser(string userId, string username, string email, string token)
    {
        UserId = userId;
        Username = username;
        Email = email;
        Token = token;
        
        SaveData();
    }

    public void ClearUser()
    {
        UserId = null;
        Username = null;
        Email = null;
        Token = null;
        
        PlayerPrefs.DeleteKey(PREF_USER_ID);
        PlayerPrefs.DeleteKey(PREF_USERNAME);
        PlayerPrefs.DeleteKey(PREF_EMAIL);
        PlayerPrefs.DeleteKey(PREF_TOKEN);
        PlayerPrefs.Save();
    }
    
    public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

    private void SaveData()
    {
        PlayerPrefs.SetString(PREF_USER_ID, UserId ?? "");
        PlayerPrefs.SetString(PREF_USERNAME, Username ?? "");
        PlayerPrefs.SetString(PREF_EMAIL, Email ?? "");
        PlayerPrefs.SetString(PREF_TOKEN, Token ?? "");
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        UserId = PlayerPrefs.GetString(PREF_USER_ID, null);
        Username = PlayerPrefs.GetString(PREF_USERNAME, null);
        Email = PlayerPrefs.GetString(PREF_EMAIL, null);
        Token = PlayerPrefs.GetString(PREF_TOKEN, null);
        
        // Handle empty strings if they were saved as ""
        if (string.IsNullOrEmpty(Token)) Token = null;
    }
}
