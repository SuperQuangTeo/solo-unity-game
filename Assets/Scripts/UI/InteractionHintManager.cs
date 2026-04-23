using TMPro;
using UnityEngine;

public class InteractionHintManager : MonoBehaviour
{
    public static InteractionHintManager Instance;

    [SerializeField] private GameObject hintUI;
    [SerializeField] private TextMeshProUGUI hintText;

    void Awake()
    {
        Instance = this;
    }

    public void ShowHint(string text, Transform target)
    {
        hintText.text = text;
        hintUI.transform.position = target.position + Vector3.up;
        hintUI.SetActive(true);
    }

    public void HideHint()
    {
        hintUI.SetActive(false);
    }
}
