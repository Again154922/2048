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
    private static readonly string HistoryFile = Path.Combine(AppContext.BaseDirectory, "2048-best.txt");

    // 按 Esc 退出时保存的当前进度（exe 同目录）
    private static readonly string SaveFile = Path.Combine(AppContext.BaseDirectory, "2048-save.txt");

    private static int s_historyBest = ReadHistoryBest();
    
    private static void Main(string[] args)
    {
        Init();

        while (s_gamerunning)
        {
            HandleKey();

            bool isGameOver = CheckGameState();
            ShowMap();
            if (isGameOver)
            {
                Console.WriteLine($"游戏结束！本局最大 {GetCurrentBest()}，按 R 重新开始，按 Esc 退出游戏");
            }
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
                Console.Write((s_map[i][j] == 0 ? (int?)null : 1 << s_map[i][j]).ToString()?.PadRight(8));
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
            if (File.Exists(HistoryFile) && int.TryParse(File.ReadAllText(HistoryFile).Trim(), out int best))
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
            Directory.CreateDirectory(Path.GetDirectoryName(HistoryFile)!);
            File.WriteAllText(HistoryFile, best.ToString());
        }
        catch (Exception)
        {
            // 写不进去也不影响游戏
        }
    }
    
    private static void HandleKey()
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
                // 重开也算结束一局，先记一笔再重置地图
                RecordHistoryBest();
                InitMap();
                Console.Clear();
                break;
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
        int[][] beforeMap = CloneMap();

        for (int line = 0; line < 4; line++)
        {
            WriteLine(direction, line, MergeLine(ReadLine(direction, line)));
        }

        if (!MapsEqual(beforeMap, s_map))
        {
            AddRandomBlock();
        }
    }

    private static int[][] CloneMap()
    {
        int[][] copy = new int[4][];
        for (int i = 0; i < 4; i++)
        {
            copy[i] = (int[])s_map[i].Clone();
        }

        return copy;
    }

    private static bool MapsEqual(int[][] left, int[][] right)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (left[i][j] != right[i][j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    // 按移动方向从棋盘上取出一条线（下标 0 是最靠近目标边缘的那格）
    private static int[] ReadLine(int direction, int line)
    {
        int[] values = new int[4];
        for (int position = 0; position < 4; position++)
        {
            (int x, int y) = GetCell(direction, line, position);
            values[position] = s_map[x][y];
        }

        return values;
    }

    // 把合并后的线按同样的方向写回棋盘
    private static void WriteLine(int direction, int line, int[] values)
    {
        for (int position = 0; position < 4; position++)
        {
            (int x, int y) = GetCell(direction, line, position);
            s_map[x][y] = values[position];
        }
    }

    // 四个方向共用一套映射：第 line 条线、线上第 position 格（0 表示目标边缘）对应的行列下标
    private static (int x, int y) GetCell(int direction, int line, int position)
    {
        return direction switch
        {
            Left => (line, position),
            Up => (position, line),
            Right => (line, 3 - position),
            Down => (3 - position, line),
            _ => (line, position)
        };
    }

    // 单行压缩 + 合并：一次遍历，非零块依次填到前面，相邻且相等就合成一个更大的块
    // 内部用指数表示方块（1 表示 2、2 表示 4……），所以"合并"就是加一
    private static int[] MergeLine(int[] line)
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
            Console.Write(saved ? $"进度已保存到 {SaveFile}" : "保存失败：文件写不进去");
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
            using StreamWriter writer = new(SaveFile);
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
        if (!File.Exists(SaveFile))
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

            foreach (string line in File.ReadAllLines(SaveFile))
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
}
