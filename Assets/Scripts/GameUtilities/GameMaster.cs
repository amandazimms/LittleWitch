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

    [Header("On Main Overlay")]
    public Text scoreText;
    public Text plantsText; //todo
    public Text potionsText; //todo

    [Header("On Morning Popup")]
    public Text townsfolkText;
    public Text reputationText, scoreDailyText;
    string townsfolkString, reputationString, scoreDailyTextString; //on morning popu: these read something like "townsfolk cured", and are grabbed from inspector at game start.

    public UnityEvent OnReputationEmpty;
    public UnityEvent OnReputationFull;

    public void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager");
        waveSpawner = gameManager.GetComponent<WaveSpawner>();

        dayInfo = GetComponent<DayInfo>();

        dayInfo.OnDay.AddListener(BeginningOfDay);
        dayInfo.OnFinalMorning.AddListener(OnWinGame);

        townsfolkString = townsfolkText.text;
        reputationString = reputationText.text;
        scoreDailyTextString = scoreDailyText.text;
    }

    public void BeginningOfDay()
    {

        UpdateScore();
        StartCoroutine(DisplayMorningPopup());
    }

    public void UpdateScore()
    {
        //CALCULATE TODAY'S SCORE ADDER
        int scoreAddingToday = (int)(currentReputation * 10 * numCuredThisDay);

        //UPDATE MORNING POPUP
        townsfolkText.text = townsfolkString + " " + numCuredThisDay.ToString();
        numCuredThisDay = 0; //now that we've used this number, can reset it to 0 for next day

        reputationText.text = reputationString + " " + currentReputation.ToString();

        scoreDailyText.text = scoreDailyTextString + " " + scoreAddingToday.ToString();


        //UPDATE MAIN OVERLAY
        //todo - when hooking up animations, we only want the text display of the score (on main overlay) to update after the fun juicy anim runs
        score += scoreAddingToday;
        scoreText.text = score.ToString();
    }

    public IEnumerator DisplayMorningPopup()
    {
        morningPopup.SetActive(true);
        yield return new WaitForSeconds(3);
        morningPopup.SetActive(false);
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
