using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrap : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
