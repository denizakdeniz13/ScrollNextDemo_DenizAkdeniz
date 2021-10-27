using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameSituation
{
    Paused,
    Continues,
    Lose,
    Win,
}
public class GameManager : MonoBehaviour
{
    public GameSituation gameSituation;

    [SerializeField] private GameObject startUI;
    public GameObject restartPanel;
    public GameObject winPanel;

    [SerializeField] private List<GameObject> levels = new List<GameObject>();
    [Space]
    [SerializeField] private Text levelText;
    [SerializeField] private MainCharacterController player;

    private int levelCount;

    private float touchTimer = 0.15f;
    private float currentTouchTimer = 0.15f;



    private void Start()
    {

        StartLevel(levelCount);

    }

    private void Update()
    {
        currentTouchTimer -= Time.deltaTime; // Use timer for prevent buttons click count as touch multiple times

        if(currentTouchTimer <= 0)
        {
            currentTouchTimer = touchTimer;

            if (Input.touchCount > 0)
            {
                if (gameSituation == GameSituation.Paused || gameSituation == GameSituation.Win)
                    {
                        gameSituation = GameSituation.Continues;
                        startUI.gameObject.SetActive(false);

                    }
            }
        }

    }


    private void OnEnable()
    {
        gameSituation = GameSituation.Paused;

        foreach(GameObject level in levels)
        {
            level.SetActive(false);
        }

        if (PlayerPrefs.HasKey("level"))
        {
            levelCount = PlayerPrefs.GetInt("level");
        }
        else
        {
            levelCount = 1;
            PlayerPrefs.SetInt("level", levelCount);
        }
    }

    private void StartLevel(int level)
    {
        levelText.text = "LEVEL " + levelCount;

        gameSituation = GameSituation.Paused;

        levels[levelCount - 1].SetActive(true);
        startUI.gameObject.SetActive(true);

    }


    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void NextLevel()
    {
        winPanel.SetActive(false);

        player.ResetCharacter();

        levels[levelCount - 1].SetActive(false);

        levelCount += 1;

        if (levelCount > levels.Count)
        {
            levelCount = 1;
            PlayerPrefs.SetInt("level", levelCount);
            SceneManager.LoadScene(0);
        }

        else
        {
            PlayerPrefs.SetInt("level", levelCount);
            StartLevel(levelCount);
        }




    }
}
