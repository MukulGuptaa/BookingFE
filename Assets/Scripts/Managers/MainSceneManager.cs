using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject loginRegisterScreen;
    [SerializeField] private GameObject lobbyScreen;
    [SerializeField] private GameObject bookingScreen;
    
    public static MainSceneManager Instance { get; private set; }
    
    void Start()
    {
        Instance = this;
        if (UserData.Instance.IsLoggedIn) {
            loginRegisterScreen.SetActive(false); 
            bookingScreen.SetActive(true);
        } else {
            loginRegisterScreen.SetActive(true);
            bookingScreen.SetActive(false);
        }
    }

    public void OnAuthSuccess()
    {
        loginRegisterScreen.SetActive(false); 
        bookingScreen.SetActive(true);
    }
    
    
    
}
