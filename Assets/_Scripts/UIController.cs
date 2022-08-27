using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    public GameObject PickupUI;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }


    public void EnablePickupUI(bool active)
    {
        PickupUI.SetActive(active);
    }
    
}
