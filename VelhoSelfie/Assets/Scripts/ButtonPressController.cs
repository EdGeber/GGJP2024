using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonPressController : MonoBehaviour
{
    private List<GameObject> gameObjectsPressing = new();
    public UnityEvent isPressed;
    public UnityEvent isReleased;
    private Vector3 initialPosition;
    private Vector3 pressedPosition;
    public float pressVelocity = 0.1f;

    private void Awake()
    {
        initialPosition = transform.position;
        pressedPosition = initialPosition - new Vector3(0, 0.5f, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObjectsPressing.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        gameObjectsPressing.Remove(other.gameObject);
    }

    private void FixedUpdate()
    {
        if (gameObjectsPressing.Count > 0)
        {
            isPressed?.Invoke();

            if (transform.position != pressedPosition)
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    pressedPosition,
                    pressVelocity
                );
            }
        }
        else
        {
            isReleased?.Invoke();

            if (transform.position != initialPosition)
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    initialPosition,
                    pressVelocity
                );
            }
        }
    }
}
