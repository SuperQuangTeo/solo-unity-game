using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private string startScene = "MainMenu";
    void Start()
    {
        SceneManager.LoadScene(startScene);
    }

}
