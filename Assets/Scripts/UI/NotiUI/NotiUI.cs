using System.Collections;
using UnityEngine;

public class NotiUI : MonoBehaviour
{
    public GameObject notiPanel;
    private void OnEnable()
    {
        Boss1Health.OnBossDeath += ActivePanel;
    }

    private void OnDisable()
    {
        Boss1Health.OnBossDeath -= ActivePanel;
    }

    private void ActivePanel()
    {
        StartCoroutine(WaitForBossDeath());
    }

    private IEnumerator WaitForBossDeath()
    {
        yield return new WaitForSeconds(3f);
        notiPanel.SetActive(true);
        AudioManager.Instance.PlayMusic("EndGame");
    }
}
