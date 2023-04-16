using UnityEngine;
using System;

public class DeathTrap : MonoBehaviour
{
    public Action GameOver;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameOver?.Invoke();
            Debug.Log("Game Over (spikes)");
        }
        else
        {
            collision.gameObject.GetComponent<Enemy>().Health = 0;
            //Destroy(collision.gameObject);
            Debug.Log("Destroyed (spikes)");
        }
    }
}
