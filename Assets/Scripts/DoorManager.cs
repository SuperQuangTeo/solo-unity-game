using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public static DoorManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void ChangeRoom(GameObject toRoomPoint)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = toRoomPoint.transform.position;
        }
    }
}
