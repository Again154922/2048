using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSelectManager : MonoBehaviour
{
    public void LoadSaveButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void NewGameButton()
    {
        BlockManager.DelMap();
        Session.MapSave = new int[4, 4];
        SceneManager.LoadScene("Game");
    }
}
