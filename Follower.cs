using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform target;  // The object to follow
    public float followDistance = 2f; // The fixed distance to maintain

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (target == null) return; // Ensure target exists

        // Get the direction in LOCAL space
        Vector3 direction = target.localPosition - transform.localPosition;

        // Check if the distance is greater than followDistance
        if (direction.magnitude > followDistance)
        {
            // Amgle Update
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(0, 0, angle);
            // Normalize direction to preserve direction without affecting magnitude
            direction = direction.normalized;

            // Compute the new local position at the exact followDistance
            Vector3 targetLocalPosition = target.localPosition - (direction * followDistance);

            // Move smoothly toward the calculated position in LOCAL space
            // transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPosition, Time.deltaTime * moveSpeed);
            transform.localPosition = targetLocalPosition;

        }
    }
}
