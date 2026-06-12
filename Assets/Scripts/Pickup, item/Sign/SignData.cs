using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "SignData", menuName = "Scriptable Objects/SignData")]
public class SignData : ScriptableObject
{
    public enum TutorialType { None, Tutorial1, Tutorial2 }

    public TutorialType tutorialType;
    public string title = "Hướng dẫn";
    public string description;
}

