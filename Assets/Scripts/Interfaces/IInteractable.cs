using UnityEngine;

public interface IInteractable
{
    public void Interact();
    public void NextLine();
    public bool IsDialogueEnded()
    {
        return false;
    }
}
