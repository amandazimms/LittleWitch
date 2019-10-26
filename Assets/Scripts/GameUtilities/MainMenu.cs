using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject reputationMeterGO;
    public GameObject loseCanvas;
    public GameObject winCanvas;

    GameObject gameManager;
    WaveSpawner waveSpawner;

    ReputationAvatar reputationMeter;

    public void Awake()
    {
        reputationMeter = reputationMeterGO.GetComponent<ReputationAvatar>();
        reputationMeter.OnReputation0.AddListener(OnReputationMeterReputation0);
        reputationMeter.OnReputation1.AddListener(OnReputationMeterReputation1);

        gameManager = GameObject.FindWithTag("GameManager");
        waveSpawner = gameManager.GetComponent<WaveSpawner>();
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


    void OnReputationMeterReputation0()
    {
        loseCanvas.SetActive(true);
        reputationMeterGO.SetActive(false);
        waveSpawner.enabled = false;
    }

    public void DoOver()    
    {
        print("do over");
        reputationMeter.reputation = .2f;
        reputationMeterGO.SetActive(true);
        waveSpawner.enabled = true;
    }

    void OnReputationMeterReputation1()
    {
        winCanvas.SetActive(true);
        reputationMeterGO.SetActive(false);
        waveSpawner.enabled = false;
    }

    public void KeepPlaying()
    {
        reputationMeterGO.SetActive(true);
    }
}
