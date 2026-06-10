using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 2f;
    private PlayerMovement playerMovement;
    private Rigidbody2D playerRigid;

    private Collider2D currentHit;
    private int combinedLayerMask;

    private string interactionKeyDisplay;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerRigid = GetComponent<Rigidbody2D>();
        combinedLayerMask = LayerMask.GetMask("NPC", "Chest", "Door");
    }
    private void OnEnable()
    {
        InputManager.Instance.Controls.Player.Interaction.performed += OnInteractionInput;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.Controls.Player.Interaction.performed -= OnInteractionInput;
        }
    }

    void Update()
    {
        currentHit = Physics2D.OverlapCircle(transform.position, interactionDistance, combinedLayerMask);
        interactionKeyDisplay = InputManager.Instance.Controls.Player.Interaction.GetBindingDisplayString().ToUpper();

        if (currentHit != null)
        {
            int objectLayer = currentHit.gameObject.layer;

            if (objectLayer == LayerMask.NameToLayer("NPC") && currentHit.GetComponent<NPCDialogueUI>() != null)
            {
                InteractionHintManager.Instance.ShowHint("Press " + interactionKeyDisplay, currentHit.transform);
            }
            else if (objectLayer == LayerMask.NameToLayer("Chest") && currentHit.GetComponent<ChestInteraction>() != null)
            {
                InteractionHintManager.Instance.ShowHint("Press " + interactionKeyDisplay, currentHit.transform);
            }
            else if (objectLayer == LayerMask.NameToLayer("Door") && currentHit.GetComponent<DoorInteraction>() != null)
            {
                InteractionHintManager.Instance.ShowHint("Press " + interactionKeyDisplay, currentHit.transform);
            }
            else
            {
                InteractionHintManager.Instance.HideHint();
            }
        }
        else
        {
            InteractionHintManager.Instance.HideHint();
        }
    }

    private void OnInteractionInput(InputAction.CallbackContext ctx)
    {
        HandleUnifiedInteraction();
    }

    private void HandleUnifiedInteraction()
    {
        if (currentHit == null) return;

        int objectLayer = currentHit.gameObject.layer;

        if (objectLayer == LayerMask.NameToLayer("NPC"))
        {
            NPCDialogueUI npcDialogueUI = currentHit.GetComponent<NPCDialogueUI>();
            if (npcDialogueUI == null) return;

            if (!playerMovement.enabled)
            {
                if (npcDialogueUI is NormalNPC normalNPC)
                {
                    normalNPC.NextLine();
                }
                else if (npcDialogueUI is Salesman salesman)
                {
                    salesman.NextLine();
                    if (salesman.IsDialogueEnded())
                    {
                        salesman.OpenShop();
                    }
                }
                if (npcDialogueUI.IsDialogueEnded())
                {
                    playerMovement.enabled = true;
                    playerRigid.simulated = true;
                }
            }
            else
            {
                if (npcDialogueUI is NormalNPC normalNPC)
                {
                    normalNPC.Interact();
                }
                else if (npcDialogueUI is Salesman salesman)
                {
                    salesman.Interact();
                }
                playerMovement.enabled = false;
                playerRigid.linearVelocity = Vector2.zero;
            }
        }

        else if (objectLayer == LayerMask.NameToLayer("Chest"))
        {
            currentHit.GetComponent<ChestInteraction>()?.Interact();
        }

        else if (objectLayer == LayerMask.NameToLayer("Door"))
        {
            currentHit.GetComponent<DoorInteraction>()?.Interact();
        }
    }

}
