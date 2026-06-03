using UnityEngine;
using UnityEngine.UI;

public enum EffectType { Buff, Debuff }

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Status Effects/New Effect")]
public class StatusEffectData : ScriptableObject
{
    public EffectType EffectType;
    public string effectName;
    public Sprite iconSprite;
    public Image iconImage;
    public float duration;
}
