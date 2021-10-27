using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyGroups = new List<GameObject>();

    [SerializeField] private float timer;

    private float currentTime;
    private int counter;

    private GameManager gm;
    private GameSituation gameSituation;

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        currentTime = timer;
        ActiveGroup();
    }
    private void Update()
    {
        gameSituation = gm.gameSituation;

        if (gameSituation == GameSituation.Continues)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = timer;
                ActiveGroup();
            }
        }

    }


    private void ActiveGroup()
    {
        if(enemyGroups.Count > counter)
        {
            enemyGroups[counter].SetActive(true);
            counter += 1;
        }

            
    }
}
