using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    public GameObject toRoomPoint;
    public void Interact()
    {
        DoorManager.Instance.ChangeRoom(toRoomPoint);
    }

    public void NextLine()
    {
        throw new System.NotImplementedException();
    }
}
