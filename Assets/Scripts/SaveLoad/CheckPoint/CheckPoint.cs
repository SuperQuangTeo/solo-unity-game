using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public enum CheckPointType { Small, Large }
    [SerializeField]private bool isActiveCheckPoint = false;
    public CheckPointType type;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ActivateCheckPoint();
        }
    }

    private void ActivateCheckPoint()
    {
        if (isActiveCheckPoint) return;
        isActiveCheckPoint = true;
        Vector3 checkPointPos = transform.position;
        GameManager.Instance.UpdateCheckPointPos(checkPointPos);
        if (type == CheckPointType.Small)
        {
            GameManager.Instance.UpdateDataInMemoryOnly();
        }
        else if (type == CheckPointType.Large && isActiveCheckPoint)
        {
            GameManager.Instance.SaveGame();
        }
    }
}
