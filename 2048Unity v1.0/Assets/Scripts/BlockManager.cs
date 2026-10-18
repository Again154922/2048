using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BlockManager : MonoBehaviour
{
    public static BlockManager Instance { get; private set; }
    public int CurrentBest;
    public int HistoryBest;
    private int[,] map = new int[4, 4];
    private List<GameObject> blockList = new();
    [Header("Prefabs")]
    [SerializeField] private GameObject BlockPrefab0;
    [SerializeField] private GameObject BlockPrefab2;
    [SerializeField] private GameObject BlockPrefab4;
    [SerializeField] private GameObject BlockPrefab8;
    [SerializeField] private GameObject BlockPrefab16;
    [SerializeField] private GameObject BlockPrefab32;
    [SerializeField] private GameObject BlockPrefab64;
    [SerializeField] private GameObject BlockPrefab128;
    [SerializeField] private GameObject BlockPrefab256;
    [SerializeField] private GameObject BlockPrefab512;
    [SerializeField] private GameObject BlockPrefab1024;
    [SerializeField] private GameObject BlockPrefab2048;
    [SerializeField] private GameObject BlockPrefab4096;
    [SerializeField] private GameObject BlockPrefab8192;
    [SerializeField] private GameObject BlockPrefab16384;
    [SerializeField] private GameObject BlockPrefab32768;

    [Header("Others")]
    [SerializeField] private GameObject NewBestInstruction;

    private GameObject[] prefabs;
    
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        HistoryBest = PlayerPrefs.GetInt(Session.HistoryBestKey, 0);
        
        prefabs = new [] {
            BlockPrefab0,
            BlockPrefab2,
            BlockPrefab4,
            BlockPrefab8,
            BlockPrefab16,
            BlockPrefab32,
            BlockPrefab64,
            BlockPrefab128,
            BlockPrefab256,
            BlockPrefab512,
            BlockPrefab1024,
            BlockPrefab2048,
            BlockPrefab4096,
            BlockPrefab8192,
            BlockPrefab16384,
            BlockPrefab32768
        };
        InitMap();
    }

    void Start()
    {
        NewBestInstruction.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        GetKey();
        CurrentBest = GetCurrentBest();
        if (CurrentBest > HistoryBest)
        {
            HistoryBest = CurrentBest;
            NewBestInstruction.SetActive(true);
        }

        if (!CanMove())
        {
            RecordHistoryBest();
            GameManager.Instance.IsGameOver = true;
        }
    }

    private void OnApplicationQuit()
    {
        RecordHistoryBest();
        if (GameManager.Instance.IsGameOver || GameManager.Instance.DontSave) DelMap();
        else SaveMap();
    }

    void InitMap()
    {
        if (!Equal(Session.MapSave, new int[4, 4]))
        {
            DeepCopy(Session.MapSave, out map);
            ShowMap();
            return;
        }

        map[Random.Range(0, 4), Random.Range(0, 4)] = Random.Range(1, 3);
        do
        {
            int x = Random.Range(0, 4);
            int y = Random.Range(0, 4);
            if (map[x, y] == 0)
            {
                map[x, y] = Random.Range(1, 3);
                break;
            }
        } while (true);
        
        ShowMap();
    }

    void ShowMap()
    {
        foreach (var o in blockList) Destroy(o);
        blockList.Clear();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject tile = Instantiate(prefabs[map[i, j]], transform);
                tile.transform.localPosition = Session.MapXYs[i, j];
                blockList.Add(tile);
            }
        }
    }

    void GetKey()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) MoveMap(Direction.Left);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) MoveMap(Direction.Down);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) MoveMap(Direction.Right);
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) MoveMap(Direction.Up);
        else if (Input.GetKeyDown(KeyCode.R)) GameManager.Instance.Refresh();
        else if (Input.GetKeyDown(KeyCode.Escape)) GameManager.Instance.IsQuitting = true;
    }

    void MoveMap(Direction direction)
    {
        DeepCopy(map, out int[,] OldMap);
        
        for (int line = 0; line < 4; line++)
        {
            WriteLine(direction, line, MergeLine(GetLine(direction, line)));
        }

        if (!Equal(OldMap, map))
        {
            AddRandomBlock();
        }
        
        ShowMap();
    }

    void AddRandomBlock()
    {
        List<(int x, int y)> emptyBlocks = new();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (map[i, j] == 0)
                {
                    emptyBlocks.Add((i, j));
                }
            }
        }

        if (emptyBlocks.Count == 0)
        {
            return;
        }

        (int x, int y) position = emptyBlocks[Random.Range(0, emptyBlocks.Count)];
        map[position.x, position.y] = Random.Range(1, 3);
    }

    int[] GetLine(Direction direction, int line)
    {
        int[] values = new int[4];
        for (int position = 0; position < 4; position++)
        {
            (int x, int y) = GetCell(direction, line, position);
            values[position] = map[x, y];
        }

        return values;
    }

    int[] MergeLine(int[] line)
    {
        int[] result = new int[4];
        int write = 0;
        bool justMerged = false;

        for (int read = 0; read < 4; read++)
        {
            if (line[read] == 0)
            {
                continue;
            }

            if (write > 0 && !justMerged && result[write - 1] == line[read])
            {
                result[write - 1]++;
                justMerged = true;
            }
            else
            {
                result[write] = line[read];
                write++;
                justMerged = false;
            }
        }

        return result;
    }

    void WriteLine(Direction direction, int line, int[] values)
    {
        for (int position = 0; position < 4; position++)
        {
            (int x, int y) = GetCell(direction, line, position);
            map[x, y] = values[position];
        }
    }

    (int x, int y) GetCell(Direction direction, int line, int position)
    {
        return direction switch
        {
            Direction.Left => (line, position),
            Direction.Up => (position, line),
            Direction.Right => (line, 3 - position),
            Direction.Down => (3 - position, line),
            _ => (line, position)
        };
    }

    int GetCurrentBest()
    {
        int best = 0;
        
        foreach (var i in map)
        {
            if (i > best) best = i;
        }

        return Mathf.RoundToInt(Mathf.Pow(2, best));
    }

    int GetHistoryBest()
    {
        return PlayerPrefs.GetInt(Session.HistoryBestKey, 0);
    }
    
    void RecordHistoryBest()
    {
        if (HistoryBest <= GetHistoryBest()) return;
        
        PlayerPrefs.SetInt(Session.HistoryBestKey, HistoryBest);
        PlayerPrefs.Save();
    }

    bool CanMove()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (map[i, j] == 0)
                {
                    return true;
                }

                if (j + 1 < 4 && map[i, j] == map[i, j + 1])
                {
                    return true;
                }

                if (i + 1 < 4 && map[i, j] == map[i + 1, j])
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void Refresh()
    {
        DelMap();
        CurrentBest = 0;
        NewBestInstruction.SetActive(false);
        Session.MapSave = new int[4, 4];
        map = new int[4, 4];
        InitMap();
    }

    public void SaveMap()
    {
        File.WriteAllLines(Session.HistoryFilePath, GetStringMap());
    }

    public static void DelMap()
    {
        File.Delete(Session.HistoryFilePath);
    }

    string[] GetStringMap()
    {
        string[] result = new string[4];
        
        for (int i = 0; i < 4; i++)
        {
            string row = "";
            
            for (int j = 0; j < 4; j++)
            {
                row += map[i, j] + " ";
            }

            row = row.Trim();
            result[i] = row;
        }

        return result;
    }
    
    void DeepCopy(int[,] sourse, out int[,] destination)
    {
        destination = new int[sourse.GetLength(0), sourse.GetLength(1)];
        for (int i = 0; i < sourse.GetLength(0); i++)
        {
            for (int j = 0; j < sourse.GetLength(1); j++)
            {
                destination[i, j] = sourse[i, j];
            }
        }
    }

    bool Equal(int[,] array1, int[,] array2)
    {
        if (array1.GetLength(0) != array2.GetLength(0) || array1.GetLength(1) != array2.GetLength(1))
        {
            return false;
        }

        for (int i = 0; i < array1.GetLength(0); i++)
        {
            for (int j = 0; j < array1.GetLength(1); j++)
            {
                if (array1[i, j] != array2[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }
}

enum Direction { Left, Up, Right, Down }
