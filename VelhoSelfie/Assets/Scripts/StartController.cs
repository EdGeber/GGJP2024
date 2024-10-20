using System.Collections;
using System.Collections.Generic;
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
        SceneManager.LoadScene("Level0");
    }
}
