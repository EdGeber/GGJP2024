using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField]
    Transform positionTarget;

    [SerializeField]
    Transform rotationTarget;

    [Tooltip(
        "How smoothly the transform follows the positionTarget's localPosition, separately for horizontal and vertical movement."
    )]
    [SerializeField]
    [Range(0f, 1f)]
    float horizontalDamping = 0.8f;

    [SerializeField]
    [Range(0f, 1f)]
    float verticalDamping = 0.8f;

    [SerializeField]
    [Tooltip("How smoothly the transform follows the positionTarget's orientation")]
    [Range(0f, 1f)]
    float rotationDamping = 0f;
    public Vector3 offset;

    Vector3 newPosition = new Vector3();

    [Button]
    void UpdateTransform()
    {
        if (positionTarget != null)
        {
            transform.localPosition = positionTarget.localPosition + offset;
        }
        if (rotationTarget != null)
        {
            transform.rotation = rotationTarget.rotation;
        }
    }

    private void Reset()
    {
        UpdateTransform();
    }

    void Start()
    {
        if (positionTarget == null)
            Debug.LogWarning(
                "A Follows component has been added to the GameObject but no positionTarget has been assigned."
            );
        UpdateTransform();
    }

    void LerpPosition()
    {
        newPosition.x = Mathf.Lerp(
            transform.localPosition.x,
            positionTarget.localPosition.x + offset.x,
            1f - horizontalDamping
        );
        newPosition.y = Mathf.Lerp(
            transform.localPosition.y,
            positionTarget.localPosition.y + offset.y,
            1f - verticalDamping
        );
        newPosition.z = Mathf.Lerp(
            transform.localPosition.z,
            positionTarget.localPosition.z + offset.z,
            1f - horizontalDamping
        );
        transform.localPosition = newPosition;
    }

    void LerpOrientation()
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotationTarget.rotation,
            1f - rotationDamping
        );
    }

    void Update()
    {
        if (positionTarget != null)
        {
            LerpPosition();
        }
        if (rotationTarget != null)
        {
            LerpOrientation();
        }
    }
}
