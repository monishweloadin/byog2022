using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class PickableObject : MonoBehaviour
{

    public Vector3 ObjectRotation;

    private void Start()
    {
        GetComponent<SphereCollider>().isTrigger = true;
        GetComponent<SphereCollider>().radius = 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.AvalaiblePickupObjects.Add(gameObject);
            UIController.Instance.EnablePickupUI(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.AvalaiblePickupObjects.Remove(gameObject);
            UIController.Instance.EnablePickupUI(false);
        }
    }

}
