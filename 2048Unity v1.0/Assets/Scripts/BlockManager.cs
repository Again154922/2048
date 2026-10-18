using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BlockManager : MonoBehaviour
{
    public static BlockManager Instance { get; private set; }
    public int CurrentBest;
    public int HistoryBest;
    private int[,] map = new int[4, 4];
    private List<GameObject> blockList = new();
    [SerializeField] private GameObject blockPrefab0;
    [SerializeField] private GameObject blockPrefab2;
    [SerializeField] private GameObject blockPrefab4;
    [SerializeField] private GameObject blockPrefab8;
    [SerializeField] private GameObject blockPrefab16;
    [SerializeField] private GameObject blockPrefab32;
    [SerializeField] private GameObject blockPrefab64;
    [SerializeField] private GameObject blockPrefab128;
    [SerializeField] private GameObject blockPrefab256;
    [SerializeField] private GameObject blockPrefab512;
    [SerializeField] private GameObject blockPrefab1024;
    [SerializeField] private GameObject blockPrefab2048;
    [SerializeField] private GameObject blockPrefab4096;
    [SerializeField] private GameObject blockPrefab8192;
    [SerializeField] private GameObject blockPrefab16384;
    [SerializeField] private GameObject blockPrefab32768;

    private GameObject[] prefabs;
    
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        HistoryBest = PlayerPrefs.GetInt(Session.NewBestKey, 0);
        
        prefabs = new [] {
            blockPrefab0,
            blockPrefab2,
            blockPrefab4,
            blockPrefab8,
            blockPrefab16,
            blockPrefab32,
            blockPrefab64,
            blockPrefab128,
            blockPrefab256,
            blockPrefab512,
            blockPrefab1024,
            blockPrefab2048,
            blockPrefab4096,
            blockPrefab8192,
            blockPrefab16384,
            blockPrefab32768
        };
    }

    void Start()
    {
        InitMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitMap()
    {
        if (!Equal(Session.MapSave, new int[4, 4]))
        {
            DeepCopy(Session.MapSave, out map);
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
                blockList.Add(Instantiate(prefabs[map[i, j]],
                    new Vector3(Session.MapXYs[i, j].x, Session.MapXYs[i, j].y), Quaternion.identity, transform));
            }
        }
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

    bool Equal(int[,] sourse, int[,] destination)
    {
        if (sourse.GetLength(0) != destination.GetLength(0) || sourse.GetLength(1) != destination.GetLength(1))
        {
            return false;
        }

        for (int i = 0; i < sourse.GetLength(0); i++)
        {
            for (int j = 0; j < sourse.GetLength(1); j++)
            {
                if (sourse[i, j] != destination[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }
}
