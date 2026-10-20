
using UnityEngine;
using UnityEngine.SceneManagement;

public class L2_BackButton : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
