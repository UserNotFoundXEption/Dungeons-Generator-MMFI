using UnityEngine;
using System;

public abstract class Enemy : MonoBehaviour
{
    public Transform movePoint;
    public float idleTime = 2;
    public int viewRange = 10;
    protected Transform playerMovePoint;
    protected int[][] dungeonTable;

    public void Start()
    {
        Initiate();
    }
    public void Kill()
    {
        Destroy(movePoint.gameObject);
        Destroy(gameObject);
    }

    protected void Initiate()
    {
        playerMovePoint = GameObject.Find("PlayerMovePoint").GetComponent<Transform>();
        movePoint.parent = null;
        movePoint.position = transform.position;
        dungeonTable = FindObjectOfType<MapGenerator>().dungeonTable;
    }

    protected float DistanceToPlayer()
    {
        float eX = transform.position.x;
        float eY = transform.position.y;
        float pX = playerMovePoint.position.x;
        float pY = playerMovePoint.position.y;
        float distance = Math.Abs(eX - pX) + Math.Abs(eY - pY);
        return distance;
    }

    protected bool PlayerInOpenSpace()
    {
        int pX = (int)playerMovePoint.position.x;
        int pY = (int)playerMovePoint.position.y;
        return dungeonTable[pX][pY] == 1;
    }
}

