using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool IsFirstDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (IsFirstDoor)
            {
                if (other.GetComponent<PlayerController>().CurrentObjectOnHand != null)
                {
                    if (other.GetComponent<PlayerController>().CurrentObjectOnHand.GetComponent<PickableObject>().PickableType == PickableType.KEY)
                    {
                        foreach (Transform i in transform)
                        {
                            i.GetComponent<Animator>().SetTrigger("Open");
                        }
                    }
                }
            }
            else
            {
                if (other.GetComponent<PlayerController>().CurrentObjectOnHand.GetComponent<PickableObject>().PickableType == PickableType.KEY && LevelManager.Instance.GameCompleted)
                {
                    foreach (Transform i in transform)
                    {
                        i.GetComponent<Animator>().SetTrigger("Open");
                    }

                    StartCoroutine(WaitForOpenDoor());
                }
                else if (other.GetComponent<PlayerController>().CurrentObjectOnHand.GetComponent<PickableObject>().PickableType == PickableType.KEY)
                {
                    
                    UIController.Instance.YouThoughItsThatEasyUI.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (IsFirstDoor)
            {
                if (other.GetComponent<PlayerController>().CurrentObjectOnHand != null)
                {
                    if (other.GetComponent<PlayerController>().CurrentObjectOnHand.GetComponent<PickableObject>().PickableType == PickableType.KEY)
                    {
                        other.GetComponent<PlayerController>().CurrentObjectOnHand.transform.parent = null;
                        other.GetComponent<PlayerController>().CurrentObjectOnHand.GetComponent<CapsuleCollider>().enabled = true;
                        other.GetComponent<PlayerController>().CurrentObjectOnHand.GetComponent<Rigidbody>().isKinematic = false;
                        other.GetComponent<PlayerController>().CurrentObjectOnHand.GetComponent<PickableObject>().CanPickup = true;
                    }
                }
            }
            else
            {
                if (other.GetComponent<PlayerController>().CurrentObjectOnHand.GetComponent<PickableObject>().PickableType == PickableType.KEY)
                {
                    UIController.Instance.YouThoughItsThatEasyUI.SetActive(false);
                }
            }
        }
    }

    public IEnumerator WaitForOpenDoor()
    {
        yield return new WaitForSeconds(3f);
        UIController.Instance.EnableGameCompletedUI();
    }
}
