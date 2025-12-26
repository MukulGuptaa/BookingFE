using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject loginRegisterScreen;
    [SerializeField] private GameObject lobbyScreen;
    
    public static MainSceneManager Instance { get; private set; }
    
    void Start()
    {
        Instance = this;
        if (UserData.Instance.IsLoggedIn) {
            loginRegisterScreen.SetActive(false); 
            lobbyScreen.SetActive(true);
        } else {
            loginRegisterScreen.SetActive(true);
            lobbyScreen.SetActive(false);
        }
    }

    public void OnAuthSuccess()
    {
        loginRegisterScreen.SetActive(false); 
        lobbyScreen.SetActive(true);
    }
    
}
