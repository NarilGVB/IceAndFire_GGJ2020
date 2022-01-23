using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TMP_Text endingPointsText;
    [SerializeField] TMP_Text maxPointsText;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowGameOverPanel(int points)
    {
        gameOverPanel.SetActive(true);
        endingPointsText.text = points.ToString();
        
        if (points > LoaderManager.Data.maxScore)
        {
            //New high score
            LoaderManager.Instance.SaveMaxScore(points);
        }
        
        maxPointsText.text = LoaderManager.Data.maxScore.ToString();
    }

    public void Restart()
    {
        LoaderManager.Instance.LoadGameScene();
    }

    public void ExitGame()
    {
        LoaderManager.Instance.LoadMenu();
    }

    

}
