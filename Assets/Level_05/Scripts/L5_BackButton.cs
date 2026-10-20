
using UnityEngine;
using UnityEngine.SceneManagement;

public class L5_BackButton : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
