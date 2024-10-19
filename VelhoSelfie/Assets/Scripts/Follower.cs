using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] Transform target;

    [Tooltip("How smoothly the transform follows the target's position, separately for horizontal and vertical movement.")]
    [SerializeField][Range(0f, 1f)] float horizontalDamping = 0.8f;
    [SerializeField][Range(0f, 1f)] float verticalDamping = 0.8f;
    [SerializeField]
    [Tooltip("How smoothly the transform follows the target's orientation")]
    [Range(0f, 1f)]
    float rotationDamping = 0f;

    Vector3 newPosition = new Vector3();

    // [Button]
    void UpdateTransform()
    {
        if (target == null)
        {
            Debug.LogWarning("A Follows component has been added to the GameObject but no target has been assigned.");
            return;
        }
        transform.position = target.position;
        transform.rotation = target.rotation;
    }

    private void Reset()
    {
        UpdateTransform();
    }

    private void OnValidate()
    {
        UpdateTransform();
    }

    void Start()
    {
        if (target == null) Debug.LogWarning("A Follows component has been added to the GameObject but no target has been assigned.");
        UpdateTransform();
    }

    void LerpPosition()
    {
        float t = 1f - horizontalDamping;
        newPosition.x = Mathf.Lerp(transform.position.x, target.position.x, 1f - horizontalDamping);
        newPosition.y = Mathf.Lerp(transform.position.y, target.position.y, 1f - verticalDamping);
        newPosition.z = Mathf.Lerp(transform.position.z, target.position.z, 1f - horizontalDamping);
        transform.localPosition = newPosition;
    }

    void LerpOrientation()
    {
        transform.localRotation = Quaternion.Slerp(transform.rotation, target.rotation, 1f - rotationDamping);
    }

    void Update()
    {
        LerpPosition();
        LerpOrientation();
    }
}
