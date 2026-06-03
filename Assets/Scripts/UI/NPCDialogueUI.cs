using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogueUI : MonoBehaviour, IInteractable
{
    public NPCDialogue npcDialogue;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText, nameText;
    public Image portraitImage;

    private int currentDialogueIndex;
    private bool isDialogueActive = false;

    public void Interact()
    {
        if(npcDialogue == null)
        {
            return;
        }
        else if (!isDialogueActive)
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        isDialogueActive = true;
        currentDialogueIndex = 0;
        nameText.text = npcDialogue.npcName;
        portraitImage.sprite = npcDialogue.npcPortrait;
        dialoguePanel.SetActive(true);
        dialogueText.text = npcDialogue.dialogueLines[0];
    }

    public void NextLine()
    {

        currentDialogueIndex++;
        if(currentDialogueIndex < npcDialogue.dialogueLines.Length)
        {
            dialogueText.text = npcDialogue.dialogueLines[currentDialogueIndex];
        }
        else
        {
            dialoguePanel.SetActive(false);
            isDialogueActive = false;
        }
    }
    
    public bool IsDialogueEnded()
    {
        return !isDialogueActive;
    }

}
