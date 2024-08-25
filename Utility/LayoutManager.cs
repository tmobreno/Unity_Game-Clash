using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private int pointThreshold;
    [SerializeField] private int thresholdIncrease;
    [SerializeField] private Layout[] layouts;
    [SerializeField] private PlayerStats baseStats;
    private PlayerStats curStats;
    [SerializeField] private PlayerController player;
    private PlayerController curPlayer;
    [SerializeField] private GameObject utility;

    private int nextLevelNumber = 0;
    private int numberOfLoops = -1;
    public int enemiesOnLayout { get; private set; }

    private GameObject currentLayout;
    private int currentLayoutPoints;

    public static Action ActivateExit;
    public static Action ActivateUpgrades;

    public static LayoutManager instance;

    private void Awake()
    {
        instance = this;
        SetCursor();
        ResetGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        if (Input.GetKeyDown(KeyCode.Space) && curPlayer == null)
        {
            SpawnPlayer();
            ResetGame();
        }
    }

    public void ResetGame()
    {
        nextLevelNumber = 0;
        numberOfLoops = -1;
        ResetPlayerStats();
        SwapLayout();
    }

    private void SpawnPlayer()
    {
        PlayerUI.instance.ActivateElements();
        curPlayer = Instantiate(player);
    }

    private void ResetPlayerStats()
    {
        if (curStats != null) Destroy(curStats.gameObject);
        curStats = Instantiate(baseStats, utility.gameObject.transform);
    }

    private void SetCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpdateScore(int change)
    {
        currentLayoutPoints += change;
        if (currentLayoutPoints >= pointThreshold / 2) ActivateUpgrades?.Invoke();
        if (currentLayoutPoints >= pointThreshold) ActivateExit?.Invoke();
    }

    public void SwapLayout()
    {
        if(currentLayout != null) Destroy(currentLayout.gameObject);
        ResetCurrentPoints();
        ResetEnemyAmount();
        SpawnLayout();
    }

    private void ResetCurrentPoints()
    {
        currentLayoutPoints = 0;
    }

    private void SpawnLayout()
    {
        if (nextLevelNumber == 0) numberOfLoops++;

        pointThreshold = layouts[nextLevelNumber].threshold + (thresholdIncrease * numberOfLoops);

        currentLayout = Instantiate(layouts[nextLevelNumber].layoutObj, this.transform);

        nextLevelNumber++;
        if (nextLevelNumber == 9) nextLevelNumber = 0;
    }

    public void ChangeEnemyAmount(int change)
    {
        enemiesOnLayout += change;
    }

    public void ResetEnemyAmount()
    {
        enemiesOnLayout = 0;
    }

    public int GetCurrentLevel()
    {
        return nextLevelNumber - 1;
    }

    public int GetLoopNumber()
    {
        return numberOfLoops;
    }
}

[System.Serializable]
public class Layout
{
    public GameObject layoutObj;
    public int threshold;
}
