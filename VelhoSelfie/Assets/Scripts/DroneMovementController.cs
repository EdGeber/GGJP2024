using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class DroneMovementController : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;
    private GameManagerController _gameManager;
    private StarterAssetsInputs _input;
    private Rigidbody _rigidbody;

    [SerializeField]
    private float DroneSpeed = 5f;

    void Start()
    {
        _gameManager = GameManagerController.instance;
        _input = StarterAssetsInputs.instance;
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!_gameManager.IsIn2D)
            return;

        _rigidbody.velocity = new Vector3(
            _input.move.x * DroneSpeed,
            0,
            _input.move.y * DroneSpeed
        );
    }
}
