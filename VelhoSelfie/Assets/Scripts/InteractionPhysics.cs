using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPhysics : MonoBehaviour
{
    private static readonly LayerMask interactionLayerMask = ~(
        (1 << LayerMask.NameToLayer("Ignore Raycast")) |
        (1 << LayerMask.NameToLayer("Player")) |
        (1 << LayerMask.NameToLayer("InvisibleGround"))
    );

    public static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    // the ray hits the interactive if its closest point is at most at
    // maxDistance distance from the origin
    public static Interactive GetHitInteractive(Ray ray, float maxDistance)
    {
        const float DISTANCE_BUFF = 2.5f;
        // cast the ray
        bool didHit = Physics.Raycast(
            ray,
            out RaycastHit hit,
            maxDistance + DISTANCE_BUFF,
            interactionLayerMask,
            QueryTriggerInteraction.Ignore  // ignore triggers
        );
        if (!didHit) return null;

        hit.collider.gameObject.TryGetComponent(out InteractionCollider interactionCollider);
        // check if the hit object is interactive
        if (
            interactionCollider == null ||
            interactionCollider.parentInteractive == null ||
            !interactionCollider.parentInteractive.IsInteractive
        ) return null;


        Vector3 closestPoint = Physics.ClosestPoint(
            ray.origin,
            hit.collider,
            hit.collider.bounds.center,
            hit.collider.transform.rotation
        );
        float sqrDistToClosestPoint = Vector3.SqrMagnitude(ray.origin - closestPoint);
        // check the distance to the closest point of the interactive
        if (sqrDistToClosestPoint > maxDistance * maxDistance) return null;

        return interactionCollider.parentInteractive;
    }
}
