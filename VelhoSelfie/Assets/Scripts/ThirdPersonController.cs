using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [SerializeField] GameObject camera2D;
    private InputActions inputActions;

    private void Awake()
    {
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        inputActions.ThirdPerson.Enable();
        camera2D.SetActive(true);
        GameEvents.CurrentView?.Set(View.ThirdPerson);
    }

    private void OnDisable()
    {
        inputActions.ThirdPerson.Disable();
        camera2D.SetActive(false);
    }
}
