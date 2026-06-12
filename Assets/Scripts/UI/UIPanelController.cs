using UnityEngine;
using UnityEngine.InputSystem;

public class UIPanelController : MonoBehaviour
{
    [SerializeField] private GameObject parentPanel;
    [SerializeField] private GameObject childrenPanel;
    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.Controls.UI.Cancel.performed += OnEscPressed;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.Controls.UI.Cancel.performed -= OnEscPressed;
        }
    }

    private void OnEscPressed(InputAction.CallbackContext ctx)
    {
        if (gameObject.activeInHierarchy)
        {
            ClosePanel();
        }
    }

    public void ClosePanel()
    {
        if (parentPanel != null && childrenPanel != null && childrenPanel.activeSelf)
        {
            parentPanel.SetActive(true);
            childrenPanel.SetActive(false);
            return;
        }

        gameObject.SetActive(false);

        if (parentPanel != null)
        {
            parentPanel.SetActive(true);
        }
    }
}
