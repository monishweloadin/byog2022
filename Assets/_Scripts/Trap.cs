using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Trap : MonoBehaviour
{

    public TrapType trapType;

    private void Start()
    {
        if (trapType == TrapType.MEDKIT)
        {
            transform.DOMoveY(transform.position.y + 1, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            transform.DORotate(Vector3.up * 360, 3f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (trapType == TrapType.SPIKE)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerController>().CanIncraseHealth = true;
                other.GetComponent<PlayerController>()._animator.SetTrigger("DamageTaken");
            }

            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<EnemyController>().CanReduceLifeOverTime = true;
            }
        }
        else if(trapType == TrapType.MEDKIT)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerController>().ReduceHealth(200);
            }

            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<EnemyController>().AddHealth(200);
            }
            StartCoroutine(TurnOffObject());
        }
    }

    private float playerDamageTakenElapsed = 0;
    private void OnTriggerStay(Collider other)
    {
        if (trapType == TrapType.SPIKE)
        {
            if (other.CompareTag("Player"))
            {
                playerDamageTakenElapsed += Time.deltaTime;
                if (playerDamageTakenElapsed >= 0.7f)
                {
                    playerDamageTakenElapsed %= 0.7f;
                    other.GetComponent<PlayerController>()._animator.SetTrigger("DamageTaken");
                }
            }

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (trapType == TrapType.SPIKE)
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

    public IEnumerator TurnOffObject()
    {
        gameObject.GetComponent<SphereCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(3f);
        gameObject.GetComponent<SphereCollider>().enabled = true;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

}

public enum TrapType
{
    SPIKE,
    MEDKIT
}
