using UnityEngine;

public class CellularAutomaton
{
    private readonly int Bmin;
    private readonly int Bmax;
    private readonly int Smin;
    private readonly int Smax;
    private readonly int R;
    private readonly bool makingHole;
    private int[][] table;
    private int[][] holeTable;
    private int xSize;
    private int ySize;

    public CellularAutomaton(int Bmin, int Bmax, int Smin, int Smax, int R, bool makingHole = false)
    {
        this.Bmin = Bmin;
        this.Bmax = Bmax;
        this.Smin = Smin;
        this.Smax = Smax;
        this.R = R;
        this.makingHole = makingHole;
    }

    public void SetTable(int[][] table)
    {
        this.table = table;
        this.xSize = table.Length;
        this.ySize = table[0].Length;
        holeTable = U.NewTable(xSize, ySize);
        Vector2Int massCenter = U.GetMassCenter(table);
        holeTable[massCenter.x][massCenter.y] = 1;
    }

    public int[][] Run(int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            int[][] newTable = U.CopyTable(table);
            int[][] newHoleTable = U.CopyTable(holeTable);

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    if (!CanSurvive(x, y))
                    {
                        newHoleTable[x][y] = 1;
                        newTable[x][y] = 0;
                    }
                    else if (CanBeBorn(x, y))
                    {
                        newTable[x][y] = 1;
                    }
                }
            }

            table = newTable;
            holeTable = newHoleTable;
        }

        return table;
    }

    private bool CanBeBorn(int x, int y)
    {
        int NumberOfNeighbors = CountNeighbors(x, y);
        return NumberOfNeighbors >= Bmin && NumberOfNeighbors <= Bmax;
    }

    private bool CanSurvive(int x, int y)
    {
        int NumberOfNeighbors = CountNeighbors(x, y);
        bool canSurvive = NumberOfNeighbors >= Smin && NumberOfNeighbors <= Smax;
        if (!canSurvive && makingHole)
        {
            if (HasHoleNeighbor(x, y))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        return canSurvive;
    }

    private bool HasHoleNeighbor(int centerX, int centerY)
    {
        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            for (int y = centerY - 1; y <= centerY + 1; y++)
            {
                if ((x != centerX || y != centerY) && x >= 0 && y >= 0 && x < xSize && y < ySize && holeTable[x][y] == 1)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private int CountNeighbors(int centerX, int centerY)
    {
        int counter = 0;

        for (int x = centerX - R; x <= centerX + R; x++)
        {
            for (int y = centerY - R; y <= centerY + R; y++)
            {
                if ((x != centerX || y != centerY) && x >= 0 && y >= 0 && x < xSize && y < ySize && table[x][y] == 1)
                {
                    counter++;
                }
            }
        }

        return counter;
    }
}