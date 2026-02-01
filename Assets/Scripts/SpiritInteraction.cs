using UnityEngine;

public class SpiritInteraction : MonoBehaviour
{
    private bool playerInRange = false;
    private bool hasInteracted = false;

    public DialogueData dialogueData;
    public DialogueData finishDialogue;
    public DialogueManager dialogueManager;

    public Transform player;
    public Transform finishPoint;     // Assign FinishPoint in Inspector

    public float followSpeed = 3f;
    public float followDistance = 1.5f;
    public float finishRadius = 1f;

    void Update()
    {
        // First interaction
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && 
            !dialogueManager.IsDialogueActive && !hasInteracted)
        {
            dialogueManager.StartDialogue(dialogueData);
            hasInteracted = true;
        }

        // Follow player
        if (hasInteracted && player != null)
        {
            FollowPlayer();
        }

        // Check if PLAYER is at FinishPoint
        bool playerAtFinishPoint = Vector2.Distance(player.position, finishPoint.position) <= finishRadius;

        // Second interaction
        if (playerAtFinishPoint && hasInteracted && Input.GetKeyDown(KeyCode.E) && 
            !dialogueManager.IsDialogueActive)
        {
            dialogueManager.StartDialogue(finishDialogue);
            Destroy(gameObject, 0.2f);
        }
    }

    void FollowPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > followDistance)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                followSpeed * Time.deltaTime
            );
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
