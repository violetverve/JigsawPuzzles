using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("UIScene"); 
    }
}
