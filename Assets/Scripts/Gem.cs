using UnityEngine;
using System;

public class Gem : MonoBehaviour
{
    public event Action<Gem> GemCollected;

    public void Collected()
    {
        GemCollected?.Invoke(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy.IndicatorIsActive == false)
            {
                enemy.EnableIndicator();
                Collected();
            }
        }
        else if (other.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player.IndicatorIsActive == false)
            {
                player.EnableIndicator();
                Collected();
            }
        }
    }
}
