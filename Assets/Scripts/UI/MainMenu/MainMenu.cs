using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject popConfirmPanel;
    void Start()
    {
        AudioManager.Instance.PlayMusic("MainMenu");
    }
    public void PlayGame()
    {
        if (GameManager.Instance.IsFileJSONExits())
        {
            popConfirmPanel.SetActive(true);
            return;
        }
        StartGameWithLoadingScene();
    }

    public void NewGame()
    {
        GameManager.Instance.DeleteFileJSON();
        StartGameWithLoadingScene();
    }
    public void ContinueGame()
    {
        StartGameWithLoadingScene();
    }

    public void PlaySFX()
    {
        AudioManager.Instance.PlaySFX("ButtonSFX");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGameWithLoadingScene()
    {
        GameManager.Instance.LoadNewSceneWithLoading("InGame");
    }
}
