using UnityEngine;

public class Salesman : NPCDialogueUI, ISalesman
{
    [SerializeField]private ShopUI shopUI;
    public void OpenShop()
    {
        Debug.Log("OpenShop called");
        shopUI.OpenShop();
    }


}
