using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("In Game Components")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text multiplierText;
    [SerializeField] private GameObject healthIconLocation;
    [SerializeField] private Image healthIcon;

    [Header("UI Components")]
    [SerializeField] private GameObject elements;
    [SerializeField] private GameObject preText;
    [SerializeField] private GameObject highScores;
    [SerializeField] private GameObject scorePlacementText;
    [SerializeField] private GameObject postText;
    [SerializeField] private GameObject coinTextParent;
    [SerializeField] private GameObject coinTextChild;

    private bool gameStarted;

    public static PlayerUI instance;

    private void Awake()
    {
        instance = this;

        ActivatePreText();
        elements.SetActive(false);
    }

    private void Start()
    {
        SpawnScoreText();
        UpdateHighScores();
    }

    public void ActivatePreText()
    {
        gameStarted = false;

        StartCoroutine(HighScoreShuffle());

        coinTextParent.SetActive(true);
        StartCoroutine(TextBlink(coinTextParent, coinTextChild));

        postText.SetActive(false);
    }

    public void ActivateElements()
    {
        gameStarted = true;

        coinTextParent.SetActive(false);
        preText.SetActive(false);
        highScores.SetActive(false);

        postText.SetActive(false);

        elements.SetActive(true);
    }

    public void ActivatePostText()
    {
        postText.SetActive(true);

        StartCoroutine(SwitchText(postText, ActivatePreText));
    }

    public void UpdateScore(float score)
    {
        scoreText.text = score.ToString();
    }

    public void UpdateMultiplier(float multiplier)
    {
        multiplierText.text = "x" + multiplier.ToString();
    }

    public void UpdateHealth(int health)
    {
        for (int i = 0; i < healthIconLocation.transform.childCount; i++) {
            Destroy(healthIconLocation.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < health; i++)
        {
            Image g = Instantiate(healthIcon, healthIconLocation.transform);
            g.gameObject.transform.localPosition = new Vector2(healthIcon.transform.position.x - (i*30), healthIcon.transform.position.y);
        }
    }

    private IEnumerator TextBlink(GameObject parentText, GameObject childText)
    {
        while (parentText.activeInHierarchy)
        {
            if (childText.activeInHierarchy)
            {
                childText.SetActive(false);
            }
            else childText.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
        yield break;
    }

    private IEnumerator SwitchText(GameObject oldText, Action newText)
    {
        yield return new WaitForSeconds(3f);
        oldText.SetActive(false);
        newText?.Invoke();
        yield break;
    }

    private IEnumerator HighScoreShuffle()
    {
        while (!gameStarted)
        {
            preText.SetActive(true);
            highScores.SetActive(false);
            yield return new WaitForSeconds(10f);
            if (gameStarted) yield break;
            preText.SetActive(false);
            highScores.SetActive(true);
            yield return new WaitForSeconds(6f);
        }
        yield break;
    }

    public void UpdateHighScores()
    {
        for (int i = 0; i < highScores.transform.childCount; i++)
        {
            highScores.transform.GetChild(i).GetComponent<Text>().text = StoredData.instance.loopNumber[i] + "-" + StoredData.instance.levelNumber[i] + "   " + StoredData.instance.highScores[i].ToString();
        }
    }

    private void SpawnScoreText()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject g = Instantiate(scorePlacementText, highScores.transform);
            g.gameObject.transform.localPosition = new Vector2(highScores.transform.position.x, highScores.transform.position.y + 75 - (i * 40));
            g.GetComponent<Text>().text = StoredData.instance.highScores[i].ToString();
        }
    }
}
