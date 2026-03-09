using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class DoorJoiner
{
    private readonly List<BoundsInt2D> allRoomBounds;
    private readonly int roomsCount;
    private readonly int maxJoints;
    private readonly int[][] dungeonTable;
    private int[][] proximityTable;
    private int[][] jointsTable;
    private readonly List<List<Doors>> doorsTable = new();

    public DoorJoiner(List<BoundsInt2D> allRoomBounds, int maxJoints, int[][] dungeonTable)
    {
        this.allRoomBounds = allRoomBounds;
        this.roomsCount = allRoomBounds.Count;
        this.maxJoints = maxJoints;
        this.dungeonTable = dungeonTable;
    }

    public void JoinRooms()
    {
        GetProximityTable();
        int[][] proximityCopy = U.CopyTable(proximityTable);
        GetJointsTable();
        proximityTable = proximityCopy;

        for (int room = 0; room < roomsCount; room++)
        {
            GetJointsForRoom(room);
        }

        for (int x = 0; x < roomsCount; x++)
        {
            for (int y = 0; y < roomsCount; y++)
            {
                if (x < y && jointsTable[x][y] == 1)
                {
                    doorsTable[x][y].Join();
                }
            }
        }
    }

    private void GetProximityTable()
    {
        proximityTable = U.NewTable(roomsCount, roomsCount);

        for (int x = 0; x < roomsCount; x++)
        {
            doorsTable.Add(new List<Doors>());
            for (int y = 0; y < roomsCount; y++)
            {
                if (x < y)
                {
                    doorsTable[x].Add(new Doors(allRoomBounds[x], allRoomBounds[y], dungeonTable));
                    proximityTable[x][y] = doorsTable[x][y].GetDistance();
                }
                else
                {
                    proximityTable[x][y] = int.MaxValue;
                    doorsTable[x].Add(null);
                }
            }
        }
    }

    private void GetJointsTable()
    {
        jointsTable = U.NewTable(roomsCount, roomsCount);
        int allJoints = 0;
        int safetyBreak = 0;
        int safetyBreakLimit = roomsCount * roomsCount;
        int room0, room1;
        int[] closestRooms;
        int[] roomJoints = new int[roomsCount];

        while (allJoints < roomsCount - 1 && safetyBreak < safetyBreakLimit)
        {
            closestRooms = GetClosestRooms();
            room0 = closestRooms[0];
            room1 = closestRooms[1];
            if (room0 == room1)
            {
                Debug.LogError("proximityTable wypelnione maxValue");
                return;
            }

            if (roomJoints[room0] < maxJoints && roomJoints[room1] < maxJoints)
            {
                jointsTable[room0][room1] = 1;
                roomJoints[room0]++;
                roomJoints[room1]++;
                allJoints++;
            }
            proximityTable[room0][room1] = int.MaxValue;
            safetyBreak++;
        }
        if (safetyBreak == safetyBreakLimit)
        {
            Debug.LogError("safetyBreak w doorjoiner");
        }
    }

    private void GetJointsForRoom(int roomId)
    {
        int jointsCount = JointsCount(roomId);
        int closestRoomId, room0, room1;

        for (int i = jointsCount; i < maxJoints; i++)
        {
            closestRoomId = GetClosestRoom(roomId);
            if (closestRoomId == roomId)    //proximity table dla roomId ma samo maxvalue
            {
                return;
            }

            room0 = Math.Min(roomId, closestRoomId);
            room1 = Math.Max(roomId, closestRoomId);
            jointsTable[room0][room1] = 1;
            proximityTable[room0][room1] = int.MaxValue;
        }
    }

    private int JointsCount(int roomId)
    {
        int[] joints = GetAllJoints(roomId);
        int i = 0;
        for (; i < maxJoints; i++)
        {
            if (joints[i] == int.MaxValue)
            {
                break;
            }
        }
        return i;
    }

    private int GetClosestRoom(int roomId)
    {
        int closestRoomId = roomId;
        int closestDistance = int.MaxValue;
        int room0, room1;

        for (int i = 0; i < roomsCount; i++)
        {
            room0 = Math.Min(i, roomId);
            room1 = Math.Max(i, roomId);
            if (proximityTable[room0][room1] < closestDistance)
            {
                closestDistance = proximityTable[room0][room1];
                closestRoomId = i;
            }
        }
        return closestRoomId;
    }

    private int[] GetClosestRooms()
    {
        int[] closestRooms = new int[2];
        int closestDistance = int.MaxValue;

        for (int x = 0; x < roomsCount; x++)
        {
            for (int y = 0; y < roomsCount; y++)
            {
                if (proximityTable[x][y] < closestDistance && !WouldMakeLoop(x, y))
                {
                    closestDistance = proximityTable[x][y];
                    closestRooms[0] = x;
                    closestRooms[1] = y;
                }
            }

        }

        return closestRooms;
    }

    private int[] GetAllJoints(int roomId)
    {
        int[] joints = GetTableWithMax();
        int jointsCount = 0;

        for (int i = 0; i < roomId; i++)
        {
            if (jointsTable[i][roomId] == 1)
            {
                joints[jointsCount++] = i;
            }
        }
        for (int i = roomId + 1; i < roomsCount; i++)
        {
            if (jointsTable[roomId][i] == 1)
            {
                joints[jointsCount++] = i;
            }
        }

        return joints;
    }

    private bool HasLoop(int startingRoomId)
    {
        int[] rooms = GetTableWithMax();
        int[] previousRooms = GetTableWithMax();
        int[] joints;
        int jointsId, roomsFound = 1, roomId;
        rooms[0] = startingRoomId;

        for (int tableId = 0; tableId < roomsCount && tableId != roomsFound; tableId++)
        {
            joints = GetAllJoints(rooms[tableId]);      //ogladamy pokoj x i szukamy wszystkich sasiadow
            jointsId = 0;
            while (joints[jointsId] != int.MaxValue)
            {
                roomId = joints[jointsId];
                jointsId++;
                if (roomId != previousRooms[tableId])    //jesli sasiad nie jest pokojem z ktorego dostalismy sie do x
                {
                    if (U.Contains(rooms, roomId))       //jezeli bylismy juz w tym pokoju to zrobilismy petle
                    {
                        return true;
                    }
                    rooms[roomsFound] = roomId;
                    previousRooms[roomsFound] = rooms[tableId];
                    roomsFound++;
                }
            }
        }
        return false;
    }

    private bool WouldMakeLoop(int roomId1, int roomId2)
    {
        if (jointsTable[roomId1][roomId2] == 1)
        {
            Debug.LogError("wouldmakeloop");
        }
        jointsTable[roomId1][roomId2] = 1;
        bool hasLoop = HasLoop(roomId1);
        jointsTable[roomId1][roomId2] = 0;
        return hasLoop;
    }

    private int[] GetTableWithMax()
    {
        int[] table = new int[roomsCount];
        for (int i = 0; i < roomsCount; i++)
        {
            table[i] = int.MaxValue;
        }
        return table;
    }
}