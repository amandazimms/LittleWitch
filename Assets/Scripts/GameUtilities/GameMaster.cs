using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public float currentReputation;
    public float score;
    public int numCuredThisDay;

    public GameObject reputationMeterGO;
    public GameObject morningPopup;
    public GameObject loseCanvas;
    public GameObject winCanvas;

    public DayInfo dayInfo;

    GameObject gameManager;
    WaveSpawner waveSpawner;

    public Text scoreText;
    public Text plantsText; //todo
    public Text potionsText; //todo

    public UnityEvent OnReputationEmpty;
    public UnityEvent OnReputationFull;

    public void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager");
        waveSpawner = gameManager.GetComponent<WaveSpawner>();

        dayInfo = GetComponent<DayInfo>();

        dayInfo.OnDay.AddListener(BeginningOfDay);
        dayInfo.OnFinalMorning.AddListener(OnWinGame);
    }

    public void BeginningOfDay()
    {
        UpdateScore();
    }

    public void UpdateScore()
    {
        score += currentReputation * 10 * numCuredThisDay;
        scoreText.text = score.ToString();

        numCuredThisDay = 0;
    }


    public void ChangeReputation(float amount)
    {
        currentReputation += amount;

        if (currentReputation > 1)
        {
            currentReputation = 1;
            if (OnReputationFull != null)
                OnReputationFull.Invoke();
        }
        if (currentReputation < 0)
        {
            currentReputation = 0;
            OnLoseGame();

            if (OnReputationEmpty != null)
                OnReputationEmpty.Invoke();
        }
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

    void OnLoseGame()
    {
        loseCanvas.SetActive(true);
        reputationMeterGO.SetActive(false);
        waveSpawner.enabled = false;
    }

    public void DoOver()    
    {
        print("do over");
        currentReputation = .2f;
        reputationMeterGO.SetActive(true);
        waveSpawner.enabled = true;
    }

    void OnWinGame()
    {
        UpdateScore();

        winCanvas.SetActive(true);
        reputationMeterGO.SetActive(false);
        waveSpawner.enabled = false;
    }

    public void KeepPlaying()
    {
        reputationMeterGO.SetActive(true);
    }
}
