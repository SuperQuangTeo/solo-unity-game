using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public Button saveButton;
    public Button loadButton;

    void Start()
    {
        if (saveButton != null)
            saveButton.onClick.AddListener(() => GameManager.Instance.SaveGame());

        if (loadButton != null)
            loadButton.onClick.AddListener(() => GameManager.Instance.LoadGame());
    }

    void Update()
    {
        DisableButton();
    }

    public void DisableButton()
    {
        if (EnemySpawnerByWave.Instance.IsRoomChallengeActive)
        {
            saveButton.interactable = false;
            loadButton.interactable = false;
        }
        else
        {
            saveButton.interactable = true;
            loadButton.interactable = true;
        }
    }

}
