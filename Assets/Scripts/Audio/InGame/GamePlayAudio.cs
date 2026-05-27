using UnityEngine;

public class GamePlayAudio : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.PlayMusic("InGame");
    }

    void Update()
    {
        
    }
}
