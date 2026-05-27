using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ElementIconUI : MonoBehaviour
{
    public Image iconImage;
    public List<ElementIcon> elementIcons;

    private PlayerElemental playerElemental;

    void Awake()
    {
        playerElemental = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerElemental>();
    }

    private void UpdateElementIconUI()
    {
        for (int i = 0; i < elementIcons.Count; i++)
        {
            if (elementIcons[i].name == playerElemental.currentElemental.ToString())
            {
                iconImage.sprite = elementIcons[i].icon;
            }
        }
    } 

    private void OnEnable()
    {
        PlayerElemental.OnChangeElemental += UpdateElementIconUI;
    }
    private void OnDisable()
    {
        PlayerElemental.OnChangeElemental -= UpdateElementIconUI;
    }
}
[System.Serializable]
public class ElementIcon
{
    public Sprite icon;
    public string name;
}
