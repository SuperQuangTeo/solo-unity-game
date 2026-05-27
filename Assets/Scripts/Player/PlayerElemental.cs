using System;
using UnityEngine;

public class PlayerElemental : MonoBehaviour,ISaveable
{
    public enum ElementalType { None, Fire, Water, Electric }
    public ElementalType currentElemental = ElementalType.None;

    public static event Action OnChangeElemental;

    public void UseElemental(int elementalIndex)
    {
        if ((int)currentElemental == elementalIndex)
        {
            return;
        }
        else
        {
            currentElemental = (ElementalType)elementalIndex;
            OnChangeElemental?.Invoke();
        }
    }

    public float getDamageBonus(float baseDamage)
    {
        switch (currentElemental)
        {
            case ElementalType.Fire:
                return baseDamage + 2;
            case ElementalType.Water:
                return baseDamage + 3.5f;
            case ElementalType.Electric:
                return baseDamage + 1.5f;
            default: return baseDamage;
        }
    }

    public void SaveData(ref GameData data)
    {
        data.element = (int)currentElemental;
    }

    public void LoadData(GameData data)
    {
        currentElemental = (ElementalType)data.element;
        OnChangeElemental?.Invoke();
    }
}
