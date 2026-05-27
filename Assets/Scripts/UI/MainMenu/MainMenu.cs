using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.PlayMusic("MainMenu");
    }
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void PlaySFX()
    {
        AudioManager.Instance.PlaySFX("ButtonSFX");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
