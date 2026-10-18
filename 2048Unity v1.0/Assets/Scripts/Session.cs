public static class Session
{
    public const string HistoryFilePath = "./save.txt";
    public static readonly (float x, float y)[,] MapXYs = {
        {(-1.65F, 1.65F), (-0.55F, 1.65F), (0.55F, 1.65F), (1.65F, 1.65F)},
        {(-1.65F, 0.55F), (-0.55F, 0.55F), (0.55F, 0.55F), (1.65F, 0.55F)},
        {(-1.65F, -0.55F), (-0.55F, -0.55F), (0.55F, -0.55F), (1.65F, -0.55F)},
        {(-1.65F, -1.65F), (-0.55F, -1.65F), (0.55F, -1.65F), (1.65F, -1.65F)}
    };
    public const string NewBestKey = "2048_best";
    public static int[,] MapSave = new int[4, 4];
}