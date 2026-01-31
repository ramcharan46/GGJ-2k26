using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;
    private Rigidbody2D targetRb;

    [Header("Follow")]
    public float baseSmoothTime = 0.15f;
    public float catchUpSmoothTime = 0.25f;
    private Vector3 velocity;

    [Header("Dead Zone")]
    public Vector2 deadZoneSize = new Vector2(1.2f, 0.8f);

    [Header("Look Ahead")]
    public float maxLookAheadDistance = 3f;
    public float lookAheadSmoothTime = 0.12f;
    private Vector3 currentLookAhead;
    private Vector3 lookAheadVelocity;

    void Start()
    {
        if (target != null)
            targetRb = target.GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = transform.position;

        // DEAD ZONE
        Vector3 toTarget = target.position - transform.position;

        if (Mathf.Abs(toTarget.x) > deadZoneSize.x)
            desiredPos.x = target.position.x - Mathf.Sign(toTarget.x) * deadZoneSize.x;

        if (Mathf.Abs(toTarget.y) > deadZoneSize.y)
            desiredPos.y = target.position.y - Mathf.Sign(toTarget.y) * deadZoneSize.y;

        // LOOK AHEAD
        Vector2 vel = (targetRb != null) ? targetRb.linearVelocity : Vector2.zero;
        Vector3 targetLookAhead = vel.normalized * maxLookAheadDistance;

        // Prevent backward drift — only look ahead when moving
        if (vel.magnitude < 0.1f) targetLookAhead = Vector3.zero;

        currentLookAhead = Vector3.SmoothDamp(
            currentLookAhead,
            targetLookAhead,
            ref lookAheadVelocity,
            lookAheadSmoothTime
        );

        desiredPos += currentLookAhead;

        // APPLY SMOOTH DAMP
        float smooth = vel.magnitude > 0.1f ? baseSmoothTime : catchUpSmoothTime;
        desiredPos.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref velocity,
            smooth
        );
    }
}
