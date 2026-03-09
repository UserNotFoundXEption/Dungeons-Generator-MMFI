using System;

public class Utilities
{
    public static int NX(int oldX, int direction, int step)
    {
        return direction % 2 == 0 ? oldX - (direction - 3) * step : oldX;
    }   // prawo: x - 2 + 3 lewo: x - 4 + 3

    public static int NX(int oldX, int direction)
    {
        return direction % 2 == 0 ? oldX - direction + 3 : oldX;
    }

    public static int NY(int oldY, int direction, int step)
    {
        return direction % 2 == 1 ? oldY - direction + 2 : oldY;
    }   // gora: y - 1 + 2 dol: y - 3 + 2

    public static int NY(int oldY, int direction)
    {
        return direction % 2 == 1 ? oldY - direction + 2 : oldY;
    }

    public static int[][] NewTable(int xSize, int ySize)
    {
        int[][] table = new int[xSize][];
        for (int x = 0; x < xSize; x++)
        {
            table[x] = new int[ySize];
        }

        return table;
    }

    public static int[][] CopyTable(int[][] table)
    {
        int xSize = table.Length;
        int ySize = table[0].Length;
        int[][] newTable = U.NewTable(xSize, ySize);

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                newTable[x][y] = table[x][y];
            }
        }

        return newTable;
    }

    public static int R(int direction, int rotation)
    {
        direction += rotation;
        if (direction < 1)
        {
            direction += 4;
        }
        if (direction > 4)
        {
            direction -= 4;
        }
        return direction;
    }

    public static bool InDungeon(int x, int y, int n)
    {
        return x >= 0 && y >= 0 && x < n && y < n;
    }

    public static bool InDungeon(int x, int y, int n, int r)
    {
        return x >= r && y >= r && x < n - r && y < n - r;
    }

    public static bool InBounds(int x, int y, BoundsInt2D room)
    {
        return x >= room.x && x <= room.xMax && y >= room.y && y <= room.yMax;
    }

    public static bool InBounds(int x, int y, int xSize, int ySize)
    {
        return x >= 0 && x < xSize && y >= 0 && y < ySize;
    }

    public static bool InBounds(int x, int y, int xSize, int ySize, int r)
    {
        return x >= r && x < xSize - r && y >= r && y < ySize - r;
    }

    public static bool Contains(int[] table, int value)
    {
        int n = table.Length;
        for (int i = 0; i < n; i++)
        {
            if (table[i] == value)
            {
                return true;
            }
        }
        return false;
    }

    public static string GetRoomCoords(BoundsInt2D room)
    {
        return " " + room.xCenter + "; " + room.yCenter + " ";
    }

    public static bool Neighbors(int x, int y, int neighbor, int[][] table)
    {
        bool left = x > 0 && table[x - 1][y] == neighbor;
        bool right = x < table.Length - 1 && table[x + 1][y] == neighbor;
        bool up = y > 0 && table[x][y - 1] == neighbor;
        bool down = y < table[0].Length - 1 && table[x][y + 1] == neighbor;
        return left || right || up || down;
    }

    public static String ToString(int[] table)
    {
        String result = "" + table[0];
        for (int i = 1; i < table.Length; i++)
        {
            result += ", " + table[i];
        }
        return result;
    }
}

public class U : Utilities { }