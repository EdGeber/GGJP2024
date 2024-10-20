using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronePickupBox : MonoBehaviour
{
    public List<GameObject> Boxes = new();
    private int _levitatingState = 0;
    public Transform droneBody;
    public float levitateVelocity = 0.04f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            _levitatingState = _levitatingState == 1 ? 0 : 1;
        }
    }

    private void FixedUpdate()
    {
        if (_levitatingState == 1)
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
        else if (_levitatingState == 0)
        {
            List<GameObject> copy = new();

            foreach (var box in Boxes)
            {
                copy.Add(box);
            }

            foreach (var box in copy)
            {
                box.GetComponent<Rigidbody>().useGravity = true;
                Outline Outline = box.GetComponent<Outline>();
                Outline.enabled = false;
                Boxes.Remove(box);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Boxes.Contains(other.gameObject) && _levitatingState != 1)
        {
            Boxes.Remove(other.gameObject);
            Outline Outline = other.GetComponent<Outline>();
            Outline.enabled = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PickItem") && !Boxes.Contains(other.gameObject) && Boxes.Count < 2)
        {
            Boxes.Add(other.gameObject);
            Outline Outline = other.GetComponent<Outline>();
            Outline.enabled = true;
            if (_levitatingState == 0)
                _levitatingState = 2;
        }
    }
}
