using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronePickupBox : MonoBehaviour
{
    public List<GameObject> Boxes = new();
    private bool _isLevitating;
    public Transform droneBody;
    public float levitateVelocity = 0.04f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _isLevitating = !_isLevitating;
        }
    }

    private void FixedUpdate()
    {
        if (_isLevitating)
        {
            foreach (var box in Boxes)
            {
                box.transform.position = Vector3.Lerp(
                    box.transform.position,
                    new Vector3(
                        droneBody.position.x,
                        droneBody.position.y - 2,
                        droneBody.position.z
                    ),
                    levitateVelocity
                );
                box.GetComponent<Rigidbody>().useGravity = false;
            }
        }
        else
        {
            foreach (var box in Boxes)
            {
                box.GetComponent<Rigidbody>().useGravity = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickItem"))
        {
            Boxes.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Boxes.Contains(other.gameObject) && !_isLevitating)
        {
            Boxes.Remove(other.gameObject);
        }
    }
}
