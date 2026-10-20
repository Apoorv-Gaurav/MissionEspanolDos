
using UnityEngine;
using UnityEngine.SceneManagement;

public class L4_BackButton : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
