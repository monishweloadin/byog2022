using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    public GameObject PickupUI;
    public GameObject GameOverUI;
    public GameObject GameCompleteUI;

    public GameObject YouThoughItsThatEasyUI;

    private PlayerController _player;

    [Header("Player UI")]
    public Slider HealthSlider;
    public Slider StaminaSider;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    private void Start()
    {
        _player = LevelManager.Instance.Player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        UpdatePlayerUI();
    }

    public void UpdatePlayerUI()
    {
        HealthSlider.value = _player.Health;
        StaminaSider.value = _player.Stamina;
    }


    public void EnablePickupUI(bool active)
    {
        PickupUI.SetActive(active);
    }

    public void EnableGameOverScreen(bool value)
    {
        GameOverUI.SetActive(value);
    }
    
    public void EnableGameCompletedUI()
    {
        Time.timeScale = 0;
        GameCompleteUI.SetActive(true);
    }

}
