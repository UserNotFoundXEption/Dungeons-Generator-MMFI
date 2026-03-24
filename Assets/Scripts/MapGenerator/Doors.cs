using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Doors
{
    int x1;
    int y1;
    int d1;
    int x2;
    int y2;
    int d2;
    int xCorner;
    int yCorner;
    readonly BoundsInt2D room1;
    readonly BoundsInt2D room2;
    readonly int[][] dungeonTable;
    readonly int maxCornersSeeked = 100;
    bool straightCorridor = false;

    public Doors(BoundsInt2D room1, BoundsInt2D room2, int[][] dungeonTable)
    {
        this.room1 = room1;
        this.room2 = room2;
        this.dungeonTable = dungeonTable;
        int[] validXCenters = new int[maxCornersSeeked];
        int[] validYCenters = new int[maxCornersSeeked];
        int cornersFound = GetCenters(validXCenters, validYCenters);

        if (cornersFound == 0)
        {
            x1 = int.MaxValue;
            y1 = 0;
            x2 = 0;
            y2 = 0;
        }
        else
        {
            int randomId;
            if (straightCorridor)
            {
                randomId = cornersFound / 2;
            }
            else
            {
                if (cornersFound > 2)
                {
                    randomId = Random.Range(1, cornersFound - 1);
                }
                else
                {
                    randomId = Random.Range(0, cornersFound);
                }
                randomId = (randomId * 3 / 4 + cornersFound / 2 / 4) / 2;
            }
            xCorner = validXCenters[randomId];
            yCorner = validYCenters[randomId];
            GetDoorsFromCorner();
        }
    }

    public int GetDistance()
    {
        return Math.Abs(x1 - x2 + y1 - y2);
    }

    private int GetCenters(int[] validCornersX, int[] validCornersY)
    {
        xCorner = room1.xCenter;
        yCorner = room2.yCenter;
        int xMax = room2.xCenter;
        int yMax = room1.yCenter;
        double xDelta = (xMax - xCorner) * 1.0 / maxCornersSeeked;
        double yDelta = (yMax - yCorner) * 1.0 / maxCornersSeeked;

        double xCornerDouble = xCorner * 1.0;
        double yCornerDouble = yCorner * 1.0;
        int cornersFound = 0;
        for (int i = 0; i < maxCornersSeeked; i++)
        {
            xCornerDouble += xDelta;
            yCornerDouble += yDelta;
            xCorner = (int)xCornerDouble;
            yCorner = (int)yCornerDouble;
            GetDoorsFromCorner();
            if (x1 == x2 || y1 == y2)
            {
                straightCorridor = true;
            }

            bool doorsInRooms = U.InBounds(x1, y1, room1) && U.InBounds(x2, y2, room2);
            bool doorsInDungeon = U.InDungeon(x1, y1, dungeonTable.Length, 1) && U.InDungeon(x2, y2, dungeonTable.Length, 1);
            if (dungeonTable[xCorner][yCorner] == 0 && doorsInRooms && doorsInDungeon)
            {
                validCornersX[cornersFound] = xCorner;
                validCornersY[cornersFound] = yCorner;
                cornersFound++;
            }
        }

        return cornersFound;
    }

    private void GetDoorsFromCorner()
    {
        d1 = GetDirection(room1);
        d2 = GetDirection(room2);
        Vector2Int doorPosition1 = GetDoorPosition(d1);
        Vector2Int doorPosition2 = GetDoorPosition(d2);
        x1 = doorPosition1[0];
        y1 = doorPosition1[1];
        x2 = doorPosition2[0];
        y2 = doorPosition2[1];
    }

    private int GetDirection(BoundsInt2D room)
    {
        if (U.InBounds(xCorner, yCorner, room))
        {
            return GetDirectionInBounds(room);
        }
        else
        {
            return GetDirectionOutBounds(room);
        }
    }

    private int GetDirectionInBounds(BoundsInt2D room)
    {
        int[] distancesToBounds = new int[] { int.MaxValue, room.yMax - yCorner, room.xMax - xCorner, yCorner - room.y, xCorner - room.x };
        int direction = 1, minDistance = int.MaxValue;
        for (int i = 1; i < 5; i++)
        {
            if (distancesToBounds[i] < minDistance)
            {
                minDistance = distancesToBounds[i];
                direction = i;
            }
        }
        return direction;
    }

    private int GetDirectionOutBounds(BoundsInt2D room)
    {
        if (yCorner > room.yMax)
        {
            return 1;
        }
        if (yCorner < room.y)
        {
            return 3;
        }
        if (xCorner > room.xMax)
        {
            return 2;
        }
        else
        {
            return 4;
        }
    }

    private Vector2Int GetDoorPosition(int direction)
    {
        int x = xCorner;
        int y = yCorner;
        int n = dungeonTable.Length;
        int moveDirection = U.R(direction, 2);
        int neighbor1Direction = U.R(direction, 1);
        int neighbor2Direction = U.R(direction, -1);
        int neighbor1 = 0;
        int neighbor2 = 0;

        while (U.InDungeon(x, y, n, 1) && (dungeonTable[x][y] != 1 || (neighbor1 != 1 && neighbor2 != 1)))
        {
            if (direction % 2 == 0)
            {
                x = U.NX(x, moveDirection, 1);
            }
            else
            {
                y = U.NY(y, moveDirection, 1);
            }

            try
            {
                neighbor1 = dungeonTable[U.NX(x, neighbor1Direction)][U.NY(y, neighbor1Direction)];
                neighbor2 = dungeonTable[U.NX(x, neighbor2Direction)][U.NY(y, neighbor2Direction)];
            }
            catch
            {
                break;
            }
        }

        return new Vector2Int(x, y);
    }

    public void Join()
    {
        int[] doorsCoords = new int[] { x1, y1, x2, y2 };
        int[] corners = new int[] { xCorner, yCorner, xCorner, yCorner };
        int delta, sign;
        for (int i = 0; i < 4; i++)
        {
            delta = corners[i] - doorsCoords[i];
            if (delta != 0)
            {
                sign = delta / Math.Abs(delta);
                if (i % 2 == 0)
                {
                    JoinDoorWithCornerByX(doorsCoords[i], corners[i], sign);
                }
                else
                {
                    JoinDoorWithCornerByY(doorsCoords[i], corners[i], sign);
                }
            }
        }
        ClearCornerNeighbors();
    }
    private void JoinDoorWithCornerByX(int start, int end, int sign)
    {
        if (NeighborToPathOrDoor(true, start))
        {
            dungeonTable[start][yCorner] = 2;
        }
        for (int x = start + sign; x != end + sign; x += sign)
        {
            if (NeighborToPathOrDoor(true, x))
            {
                dungeonTable[x][yCorner] = 3;
                if (dungeonTable[x][yCorner + 1] == 1)
                {
                    dungeonTable[x][yCorner + 1] = 0;
                }
                if (dungeonTable[x][yCorner - 1] == 1)
                {
                    dungeonTable[x][yCorner - 1] = 0;
                }
            }
        }
    }

    private void JoinDoorWithCornerByY(int start, int end, int sign)
    {
        if (NeighborToPathOrDoor(false, start))
        {
            dungeonTable[xCorner][start] = 2;
        }
        for (int y = start + sign; y != end + sign; y += sign)
        {
            if (NeighborToPathOrDoor(false, y))
            {
                dungeonTable[xCorner][y] = 3;
                if (dungeonTable[xCorner + 1][y] == 1)
                {
                    dungeonTable[xCorner + 1][y] = 0;
                }
                if (dungeonTable[xCorner - 1][y] == 1)
                {
                    dungeonTable[xCorner - 1][y] = 0;
                }
            }
        }
    }

    private bool NeighborToPathOrDoor(bool byX, int coord)
    {
        if (byX)
        {
            return dungeonTable[coord][yCorner + 1] < 2 && dungeonTable[coord][yCorner - 1] < 2;
        }
        else
        {
            return dungeonTable[xCorner + 1][coord] < 2 && dungeonTable[xCorner - 1][coord] < 2;
        }
    }

    private void ClearCornerNeighbors()
    {
        for (int x = xCorner - 1; x < xCorner + 2; x++)
        {
            for (int y = yCorner - 1; y < yCorner + 2; y++)
            {
                if (dungeonTable[x][y] == 1)
                {
                    dungeonTable[x][y] = 0;
                }
            }
        }
    }
}