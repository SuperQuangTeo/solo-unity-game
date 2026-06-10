using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public PlayerControls Controls { get; private set; }

    private const string RebindsPrefKey = "PlayerInputOverrides";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Controls = new PlayerControls();
            LoadRebinds();
            Controls.Enable();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveRebinds()
    {
        string rebindsJson = Controls.asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(RebindsPrefKey, rebindsJson);
        PlayerPrefs.Save();
    }

    public void LoadRebinds()
    {
        if (PlayerPrefs.HasKey(RebindsPrefKey))
        {
            string rebindsJson = PlayerPrefs.GetString(RebindsPrefKey);
            Controls.asset.LoadBindingOverridesFromJson(rebindsJson);
        }
    }

    public void ResetAllRebinds()
    {
        Controls.asset.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(RebindsPrefKey);
        SaveRebinds();
    }
}
