using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().CanIncraseHealth = true;
        }

        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().CanReduceLifeOverTime = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().CanIncraseHealth = false;
        }

        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().CanReduceLifeOverTime = false;
        }
    }
}
