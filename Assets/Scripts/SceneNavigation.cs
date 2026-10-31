using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigation : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("overworld");
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("credits");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("mainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
