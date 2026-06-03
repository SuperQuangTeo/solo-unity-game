using UnityEngine;

public class Salesman : NPCDialogueUI, ISalesman
{
    [SerializeField]private ShopUI shopUI;
    public void OpenShop()
    {
        shopUI.OpenShop();
    }


}
