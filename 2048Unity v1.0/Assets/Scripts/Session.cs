using UnityEngine;

public static class Session
{
    public const string HistoryFilePath = "./save.txt";
    public const string HistoryBestKey = "2048_best";
    
    private static readonly (float x, float y)[,] mapXYs = {
        {(-1.65F, 1.65F), (-0.55F, 1.65F), (0.55F, 1.65F), (1.65F, 1.65F)},
        {(-1.65F, 0.55F), (-0.55F, 0.55F), (0.55F, 0.55F), (1.65F, 0.55F)},
        {(-1.65F, -0.55F), (-0.55F, -0.55F), (0.55F, -0.55F), (1.65F, -0.55F)},
        {(-1.65F, -1.65F), (-0.55F, -1.65F), (0.55F, -1.65F), (1.65F, -1.65F)}
    };
    public static readonly Vector3[,] MapXYs = GetVectorArray(mapXYs);
    public static int[,] MapSave = new int[4, 4];

    private static Vector3[,] GetVectorArray((float x, float y)[,] xys)
    {
        int lengthX = xys.GetLength(0);
        int lengthY = xys.GetLength(1);
        
        Vector3[,] result = new Vector3[lengthX, lengthY];
        for (int i = 0; i < lengthX; i++)
        {
            for (int j = 0; j < lengthY; j++)
            {
                result[i, j] = new Vector3(xys[i, j].x, xys[i, j].y);
            }
        }

        return result;
    }
}