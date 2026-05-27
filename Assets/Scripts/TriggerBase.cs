using UnityEngine;
using UnityEngine.Events;

public class TriggerBase : MonoBehaviour
{
    [Header("Settings")]
    public UnityEvent onTriggerActivated;
    public bool destroyOnUse = true;

    public void Activate()
    {
        onTriggerActivated?.Invoke();

        //if (destroyOnUse)
        //{
        //    gameObject.SetActive(false);
        //}
    }
}