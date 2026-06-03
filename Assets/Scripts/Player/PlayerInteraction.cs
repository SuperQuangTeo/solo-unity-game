using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    //private IInteractable currentNPC;
    //private NPCDialogue NPCDialogue;
    [SerializeField] private float interactionDistance = 2f;
    private movement playerMovement;
    private Rigidbody2D playerRigid;

    void Awake()
    {
        playerMovement = GetComponent<movement>();
        playerRigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D hitNPC = Physics2D.OverlapCircle(transform.position, interactionDistance, LayerMask.GetMask("NPC"));
        Collider2D hitChest = Physics2D.OverlapCircle(transform.position, interactionDistance, LayerMask.GetMask("Chest"));
        Collider2D hitDoor = Physics2D.OverlapCircle(transform.position, interactionDistance, LayerMask.GetMask("Door"));
        if (hitNPC != null)
        {
            var npcDialogueUI = hitNPC.GetComponent<NPCDialogueUI>();
            if (npcDialogueUI != null)
            {
                InteractionHintManager.Instance.ShowHint("Press E", hitNPC.transform);
            }
        }
        else if (hitChest != null)
        {
            var chestInteraction = hitChest.GetComponent<ChestInteraction>();
            if (chestInteraction != null)
            {
                InteractionHintManager.Instance.ShowHint("Press E", hitChest.transform);
            }
        }
        else if(hitDoor != null)
        {
            var doorInteraction = hitDoor.GetComponent<DoorInteraction>();
            if (doorInteraction != null)
            {
                InteractionHintManager.Instance.ShowHint("Press E", hitDoor.transform);
            }
        }
        else
        {
            InteractionHintManager.Instance.HideHint();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (hitNPC != null)
            {
                NPCDialogueUI npcDialogueUI = hitNPC.GetComponent<NPCDialogueUI>();
                if (npcDialogueUI is NormalNPC normalNPC)
                {
                    normalNPC.Interact();
                }
                else if (npcDialogueUI is Salesman salesman)
                {
                    salesman.Interact();
                }
                playerMovement.enabled = false;
                playerRigid.linearVelocity = Vector3.zero;
            }
            if(hitChest != null)
            {
                var chestInteraction = hitChest.GetComponent<ChestInteraction>();
                if (chestInteraction != null)
                {
                    chestInteraction.Interact();
                }
            }
            if(hitDoor != null)
            {
                var doorInteraction = hitDoor.GetComponent<DoorInteraction>();
                if (doorInteraction != null)
                {
                    doorInteraction.Interact();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (hitNPC != null)
            {
                NPCDialogueUI npcDialogueUI = hitNPC.GetComponent<NPCDialogueUI>();
                if (npcDialogueUI is NormalNPC normalNPC)
                {
                    normalNPC.NextLine();
                }
                else if (npcDialogueUI is Salesman salesman)
                {
                    salesman.NextLine();
                    if (salesman.IsDialogueEnded())
                    {
                        //Debug.Log("Dialogue ended, opening shop");
                        salesman.OpenShop();
                    }
                }
                if (hitNPC.GetComponent<NPCDialogueUI>().IsDialogueEnded())
                {
                    playerMovement.enabled = true;
                    playerRigid.simulated = true;
                }
            }
        }
    }

}
