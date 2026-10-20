
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level01");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level02");
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene("L05_Scene");
    }

    public void LoadLevel4()
    {
        SceneManager.LoadScene("L04_Scene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
