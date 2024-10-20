using System.Collections;
using System.Collections.Generic;
using MaskTransitions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour
{
    private void Start()
    {
        GameEventContainer.Instance.ResetAllToDefault();
    }

    public void GoToGame()
    {
        TransitionManager.Instance.LoadLevel("Level0");
    }
}
