using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider),typeof(Rigidbody))]
public class PickableObject : MonoBehaviour
{
    public PickableType PickableType;

    public bool CanPickup;
    public Vector3 PostionOffset;
    public Vector3 ObjectRotation;

    private void Start()
    {
        GetComponent<CapsuleCollider>().isTrigger = true;
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

public enum PickableType
{
    STOP,
    BAT,
    KEY,
    WRENCH
}
