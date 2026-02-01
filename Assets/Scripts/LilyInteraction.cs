using UnityEngine;
using TMPro;

public class LilyInteraction : MonoBehaviour
{
    private bool playerAtEndPoint = false;

    private bool playerInRange = false;
    private bool hasInteracted = false;
    private bool uiShown = false;

    public DialogueData dialogueData;
    public DialogueData finishDialogue;
    public DialogueManager dialogueManager;

    public Transform player;

    [Header("Path Points")]
    public Transform[] middlePoints;   // Size = 3
    public Transform endPoint;
    public float pointRadius = 1f;

    public float followSpeed = 3f;
    public float followDistance = 1.5f;

    [Header("Flower Settings")]
    public int flowersNeeded = 3;
    private int currentFlowers = 0;

    private int visitedCount = 0;
    private bool readyForEnd = false;

    [Header("UI")]
    public GameObject questUI;
    public TextMeshProUGUI flowerCountText;   // Assign in Inspector


    void Update()
    {
        // First interaction
        if (playerInRange && Input.GetKeyDown(KeyCode.E) &&
            !dialogueManager.IsDialogueActive && !hasInteracted)
        {
            dialogueManager.StartDialogue(dialogueData);
            hasInteracted = true;

            SpawnUI();
            UpdateFlowerUI();
        }

        // Follow player
        if (hasInteracted && player != null)
        {
            FollowPlayer();
        }

        if (!hasInteracted) return;

        // Check middle points
        CheckMiddlePoints();

        // Check end point interaction (PLAYER must overlap)
        if (readyForEnd && playerAtEndPoint)
            {   
                if (Input.GetKeyDown(KeyCode.E) && !dialogueManager.IsDialogueActive)
                {
                    dialogueManager.StartDialogue(finishDialogue);
                    CleanupUI();
                    Destroy(gameObject, 0.2f);
                }
            }


        // Hide UI when dialogue ends
        if (uiShown && !dialogueManager.IsDialogueActive)
        {
            questUI.SetActive(false);
            uiShown = false;
        }
    }

    void SpawnUI()
    {
        if (questUI != null)
        {
            questUI.SetActive(true);
            uiShown = true;
        }
    }

    void CleanupUI()
    {
        if (questUI != null)
            questUI.SetActive(false);
    }

    void CheckMiddlePoints()
    {
        for (int i = 0; i < middlePoints.Length; i++)
        {
            if (middlePoints[i] == null) continue;

            if (IsPlayerAtPoint(middlePoints[i]))
            {
                middlePoints[i] = null;
                visitedCount++;
                Debug.Log("Visited point: " + visitedCount);
            }
        }

        if (visitedCount >= 3 && currentFlowers >= flowersNeeded)
        {
            readyForEnd = true;
        }
    }

    bool IsPlayerAtPoint(Transform point)
    {
        return Vector2.Distance(player.position, point.position) <= pointRadius;
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

    if (other.CompareTag("EndPoint"))
        playerAtEndPoint = true;
}

void OnTriggerExit2D(Collider2D other)
{
    if (other.CompareTag("Player"))
        playerInRange = false;

    if (other.CompareTag("EndPoint"))
        playerAtEndPoint = false;
}


    public void GiveFlower()
    {
        currentFlowers++;
        Debug.Log("Flowers: " + currentFlowers + "/" + flowersNeeded);

        UpdateFlowerUI();

        if (visitedCount >= 3 && currentFlowers >= flowersNeeded)
        {
            readyForEnd = true;
        }
    }

    void UpdateFlowerUI()
    {
        if (flowerCountText != null)
            flowerCountText.text = currentFlowers + " / " + flowersNeeded;
    }
}