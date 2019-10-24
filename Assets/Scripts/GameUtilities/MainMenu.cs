using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    ReputationAvatar reputationMeter;
    public GameObject loseCanvas;
    public GameObject winCanvas;


    public void Awake()
    {
        reputationMeter = GameObject.FindWithTag("ReputationMeter").GetComponent<ReputationAvatar>();
        reputationMeter.OnReputation0.AddListener(OnReputationMeterReputation0);
        reputationMeter.OnReputation1.AddListener(OnReputationMeterReputation1);
    }

    void OnReputationMeterReputation0()
    {
        loseCanvas.SetActive(true);
    }

    void OnReputationMeterReputation1()
    {
        winCanvas.SetActive(true);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void NewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void QuitGame()
    {
        Application.Quit();
        print("Game Quit");
    }

    public void Resurrect()
    {
        
    }
}
