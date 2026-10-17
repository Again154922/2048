using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace Console2048;

internal static class Program
{
    private const int Left = 0;
    private const int Up = 1;
    private const int Right = 2;
    private const int Down = 3;
    private const int BoardWidth = 32;
    private static int[][] s_map = null!;
    private static bool s_gamerunning;

    private static Random s_random = new();

    // 历史最高记录保存在电脑上（exe 同目录），跨局保留
    private static readonly string s_historyFile = Path.Combine(AppContext.BaseDirectory, "2048-best.txt");

    // 按 Esc 退出时保存的当前进度（exe 同目录）
    private static readonly string s_saveFile = Path.Combine(AppContext.BaseDirectory, "2048-save.txt");

    private static int s_historyBest = ReadHistoryBest();
    
    private static void Main(string[] args)
    {
        Init();
        while (s_gamerunning)
        {
            HandleKey();
            ShowMap();
        }
    }

    private static void Init()
    {
        Console.CursorVisible = false;

        Console.WriteLine("2048控制台版");

        if (!TryLoadSavedMap())
        {
            InitMap();
        }

        Console.WriteLine("按方向键/WASD移动方块，按Esc退出游戏，按R重新开始游戏");
        Console.WriteLine("按任意键开始游戏");
        Console.ReadKey(true);
        s_gamerunning = true;
        Console.Clear();
        ShowMap();
    }

    private static void InitMap()
    {
        s_map =
        [
            [0, 0, 0, 0],
            [0, 0, 0, 0],
            [0, 0, 0, 0],
            [0, 0, 0, 0]
        ];
        Console.WriteLine("初始化地图中...");
        (int x, int y) block1 = (s_random.Next(0, 4), s_random.Next(0, 4));
        (int x, int y) block2;
        do
        {
            block2 = (s_random.Next(0, 4), s_random.Next(0, 4));
        } while (block1 == block2);
        s_map[block1.x][block1.y] = s_random.Next(1, 3);
        s_map[block2.x][block2.y] = s_random.Next(1, 3);
        Console.WriteLine("初始化完成");
    }

    private static void ShowMap()
    {
        Console.SetCursorPosition(0, 0);
        
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                // 固定宽度输出：用空格补齐，避免数字位数变少时残留旧字符
                Console.Write((s_map[i][j] == 0 ? 0 : 1 << s_map[i][j]).ToString().PadRight(8));
            }
            Console.WriteLine();
        }

        ShowStats();
    }

    // 棋盘下方左右两端：左边贴棋盘左下角，右边贴棋盘右下角
    private static void ShowStats()
    {
        string left = $"历史最高 {s_historyBest}";
        string right = $"当前最高 {GetCurrentBest()}";

        int rightColumn = BoardWidth - DisplayWidth(right);
        if (rightColumn < DisplayWidth(left) + 2)
        {
            rightColumn = DisplayWidth(left) + 2;
        }

        ClearLine(4);
        Console.Write(left);

        if (rightColumn + DisplayWidth(right) <= Console.WindowWidth)
        {
            Console.SetCursorPosition(rightColumn, 4);
            Console.Write(right);
        }
        else
        {
            Console.Write("  " + right);
        }

        ClearLine(5);
    }

    private static void ClearLine(int row)
    {
        Console.SetCursorPosition(0, row);
        Console.Write(new string(' ', Math.Max(Console.WindowWidth - 1, 0)));
        Console.SetCursorPosition(0, row);
    }

    // 中文字符按两个字符宽度计算，用于把"当前最高"贴到右下角
    private static int DisplayWidth(string text)
    {
        return text.Sum(c => c > 0x7F ? 2 : 1);
    }

    private static int GetCurrentBest()
    {
        int maxExponent = s_map.SelectMany(row => row).Max();
        return maxExponent == 0 ? 0 : 1 << maxExponent;
    }

    private static int ReadHistoryBest()
    {
        try
        {
            if (File.Exists(s_historyFile) && int.TryParse(File.ReadAllText(s_historyFile).Trim(), out int best))
            {
                return best;
            }
        }
        catch (Exception)
        {
            // 读不到就当没有记录
        }

        return 0;
    }

    private static void SaveHistoryBest(int best)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(s_historyFile)!);
            File.WriteAllText(s_historyFile, best.ToString());
        }
        catch (Exception)
        {
            // 写不进去也不影响游戏
        }
    }
    
    private static void HandleKey()
    {
        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    MoveMap(Left);
                    break;
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    MoveMap(Up);
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    MoveMap(Right);
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    MoveMap(Down);
                    break;
                case ConsoleKey.Escape:
                    Exit();
                    break;
                case ConsoleKey.R:
                    InitMap();
                    Console.Clear();
                    ShowMap();
                    break;
            }
            bool isGameOver = CheckGameState();
            ShowMap();
            if (isGameOver)
            {
                Console.WriteLine($"游戏结束！本局最大 {GetCurrentBest()}，按 R 重新开始，按 Esc 退出游戏");
            }
        }
    }

    // 返回是否已无法移动；死亡时把本局最大值记入历史最高并存到电脑上
    private static bool CheckGameState()
    {
        if (CanMove())
        {
            return false;
        }

        RecordHistoryBest();

        return true;
    }

    // 本局最大值超过历史记录时，更新并写到电脑上
    private static void RecordHistoryBest()
    {
        int currentBest = GetCurrentBest();
        if (currentBest > s_historyBest)
        {
            s_historyBest = currentBest;
            SaveHistoryBest(s_historyBest);
        }
    }

    private static bool CanMove()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (s_map[i][j] == 0)
                {
                    return true;
                }

                if (j + 1 < 4 && s_map[i][j] == s_map[i][j + 1])
                {
                    return true;
                }

                if (i + 1 < 4 && s_map[i][j] == s_map[i + 1][j])
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    private static void MoveMap(int direction)
    {
        int[][] beforeMap = s_map.Select(row => (int[])row.Clone()).ToArray();

        switch (direction)
        {
            case Left:
                for (int i = 0; i < 4; i++)
                {
                    MoveRow([s_map[i][0], s_map[i][1], s_map[i][2], s_map[i][3]], out int[] newRow);
                    s_map[i][0] = newRow[0];
                    s_map[i][1] = newRow[1];
                    s_map[i][2] = newRow[2];
                    s_map[i][3] = newRow[3];
                }
                
                break;
            
            case Up:
                for (int j = 0; j < 4; j++)
                {
                    MoveRow([s_map[0][j], s_map[1][j], s_map[2][j], s_map[3][j]], out int[] newRow);
                    s_map[0][j] = newRow[0];
                    s_map[1][j] = newRow[1];
                    s_map[2][j] = newRow[2];
                    s_map[3][j] = newRow[3];
                }
                
                break;
            
            case Right:
                for (int i = 0; i < 4; i++)
                {
                    MoveRow([s_map[i][3], s_map[i][2], s_map[i][1], s_map[i][0]], out int[] newRow);
                    s_map[i][3] = newRow[0];
                    s_map[i][2] = newRow[1];
                    s_map[i][1] = newRow[2];
                    s_map[i][0] = newRow[3];
                }
                
                break;
            
            case Down:
                for (int j = 0; j < 4; j++)
                {
                    MoveRow([s_map[3][j], s_map[2][j], s_map[1][j], s_map[0][j]], out int[] newRow);
                    s_map[3][j] = newRow[0];
                    s_map[2][j] = newRow[1];
                    s_map[1][j] = newRow[2];
                    s_map[0][j] = newRow[3];
                }
                break;
        }

        bool hasMoved = false;
        for (int i = 0; i < 4 && !hasMoved; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (s_map[i][j] != beforeMap[i][j])
                {
                    hasMoved = true;
                    break;
                }
            }
        }

        if (hasMoved)
        {
            AddRandomBlock();
        }
    }

    private static void AddRandomBlock()
    {
        List<(int x, int y)> emptyBlocks = [];
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (s_map[i][j] == 0)
                {
                    emptyBlocks.Add((i, j));
                }
            }
        }

        if (emptyBlocks.Count == 0)
        {
            return;
        }

        (int x, int y) position = emptyBlocks[s_random.Next(emptyBlocks.Count)];
        s_map[position.x][position.y] = s_random.Next(1, 3);
    }

    private static void MoveRow(int[] row, out int[] newRow)
    {
        int[] zeroIndices = FindZero(row);
        
        if (zeroIndices.Length == 4)
        {
            newRow = [0, 0, 0, 0];
            return;
        }
        if (zeroIndices.Length == 3)
        {
            newRow =
            [
                row[zeroIndices.Contains(0) ? zeroIndices.Contains(1) ? zeroIndices.Contains(2) ? 3 : 2 : 1 : 0],
                0, 0, 0
            ];
            return;
        }
        if (zeroIndices.Length == 2)
        {
            if (zeroIndices[0] == 0 && zeroIndices[1] == 1)
            {
                if (row[2] == row[3])
                {
                    newRow = [++row[2], 0, 0, 0];
                }
                else
                {
                    newRow = [row[2], row[3], 0, 0];
                }

                return;
            }
            if (zeroIndices[0] == 0 && zeroIndices[1] == 2)
            {
                if (row[1] == row[3])
                {
                    newRow = [++row[1], 0, 0, 0];
                }
                else
                {
                    newRow = [row[1], row[3], 0, 0];
                }

                return;
            }
            if (zeroIndices[0] == 0 && zeroIndices[1] == 3)
            {
                if (row[1] == row[2])
                {
                    newRow = [++row[1], 0, 0, 0];
                }
                else
                {
                    newRow = [row[1], row[2], 0, 0];
                }

                return;
            }
            if (zeroIndices[0] == 1 && zeroIndices[1] == 2)
            {
                if (row[0] == row[3])
                {
                    newRow = [++row[0], 0, 0, 0];
                }
                else
                {
                    newRow = [row[0], row[3], 0, 0];
                }

                return;
            }
            if (zeroIndices[0] == 1 && zeroIndices[1] == 3)
            {
                if (row[0] == row[2])
                {
                    newRow = [++row[0], 0, 0, 0];
                }
                else
                {
                    newRow = [row[0], row[2], 0, 0];
                }

                return;
            }
            if (zeroIndices[0] == 2 && zeroIndices[1] == 3)
            {
                if (row[0] == row[1])
                {
                    newRow = [++row[0], 0, 0, 0];
                }
                else
                {
                    newRow = [row[0], row[1], 0, 0];
                }

                return;
            }
        }
        if (zeroIndices.Length == 1)
        {
            if (zeroIndices[0] == 0)
            {
                if (row[1] == row[2])
                {
                    newRow = [++row[1], row[3], 0, 0];
                }
                else if (row[2] == row[3])
                {
                    newRow = [row[1], ++row[2], 0, 0];
                }
                else
                {
                    newRow = [row[1], row[2], row[3], 0];
                }

                return;
            }
            if (zeroIndices[0] == 1)
            {
                if (row[0] == row[2])
                {
                    newRow = [++row[0], row[3], 0, 0];
                }
                else if (row[2] == row[3])
                {
                    newRow = [row[0], ++row[2], 0, 0];
                }
                else
                {
                    newRow = [row[0], row[2], row[3], 0];
                }

                return;
            }
            if (zeroIndices[0] == 2)
            {
                if (row[0] == row[1])
                {
                    newRow = [++row[0], row[3], 0, 0];
                }
                else if (row[1] == row[3])
                {
                    newRow = [row[0], ++row[1], 0, 0];
                }
                else
                {
                    newRow = [row[0], row[1], row[3], 0];
                }

                return;
            }
            if (zeroIndices[0] == 3)
            {
                if (row[0] == row[1])
                {
                    newRow = [++row[0], row[2], 0, 0];
                }
                else if (row[1] == row[2])
                {
                    newRow = [row[0], ++row[1], 0, 0];
                }
                else
                {
                    newRow = [row[0], row[1], row[2], 0];
                }

                return;
            }
        }
        if (zeroIndices.Length == 0)
        {
            if (row[0] == row[1] && row[2] != row[3])
            {
                newRow = [++row[0], row[2], row[3], 0];
            }
            else if (row[1] == row[2] && row[0] != row[1])
            {
                newRow = [row[0], ++row[1], row[3], 0];
            }
            else if (row[2] == row[3] && row[0] != row[1])
            {
                newRow = [row[0], row[1], ++row[2], 0];
            }
            else if (row[0] == row[1] && row[2] == row[3])
            {
                newRow = [++row[0], ++row[2], 0, 0];
            }
            else
            {
                newRow = [row[0], row[1], row[2], row[3]];
            }

            return;
        }

        newRow = [row[0], row[1], row[2], row[3]];
    }

    private static void Exit()
    {
        // 按 Esc 退出也把这局的最大值记进历史最高
        RecordHistoryBest();

        ClearLine(6);
        Console.Write("是否保存当前进度？(Y/N) ");
        ConsoleKeyInfo answer = Console.ReadKey(true);
        Console.Write(answer.Key == ConsoleKey.Y ? "Y" : "N");

        if (answer.Key == ConsoleKey.Y)
        {
            bool saved = SaveMapProgress();
            ClearLine(7);
            Console.Write(saved ? $"进度已保存到 {s_saveFile}" : "保存失败：文件写不进去");
        }

        ClearLine(8);
        Console.Write("按任意键退出游戏");
        Console.ReadKey(true);

        try
        {
            Console.In.Close();
        }
        catch (Exception)
        {
            // ignored
        }
        
        s_gamerunning = false;
        Environment.Exit(0);
    }

    // 把完整地图保存下来：每行 4 个格子，写的是显示值（2、4、8……），空格记 0
    private static bool SaveMapProgress()
    {
        try
        {
            using StreamWriter writer = new(s_saveFile);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    writer.Write((s_map[i][j] == 0 ? 0 : 1 << s_map[i][j]).ToString().PadRight(8));
                }

                writer.WriteLine();
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    // 启动时如果发现上次的存档，问玩家要不要接着玩
    private static bool TryLoadSavedMap()
    {
        if (!File.Exists(s_saveFile))
        {
            return false;
        }

        Console.Write("发现上次保存的进度，是否继续？(Y/N) ");
        ConsoleKeyInfo answer = Console.ReadKey(true);
        Console.Write(answer.Key == ConsoleKey.Y ? "Y" : "N");
        Console.WriteLine();

        if (answer.Key != ConsoleKey.Y)
        {
            return false;
        }

        if (ReadSavedMap())
        {
            return true;
        }

        Console.WriteLine("存档读取失败，开始新游戏");
        return false;
    }

    // 读回存档：文件里是显示值（2、4、8……），换算成内部使用的指数
    private static bool ReadSavedMap()
    {
        try
        {
            int[][] map = new int[4][];
            int lineIndex = 0;

            foreach (string line in File.ReadAllLines(s_saveFile))
            {
                if (line.Trim() == "")
                {
                    continue;
                }

                if (lineIndex >= 4)
                {
                    break;
                }

                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 4)
                {
                    return false;
                }

                map[lineIndex] = new int[4];
                for (int j = 0; j < 4; j++)
                {
                    if (!int.TryParse(parts[j], out int value) || value < 0)
                    {
                        return false;
                    }

                    if (value == 0)
                    {
                        map[lineIndex][j] = 0;
                        continue;
                    }

                    int exponent = BitOperations.Log2((uint)value);
                    if (1 << exponent != value)
                    {
                        return false;   // 不是 2 的幂，说明文件被改坏了
                    }

                    map[lineIndex][j] = exponent;
                }

                lineIndex++;
            }

            if (lineIndex < 4)
            {
                return false;
            }

            s_map = map;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static int[] FindZero(int[] row)
    {
        List<int> zeroIndices = [];
        if (row[0] == 0) zeroIndices.Add(0);
        if (row[1] == 0) zeroIndices.Add(1);
        if (row[2] == 0) zeroIndices.Add(2);
        if (row[3] == 0) zeroIndices.Add(3);
        return [.. zeroIndices];
    }
}
