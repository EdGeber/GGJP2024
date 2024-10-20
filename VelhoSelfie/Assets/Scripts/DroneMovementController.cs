using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class DroneMovementController : MonoBehaviour
{
    [SerializeField]
    private Transform _player;
    private GameManagerController _gameManager;
    private StarterAssetsInputs _input;
    private Rigidbody _rigidbody;
    private float _distanceToPlayer;

    [SerializeField]
    private float DroneSpeed = 5f,
        followDistance = 10f;

    void Start()
    {
        _gameManager = GameManagerController.instance;
        _input = StarterAssetsInputs.instance;
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CalculateDistance();

        if (_gameManager.IsIn2D)
        {
            MoveIn2D();
        }



        Follow();
    }

    private void CalculateDistance()
    {
        Vector3 dronePositionXZ = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPositionXZ = new Vector3(_player.position.x, 0, _player.position.z);
        _distanceToPlayer = Vector3.Distance(dronePositionXZ, playerPositionXZ);
    }

    private void Follow()
    {
        Vector3 dronePositionXZ = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPositionXZ = new Vector3(_player.position.x, 0, _player.position.z);
        if (_distanceToPlayer > followDistance || !_gameManager.IsIn2D)
        {
            Vector3 direction = (playerPositionXZ - dronePositionXZ).normalized;

            _rigidbody.velocity = new Vector3(
                direction.x * DroneSpeed,
                _rigidbody.velocity.y,
                direction.z * DroneSpeed
            );
        }
        else if (!_gameManager.IsIn2D)
        {
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
        }
    }

    private void MoveIn2D()
    {
        if (_distanceToPlayer <= followDistance)
        {
            _rigidbody.velocity = new Vector3(
                _input.move.x * DroneSpeed,
                0,
                _input.move.y * DroneSpeed
            );
        }
        else
        {
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
        }
    }
}
