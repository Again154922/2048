using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI CurrentBestValue;
    [SerializeField] private TextMeshProUGUI HistoryBestValue;
    [SerializeField] private GameObject NewBestInstruction;
    [SerializeField] private GameObject GameOverPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        NewBestInstruction.SetActive(false);
        GameOverPanel.SetActive(false);
        HistoryBestValue.text = BlockManager.Instance.HistoryBest.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentBestValue.text = BlockManager.Instance.CurrentBest.ToString();
    }

    private void OnApplicationQuit()
    {
        if (BlockManager.Instance.CurrentBest > BlockManager.Instance.HistoryBest)
        {
            PlayerPrefs.SetInt(Session.NewBestKey, BlockManager.Instance.CurrentBest);
            PlayerPrefs.Save();
        }
    }
}
