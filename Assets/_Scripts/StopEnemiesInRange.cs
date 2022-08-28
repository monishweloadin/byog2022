using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopEnemiesInRange : MonoBehaviour
{

    public List<EnemyController> enemies;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemies.Add(other.GetComponent<EnemyController>());

            other.GetComponent<EnemyController>().ChangeToIdle();
            other.GetComponent<EnemyController>().CanMove = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemies.Remove(other.GetComponent<EnemyController>());

            other.GetComponent<EnemyController>().ResetEnemyMoving();
            other.GetComponent<EnemyController>().ResetState();
            other.GetComponent<EnemyController>().CanMove = true;
        }
    }

    private void OnDisable()
    {
        foreach(EnemyController enemy in enemies)
        {
            enemy.ResetEnemyMoving();
            enemy.ResetState();
            enemy.CanMove = true;
        }
        enemies.Clear();
    }

}
