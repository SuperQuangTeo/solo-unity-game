using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleLoading : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;

    void Start()
    {
        string sceneToLoad = GameManager.Instance.targetSceneName;
        StartCoroutine(LoadScene(sceneToLoad));
    }

    IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            loadingText.text = "Loading... " + Mathf.RoundToInt(progress * 100f) + "%";

            yield return null;
        }
    }
}
