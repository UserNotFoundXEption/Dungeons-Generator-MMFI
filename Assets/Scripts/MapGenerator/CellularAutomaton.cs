public class CellularAutomaton
{
    private readonly int Bmin;
    private readonly int Bmax;
    private readonly int Smin;
    private readonly int Smax;
    private readonly int R;
    private int[][] table;
    private int xSize;
    private int ySize;

    public CellularAutomaton(int Bmin, int Bmax, int Smin, int Smax, int R)
    {
        this.Bmin = Bmin;
        this.Bmax = Bmax;
        this.Smin = Smin;
        this.Smax = Smax;
        this.R = R;
    }

    public void SetTable(int[][] table)
    {
        this.table = table;
        this.xSize = table.Length;
        this.ySize = table[0].Length;
    }

    public int[][] GetNewIteration()
    {
        int[][] newTable = U.CopyTable(table);

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (!CanSurvive(x, y))
                {
                    newTable[x][y] = 0;
                }
                else if (CanBeBorn(x, y))
                {
                    newTable[x][y] = 1;
                }
            }
        }

        table = newTable;
        return newTable;
    }

    private bool CanBeBorn(int x, int y)
    {
        int NumberOfNeighbors = CountNeighbors(x, y);
        return NumberOfNeighbors >= Bmin && NumberOfNeighbors <= Bmax;
    }

    private bool CanSurvive(int x, int y)
    {
        int NumberOfNeighbors = CountNeighbors(x, y);
        return NumberOfNeighbors >= Smin && NumberOfNeighbors <= Smax;
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