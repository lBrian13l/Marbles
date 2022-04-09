using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrap : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EventBus.RaiseEvent<IGameOverHandler>(h => h.HandleGameOver());
            Debug.Log("Game Over (spikes)");
        }
        else
        {
            //EventBus.RaiseEvent<IEnemyDiedHandler>(h => h.HandleEnemyDied(collision.gameObject));
            collision.gameObject.GetComponent<Enemy>().Health = 0;
            //Destroy(collision.gameObject);
            Debug.Log("Destroyed (spikes)");
        }
    }
}
