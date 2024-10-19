using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class GameManagerController : MonoBehaviour
{
    public GameManagerController instance;
    public GameObject camera2D,
        player;
    private bool _isIn2D = false;
    private StarterAssetsInputs _playerInputs;

    void Start()
    {
        instance = this;
        _playerInputs = player.GetComponent<StarterAssetsInputs>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (_isIn2D)
            {
                camera2D.SetActive(false);
                _isIn2D = false;
                _playerInputs.cursorInputForLook = true;
            }
            else
            {
                camera2D.SetActive(true);
                _isIn2D = true;
                _playerInputs.cursorInputForLook = false;
            }
        }
    }
}
