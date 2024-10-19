using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(FirstPersonController))]
[RequireComponent(typeof(ThirdPersonController))]
public class GameManagerController : MonoBehaviour
{
    FirstPersonController firstPersonController;
    ThirdPersonController thirdPersonController;
    private InputActions inputActions;
    private bool IsIn2D => thirdPersonController.enabled;


    private void Awake()
    {
        firstPersonController = GetComponent<FirstPersonController>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        inputActions = new InputActions();
        inputActions.PersonChange.Enable();
        firstPersonController.enabled = true;
        thirdPersonController.enabled = false;
    }
    
    void Update()
    {
        if (inputActions.PersonChange.ChangeCamera.triggered)
        {
            firstPersonController.enabled = !firstPersonController.enabled;
            thirdPersonController.enabled = !thirdPersonController.enabled;
        }
    }
}
