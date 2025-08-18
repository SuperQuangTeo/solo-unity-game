using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    //private IInteractable currentNPC;
    //private NPCDialogue NPCDialogue;
    [SerializeField] private float interactionDistance = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionDistance, LayerMask.GetMask("NPC"));
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (hit != null)
            {
                NPCDialogueUI npcDialogueUI = hit.GetComponent<NPCDialogueUI>();
                if (npcDialogueUI is NormalNPC normalNPC)
                {
                    normalNPC.Interact();
                }
                else if (npcDialogueUI is Salesman salesman)
                {
                    salesman.Interact();
                }

            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (hit != null)
            {
                NPCDialogueUI npcDialogueUI = hit.GetComponent<NPCDialogueUI>();
                if (npcDialogueUI is NormalNPC normalNPC)
                {
                    normalNPC.NextLine();
                }
                else if (npcDialogueUI is Salesman salesman)
                {
                    salesman.NextLine();
                    if (salesman.IsDialogueEnded())
                    {
                        Debug.Log("Dialogue ended, opening shop");
                        salesman.OpenShop();
                    }
                }
            }
        }
    }

}
