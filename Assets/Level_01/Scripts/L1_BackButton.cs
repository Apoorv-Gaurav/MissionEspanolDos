
using UnityEngine;
using UnityEngine.SceneManagement;

public class L1_BackButton : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
