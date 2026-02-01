using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SpriteRenderer))]
public class BuildingSortHandler : MonoBehaviour
{
    private SpriteRenderer sr;

    public int defaultOrder = 0;
    public int behindBuildingOffset = -10;
    public int frontBuildingOffset = 10;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Building"))
            return;

        Bounds b = other.bounds;

        float buildingTop = b.max.y;
        float buildingBottom = b.min.y;
        float playerY = transform.position.y;

        if (playerY < buildingBottom)
        {
            // In front of building
            sr.sortingOrder = defaultOrder + frontBuildingOffset;
        }
        else if (playerY > buildingTop)
        {
            // Behind building
            sr.sortingOrder = defaultOrder + behindBuildingOffset;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Building"))
            return;

        // Reset when leaving building
        sr.sortingOrder = defaultOrder;
    }
}
