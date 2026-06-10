using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindButtonUI : MonoBehaviour
{
    [Header("Config Action")]
    [SerializeField] private InputActionReference actionReference;
    [SerializeField] private int bindingIndex = 0;

    [Header("UI")]
    [SerializeField] private TMP_Text displayTextField;
    [SerializeField] private GameObject listeningOverlay;

    private Button rebindButton;
    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    void Awake()
    {
        rebindButton = GetComponent<Button>();
    }

    void OnEnable()
    {
        rebindButton.onClick.AddListener(StartRebinding);
        UpdateUI();
    }

    void OnDisable()
    {
        rebindButton.onClick.RemoveListener(StartRebinding);
    }

    public void UpdateUI()
    {
        if (actionReference != null && displayTextField != null && InputManager.Instance != null)
        {
            InputAction action = InputManager.Instance.Controls.asset.FindAction(actionReference.action.id);
            if (action != null)
            {
                displayTextField.text = action.GetBindingDisplayString(bindingIndex);
            }
        }
    }

    private void StartRebinding()
    {
        if (actionReference == null) return;

        InputAction action = InputManager.Instance.Controls.asset.FindAction(actionReference.action.id);
        if (action == null) return;

        InputManager.Instance.Controls.Disable();

        if (listeningOverlay != null) listeningOverlay.SetActive(true);

        rebindOperation?.Cancel();

        rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("<Mouse>/delta")
            .WithControlsExcluding("<Mouse>/position")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => FinishRebinding(action))
            .OnCancel(operation => CancelRebindingProcess());

        rebindOperation.Start();
    }

    private void FinishRebinding(InputAction action)
    {
        rebindOperation?.Dispose();
        rebindOperation = null;

        if (listeningOverlay != null) listeningOverlay.SetActive(false);

        InputManager.Instance.Controls.Enable();

        UpdateUI();

        InputManager.Instance.SaveRebinds();
    }

    private void CancelRebindingProcess()
    {
        CleanUpOperation();
        UpdateUI();
    }

    private void CleanUpOperation()
    {
        rebindOperation?.Dispose();
        rebindOperation = null;

        if (listeningOverlay != null) listeningOverlay.SetActive(false);

        if (InputManager.Instance != null)
        {
            InputManager.Instance.Controls.Enable();
        }
    }
}
