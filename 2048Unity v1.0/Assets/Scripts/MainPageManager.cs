using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainPageManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionText;
    
    public void StartButtonOnClick()
    {
        instructionText.text = "\nEntering offline mode...";
        SceneManager.LoadScene(TryLoadSave(out Session.MapSave) ? "Select Saves" : "Game");
    }

    bool TryLoadSave(out int[,] save)
    {
        string[] saveLines;

        try { saveLines = File.ReadAllLines(Session.HistoryFilePath); }
        catch (Exception) { saveLines = Array.Empty<string>(); }

        if (saveLines.Length == 4)
        {
            save = new int[4, 4];
            for (int i = 0; i < 4; i++)
            {
                string[] row = saveLines[i].Split(' ');
                for (int j = 0; j < 4; j++)
                {
                    save[i, j] = Mathf.RoundToInt(Mathf.Log(float.Parse(row[j]), 2));
                }
            }
            return true;
        }

        save = new int[4, 4];
        return false;
    }
}
