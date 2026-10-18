using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool IsGameOver;
    public bool IsQuitting;
    public bool DontSave;
    [SerializeField] private TextMeshProUGUI CurrentBestValue;
    [SerializeField] private TextMeshProUGUI HistoryBestValue;
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private GameObject SavePanel;

    void Awake()
    {
        Instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        GameOverPanel.SetActive(false);
        SavePanel.SetActive(false);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CurrentBestValue.text = BlockManager.Instance.CurrentBest.ToString();
        HistoryBestValue.text = BlockManager.Instance.HistoryBest.ToString();
        if (IsGameOver)
        {
            GameOverPanel.SetActive(true);
            Time.timeScale = 0;
        }
        else if (IsQuitting)
        {
            SavePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void RestartButtonOnclick()
    {
        Refresh();
    }

    public void QuitButtonOnclick()
    {
        Application.Quit(0);
    }

    public void SaveButtonOnclick()
    {
        BlockManager.Instance.SaveMap();
        Application.Quit(0);
    }

    public void DontSaveButtonOnclick()
    {
        DontSave = true;
        BlockManager.DelMap();
        Application.Quit(0);
    }

    public void Refresh()
    {
        BlockManager.Instance.Refresh();
        SavePanel.SetActive(false);
        GameOverPanel.SetActive(false);
        IsGameOver = false;
        Time.timeScale = 1;
    }
}
