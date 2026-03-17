using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public int[][] dungeonTable;
    public int n;
    public int minHeight;
    public int minWidth;
    public int maxRooms;
    public int offset;
    public int maxJoints;
    public bool reshapeCut;
    public int chanceForHorizontal;
    public bool continueAfterIteration;
    public int cellFixIterations;
    public bool cellRounding;
    public int holesRadius;
    public Tilemap obstacles;
    public Tilemap visuals;
    public Tile wall;
    public Tile floor;
    public Tile door;
    public Tile corridor;
    public List<BoundsInt2D> allRoomBounds;



    // 0 - sciana, 1 - przestrzen, 2 - drzwi, 3 - korytarz
    public int[][] Generate()
    {
        UnityEngine.Random.InitState(69);
        dungeonTable = U.NewTable(n, n);
        MakeDungeon();
        return dungeonTable;
    }

    public void RemoveDoor(int x, int y)
    {
        dungeonTable[x][y] = 1;
        obstacles.SetTile(new Vector3Int(x, y), null);
        visuals.SetTile(new Vector3Int(x, y), floor);
    }

    private void MakeDungeon()
    {
        BinarySpacePartitioning bsp = new();
        allRoomBounds = bsp.GetRooms(new BoundsInt2D(1, 1, n - 1, n - 1), minWidth, minHeight, maxRooms);
        DoorJoiner doorJoiner = new(allRoomBounds, maxJoints, dungeonTable);

        foreach (BoundsInt2D roomBounds in allRoomBounds)
        {
            if (roomBounds.x == roomBounds.xMax)
            {
                break;
            }
            Room room = new(roomBounds, offset);
            int[][] roomTable = room.Create(reshapeCut, chanceForHorizontal, continueAfterIteration, cellFixIterations, cellRounding, holesRadius);
            for (int x = 0; x < room.xSize; x++)
            {
                for (int y = 0; y < room.ySize; y++)
                {
                    if (roomTable[x][y] == 1)
                    {
                        dungeonTable[x + roomBounds.x][y + roomBounds.y] = 1;
                    }
                }
            }
        }

        doorJoiner.JoinRooms();
        FillTileMap();
    }

    private void FillTileMap()
    {
        Tile chosenTile = wall;
        Tilemap tilemap;
        for (int y = 0; y <= n - 1; y++)
        {
            for (int x = 0; x <= n - 1; x++)
            {
                switch (dungeonTable[x][y])
                {
                    case 0: chosenTile = wall; break;
                    case 1: chosenTile = floor; break;
                    case 2: chosenTile = door; break;
                    case 3: chosenTile = corridor; break;
                }

                if (dungeonTable[x][y] == 0 || dungeonTable[x][y] == 2)
                {
                    tilemap = obstacles;
                }
                else
                {
                    tilemap = visuals;
                }
                tilemap.SetTile(new Vector3Int(x, y), chosenTile);
            }
        }
    }

    public void OpenRoom(BoundsInt2D roomBounds)
    {
        for (int x = roomBounds.x; x < roomBounds.xMax; x++)
        {
            for (int y = roomBounds.y; y < roomBounds.yMax; y++)
            {
                if (dungeonTable[x][y] == 2)
                {
                    RemoveDoor(x, y);
                }
            }
        }
    }
}
