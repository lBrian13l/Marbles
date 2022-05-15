using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpButton : MonoBehaviour, IPlayerSpawnedHandler
{
    private PlayerController _player;

    public void Jump()
    {
        _player.OnJump();
    }

    public void HandlePlayerSpawned(GameObject player)
    {
        _player = player.GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }
}
