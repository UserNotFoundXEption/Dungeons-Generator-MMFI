using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManagerScript : MonoBehaviour
{
    public Texture2D cursor;
    public LayerMask enemyLayer;
    public Object player;
    public Transform playerMovePoint;
    public GameObject enemyChaser;
    public GameObject enemyMage;
    public GameObject enemyCoward;
    public GameObject treasure;
    public GameObject key;
    public int randomEnemiesCount;
    public int enemiesPerRoom;
    public int treasuresCount;
    public int keysCount;
    private int[][] dungeonTable;
    private int n;
    void Start()
    {
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.ForceSoftware);


        MapGenerator mapGenerator = FindObjectOfType<MapGenerator>();
        dungeonTable = mapGenerator.Generate();
        FindObjectOfType<GridMovement>().SetDungeonTable(dungeonTable);
        n = dungeonTable.Length;

        List<BoundsInt2D> allRoomBounds = mapGenerator.allRoomBounds;
        FindObjectOfType<OpenDoors>().Initiate(allRoomBounds);

        BoundsInt2D startingRoom = allRoomBounds[Random.Range(0, allRoomBounds.Count)];
        Vector3 playerPosition = FindOpenSpace(startingRoom);
        player.GetComponent<Transform>().position = playerPosition;
        playerMovePoint.position = playerPosition;
        playerMovePoint.parent = null;
        FindObjectOfType<CameraFollowPlayer>().Initiate();

        for (int i = 0; i < randomEnemiesCount; i++)
        {
            Spawn(RandomEnemy(), startingRoom, true);
        }
        for (int i = 0; i < treasuresCount; i++)
        {
            Spawn(treasure);
        }
        for (int i = 0; i < keysCount; i++)
        {
            Spawn(key);
        }
        foreach (BoundsInt2D roomBounds in allRoomBounds)
        {
            if (roomBounds != startingRoom)
            {
                for (int i = 0; i < enemiesPerRoom; i++)
                {
                    Spawn(RandomEnemy(), roomBounds);
                }
            }
        }
    }

    void Update()
    {
        if (!Physics2D.OverlapBox(new Vector2(n / 2, n / 2), new Vector2(n, n), 0, enemyLayer)
            && randomEnemiesCount + enemiesPerRoom > 0)
        {
            SceneManager.LoadScene("Win");
        }
    }

    private GameObject RandomEnemy()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                return enemyChaser;
            case 1:
                return enemyMage;
            case 2:
                return enemyCoward;
            default:
                break;
        }
        return null;
    }
    private void Spawn(GameObject entity)
    {
        Instantiate(entity, FindOpenSpace(), new Quaternion(0, 0, 0, 0));
    }

    private void Spawn(GameObject entity, BoundsInt2D roomBounds, bool reverseMask = false)
    {
        Instantiate(entity, FindOpenSpace(roomBounds, reverseMask), new Quaternion(0, 0, 0, 0));
    }

    private Vector3 FindOpenSpace()
    {
        int x, y;
        bool openSpace;
        do
        {
            x = Random.Range(0, n);
            y = Random.Range(0, n);
            openSpace = dungeonTable[x][y] == 1;
        } while (!openSpace);
        return new Vector3(x, y, 0);
    }

    private Vector3 FindOpenSpace(BoundsInt2D roomBounds, bool reverseMask = false)
    {
        int x, y;
        bool openSpace, inBounds, reverse, ok;
        do
        {
            if (reverseMask)
            {
                x = Random.Range(0, n);
                y = Random.Range(0, n);
            }
            else
            {
                x = Random.Range(roomBounds.x, roomBounds.xMax);
                y = Random.Range(roomBounds.y, roomBounds.yMax);
            }
            openSpace = dungeonTable[x][y] == 1;
            inBounds = U.InBounds(x, y, roomBounds);
            reverse = reverseMask && !inBounds;
            ok = !reverseMask || reverse;
        } while (!(openSpace && ok));
        return new Vector3(x, y, 0);
    }
}
