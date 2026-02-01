using UnityEngine;

public class FlowerPickup : MonoBehaviour
{
    public LilyInteraction lily;   // Assign Lily in Inspector

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            lily.GiveFlower();
            Destroy(gameObject);
        }
    }
}
