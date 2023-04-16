using UnityEngine;

public class AttackButton : MonoBehaviour
{
    private PlayerController _player;

    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();
    }

    public void Attack()
    {
        _player.OnAttack();
    }
}
