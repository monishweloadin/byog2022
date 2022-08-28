using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DummyEnemy : MonoBehaviour
{

    public GameObject RightHand;

    private GameObject _player;


    public GameObject CurrentlyHoldingItem;


    private void Start()
    {

        _player = LevelManager.Instance.Player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHit"))
        {
            if (other.transform.root.GetComponent<PlayerController>().CurrentObjectOnHand != null)
            {

                other.transform.root.GetComponent<PlayerController>().ReduceHealth(10);

                GameObject obj = other.transform.root.GetComponent<PlayerController>().CurrentObjectOnHand;
                other.transform.root.GetComponent<PlayerController>().CurrentObjectOnHand = null;

                if (CurrentlyHoldingItem != null)
                {
                    CurrentlyHoldingItem.transform.parent = null;

                    CurrentlyHoldingItem.GetComponent<CapsuleCollider>().enabled = true;
                    CurrentlyHoldingItem.GetComponent<BoxCollider>().enabled = true;
                    CurrentlyHoldingItem.GetComponent<Rigidbody>().isKinematic = false;
                    CurrentlyHoldingItem.GetComponent<PickableObject>().CanPickup = true;
                }

                PickupObject(obj);

            }
        }

        if (other.CompareTag("PickableObject"))
        {
            if (CurrentlyHoldingItem == null)
            {
                GameObject obj = other.gameObject;
                PickupObject(obj);
            }
        }
    }

    private void PickupObject(GameObject obj)
    {
        if (LevelManager.Instance.AvalaiblePickupObjects.Contains(obj))
            LevelManager.Instance.AvalaiblePickupObjects.Remove(obj);

        obj.GetComponent<CapsuleCollider>().enabled = false;
        obj.GetComponent<BoxCollider>().enabled = false;
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.GetComponent<PickableObject>().CanPickup = false;

        obj.transform.SetParent(this.RightHand.transform);
        obj.transform.localPosition = obj.GetComponent<PickableObject>().PostionOffset;
        obj.transform.localRotation = Quaternion.Euler(obj.GetComponent<PickableObject>().ObjectRotation);

        CurrentlyHoldingItem = obj;

        UIController.Instance.EnablePickupUI(false);
    }

}
