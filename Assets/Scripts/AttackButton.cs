using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour, IPlayerSpawnedHandler
{
    private PlayerController _player;

    public void Attack()
    {
        _player.OnAttack();
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
