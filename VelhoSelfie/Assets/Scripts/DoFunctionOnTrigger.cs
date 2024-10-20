using UnityEngine;
using UnityEngine.Events;

public class DoFunctionOnTrigger : MonoBehaviour
{
    public LayerMask excludeLayers;
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (excludeLayers == (excludeLayers | (1 << other.gameObject.layer)))
            return;

        onTriggerEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (excludeLayers == (excludeLayers | (1 << other.gameObject.layer)))
            return;

        onTriggerExit?.Invoke();
    }
}
