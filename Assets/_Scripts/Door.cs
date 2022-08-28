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
            UIController.Instance.EnableInteractUI(true);
            if (IsFirstDoor)
            {
                if(other.GetComponent<PlayerController>().CurrentObjectOnHand.GetComponent<PickableObject>().PickableType == PickableType.KEY)
                {
                    foreach(Transform i in transform)
                    {
                        i.GetComponent<Animator>().SetTrigger("Open");
                    }
                }
            }
            else
            {
                if (other.GetComponent<PlayerController>().CurrentObjectOnHand.GetComponent<PickableObject>().PickableType == PickableType.KEY)
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
            UIController.Instance.EnableInteractUI(false);
            if (IsFirstDoor)
            {

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
}
