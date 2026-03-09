using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class RoomJoiner
{
    private readonly List<BoundsInt2D> allRoomBounds;
    private readonly int roomsCount;
    private readonly int maxJoints;
    private readonly int[][] dungeonTable;
    private int[][] proximityTable;
    private int[][] jointsTable;

    public RoomJoiner(List<BoundsInt2D> allRoomBounds, int maxJoints, int[][] dungeonTable)
    {
        this.allRoomBounds = allRoomBounds;
        this.roomsCount = allRoomBounds.Count;
        this.maxJoints = maxJoints;
        this.dungeonTable = dungeonTable;
    }

    public void JoinRooms()
    {
        GetProximityTable();
        jointsTable = U.NewTable(roomsCount, roomsCount);
        for (int roomId = 0; roomId < roomsCount; roomId++)
        {
            GetJointsForRoom(roomId);
        }

        for (int x = 0; x < roomsCount; x++)
        {
            for (int y = 0; y < roomsCount; y++)
            {
                if (jointsTable[x][y] == 1)
                {
                    Debug.Log(x + "   " + y);
                }
            }
        }

        for (int x = 0; x < roomsCount; x++)
        {
            for (int y = 0; y < roomsCount; y++)
            {
                if (jointsTable[x][y] == 1)
                {
                    Join(allRoomBounds[x].xCenter, allRoomBounds[x].yCenter, allRoomBounds[y].xCenter, allRoomBounds[y].yCenter);
                }
            }
        }
    }

    private void GetProximityTable()
    {
        proximityTable = U.NewTable(roomsCount, roomsCount);

        for (int x = 0; x < roomsCount; x++)
        {
            for (int y = 0; y < roomsCount; y++)
            {
                if (x >= y)
                {
                    proximityTable[x][y] = int.MaxValue;
                }
                else
                {
                    proximityTable[x][y] = Math.Abs(allRoomBounds[x].xCenter - allRoomBounds[y].xCenter) + Math.Abs(allRoomBounds[x].yCenter - allRoomBounds[y].yCenter);
                }
            }
        }
    }

    private void GetJointsForRoom(int roomId)
    {
        int jointsCount = JointsCount(roomId);
        int closestRoomId;
        int[] joinedRooms = new int[roomsCount];
        int joinedRoomsCount = 0;

        for (int i = jointsCount; i < maxJoints; i++)
        {
            closestRoomId = GetClosestRoom(roomId);
            if (closestRoomId == roomId)
            {
                return;
            }
            if (closestRoomId < roomId)
            {
                jointsTable[closestRoomId][roomId] = 1;
                proximityTable[closestRoomId][roomId] = int.MaxValue;
            }
            else
            {
                jointsTable[roomId][closestRoomId] = 1;
                proximityTable[roomId][closestRoomId] = int.MaxValue;
            }
            joinedRooms[joinedRoomsCount++] = closestRoomId;
        }

        for (int i = 0; i < joinedRoomsCount; i++)
        {
            GetJointsForRoom(joinedRooms[i]);
        }
    }

    private int JointsCount(int roomId)
    {
        int counter = 0;

        for (int i = 0; i < roomId; i++)
        {
            if (jointsTable[i][roomId] == 1)
            {
                counter++;
            }
        }
        for (int i = roomId + 1; i < roomsCount; i++)
        {
            if (jointsTable[roomId][i] == 1)
            {
                counter++;
            }
        }
        return counter;
    }

    private int GetClosestRoom(int roomId)
    {
        int closestRoomId = roomId;
        int closestDistance = int.MaxValue;

        for (int i = 0; i < roomId; i++)
        {
            if (proximityTable[i][roomId] < closestDistance)
            {
                closestDistance = proximityTable[i][roomId];
                closestRoomId = i;
            }
        }
        for (int i = roomId + 1; i < roomsCount; i++)
        {
            if (proximityTable[roomId][i] < closestDistance)
            {
                closestDistance = proximityTable[roomId][i];
                closestRoomId = i;
            }
        }
        return closestRoomId;
    }

    private void Join(int x1, int y1, int x2, int y2)
    {
        int xDiff = x2 - x1;
        int xSign = xDiff == 0 ? 0 : xDiff / Math.Abs(xDiff);
        int yDiff = y2 - y1;
        int ySign = yDiff == 0 ? 0 : yDiff / Math.Abs(yDiff);
        int xMean = (x1 - x2) / 2 + x2;
        int yMean = (y1 - y2) / 2 + y2;

        for (int x = x1; x != xMean; x += xSign)
        {
            dungeonTable[x][y1] = 1;
        }
        for (int y = y1; y != yMean; y += ySign)
        {
            dungeonTable[xMean][y] = 1;
        }
        for (int x = xMean; x != x2; x += xSign)
        {
            dungeonTable[x][yMean] = 1;
        }
        for (int y = yMean; y != y2; y += ySign)
        {
            dungeonTable[x2][y] = 1;
        }
    }
}