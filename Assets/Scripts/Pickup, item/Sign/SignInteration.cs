using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SignInteration : MonoBehaviour
{
    public SignData signData;
    public GameObject panel;
    public TextMeshProUGUI title, description;

    private bool isPlayerInZone = false;

    void Start()
    {
        //title.text = signData.title;
        //description.text = getDescription();
        if (signData != null && signData.tutorialType == SignData.TutorialType.Tutorial1)
        {
            DisplayTutorial1AtBegining();
        }
        
    }

    public string getDescription()
    {
        switch (signData.tutorialType)
        {
            case SignData.TutorialType.Tutorial1:
                return Tutorial1Display();
            case SignData.TutorialType.Tutorial2:
                return Tutorial2Display();
            default:
                return signData.description;
        }
    }

    public string Tutorial1Display()
    {
        string runKey = InputManager.Instance.Controls.Player.Run.GetBindingDisplayString().ToUpper();
        string attackKey = InputManager.Instance.Controls.Player.Attack.GetBindingDisplayString().ToUpper();
        string jumpKey = InputManager.Instance.Controls.Player.Jump.GetBindingDisplayString().ToUpper();
        string rollKey = InputManager.Instance.Controls.Player.Roll.GetBindingDisplayString().ToUpper();
        string HealKey = InputManager.Instance.Controls.Player.Heal.GetBindingDisplayString().ToUpper();

        return $"Di chuyển - {runKey} \nTấn công - {attackKey} \nNhảy - {jumpKey} \nLướt/Lộn - {rollKey} \nHồi máu - Giữ {HealKey}";
    }

    public string Tutorial2Display()
    {
        string jumpKey = InputManager.Instance.Controls.Player.Jump.GetBindingDisplayString().ToUpper();
        string rollKey = InputManager.Instance.Controls.Player.Roll.GetBindingDisplayString().ToUpper();

        return $"Nhân vật có thể bám và nhảy trên tường.\n\nBật mí: Có thể kết hợp nhảy và lộn \n {jumpKey} + {rollKey}";
    }

    private IEnumerator DisplayTutorial1AtBeginingCoroutine()
    {
        this.title.text = signData.title;
        this.description.text = Tutorial1Display();

        panel.SetActive(true);
        yield return new WaitForSeconds(7f);
        if (!isPlayerInZone)
        {
            panel.SetActive(false);
        }
    }

    private void DisplayTutorial1AtBegining()
    {
        StartCoroutine(DisplayTutorial1AtBeginingCoroutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInZone = true;
            title.text = signData.title;
            description.text = getDescription();
            panel.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInZone = false;
            panel.SetActive(false);
        }
    }
}
