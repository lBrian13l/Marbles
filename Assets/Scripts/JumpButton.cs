using UnityEngine;

public class JumpButton : MonoBehaviour
{
    private PlayerController _player;

    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();
    }

    public void Jump()
    {
        _player.OnJump();
    }
}
