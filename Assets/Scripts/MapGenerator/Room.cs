using System;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class Room
{
    public int xSize;
    public int ySize;
    private readonly int offset;
    private int[][] table;
    private int safetyBreak = 0;
    private readonly int safetyBreakLimit = 10;
    public Room(BoundsInt2D roomBounds, int offset)
    {
        this.xSize = roomBounds.xMax - roomBounds.x;
        this.ySize = roomBounds.yMax - roomBounds.y;
        this.offset = offset;
        this.table = U.NewTable(xSize, ySize);
    }

    public int[][] Create(bool reshapeCut, int chanceForHorizontal, bool continueAfterIteration, int cellFixIterations, bool cellRounding, int holesRadius)
    {
        do
        {
            safetyBreak++;
            FillTable(table);
            if (reshapeCut)
            {
                ReshapeCut();
            }
            if (holesRadius > -1)
            {
                if (holesRadius == 0)
                {
                    holesRadius = Math.Min(xSize, ySize) / 3 - 2;
                }
                MakeHoles(holesRadius);
            }
            if (chanceForHorizontal != -1)
            {
                ReshapeRandomWalk(chanceForHorizontal, continueAfterIteration);
            }
            FixWithCellularAutomaton(cellFixIterations, cellRounding);
        } while (!VerifyRoomIntegrity() && safetyBreak != safetyBreakLimit);

        if (safetyBreak == safetyBreakLimit)
        {
            Debug.LogError("Can't make intergral room");
        }

        return table;
    }

    private void FillTable(int[][] emptyTable)
    {
        FillTable(emptyTable, offset, xSize - offset, offset, ySize - offset);
    }

    private void FillTable(int[][] emptyTable, int xMin, int xMax, int yMin, int yMax)
    {
        for (int x = xMin; x < xMax; x++)
        {
            for (int y = yMin; y < yMax; y++)
            {
                emptyTable[x][y] = 1;
            }
        }
    }
    private void ApplyMask(int[][] filledTable, int[][] mask)
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (mask[x][y] == 1)
                {
                    filledTable[x][y] = 0;
                }
            }
        }
    }


    private void ReshapeCut()
    {
        int xMin, xMax, yMin, yMax;
        int xCut = Random.Range(offset, xSize - offset + 1);
        int yCut = Random.Range(offset, ySize - offset + 1);

        if (xCut > xSize / 2)
        {
            xMin = xCut / 2 + xSize / 4;
            xMax = xSize - offset;
        }
        else
        {
            xMin = offset;
            xMax = xCut / 2 + xSize / 4;
        }
        if (yCut > ySize / 2)
        {
            yMin = yCut / 2 + ySize / 4;
            yMax = ySize - offset;
        }
        else
        {
            yMin = offset;
            yMax = yCut / 2 + ySize / 4;
        }



        int[][] mask = U.NewTable(xSize, ySize);
        FillTable(mask, xMin, xMax, yMin, yMax);
        ApplyMask(table, mask);
    }

    private void ReshapeRandomWalk(int chanceForHorizontal, bool continueAfterIteration)
    {
        int[][] newTable = U.NewTable(xSize, ySize);
        RandomWalk randomWalkRef = new(newTable);

        for (int x = offset; x < xSize; x++)
        {
            for (int y = offset; y < ySize; y++)
            {
                if (table[x][y] == 1 && Random.Range(1, 5) == 1)
                {
                    if (continueAfterIteration)
                    {
                        randomWalkRef.SendWalker(x, y, 3, 2, chanceForHorizontal, true);
                        randomWalkRef.SendWalker(x, y, 3, 3, chanceForHorizontal, false);
                    }
                    else
                    {
                        randomWalkRef.SendWalker(x, y, 5, 5, chanceForHorizontal, false);
                    }
                }
            }
        }

        table = newTable;
    }

    private void FixWithCellularAutomaton(int iterations, bool rounding)
    {
        CellularAutomaton cellAutRef = new(7, 8, rounding ? 4 : 1, 8, 1);
        cellAutRef.SetTable(table);
        for (int i = 0; i < iterations; i++)
        {
            table = cellAutRef.GetNewIteration();
        }
    }

    private void MakeHoles(int R)
    {
        double area = Math.Pow(R * 2 + 1, 2) - 1;
        CellularAutomaton cellAutRef = new(0, 0, 1, (int)(area * 0.9), R);
        cellAutRef.SetTable(table);
        cellAutRef.SetTable(table);
        table = cellAutRef.GetNewIteration();
    }

    private bool VerifyRoomIntegrity()
    {
        int[][] verificationTable = U.CopyTable(table);
        int x, y;
        do
        {
            x = Random.Range(0, xSize);
            y = Random.Range(0, ySize);
        } while (verificationTable[x][y] != 1);
        verificationTable[x][y] = 2;

        int tilesFound = 1;
        while (tilesFound > 0)
        {
            tilesFound = 0;
            for (x = 0; x < xSize; x++)
            {
                for (y = 0; y < ySize; y++)
                {
                    if (verificationTable[x][y] == 1 && U.Neighbors(x, y, 2, verificationTable))
                    {
                        verificationTable[x][y] = 2;
                        tilesFound++;
                        if (y > 0)
                        {
                            y -= 2;
                        }
                        else
                        {
                            if (x > 0)
                            {
                                x -= 2;
                            }
                        }
                    }
                }
            }
        }

        for (x = 0; x < xSize; x++)
        {
            for (y = 0; y < ySize; y++)
            {
                if (verificationTable[x][y] == 1)
                {
                    return false;
                }
            }
        }
        return true;
    }
}