using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public GameObject Player;
    public List<GameObject> AvalaiblePickupObjects;

    public List<GameObject> AvailableEnemeis;

    public bool GameCompleted;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        Time.timeScale = 1;
    }

    private void Update()
    {
        if (!GameCompleted)
        {
            foreach (GameObject obj in AvailableEnemeis)
            {
                if (obj.activeSelf)
                    return;
            }
            GameCompleted = true;
            //open door
        }
    }

    public void PlayAgainPressed()
    {
        SceneManager.LoadScene("Level");
    }

    public void ExitPressed()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
