using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class EnemyMage : Enemy
{
    public Transform orbital;
    public float teleportInterval = 2;
    public int teleportRange = 5;
    private float currentTeleportTimer = 0;

    new void Start()
    {
        Initiate();
        movePoint.GetComponent<Rigidbody2D>().AddTorque(100);
    }

    void Update()
    {
        orbital.RotateAround(transform.position, Vector3.forward, 100 * Time.deltaTime);
        currentTeleportTimer -= Time.deltaTime;
        if (currentTeleportTimer < 0)
        {
            currentTeleportTimer = teleportInterval;
            transform.position = movePoint.position;
            float distance = DistanceToPlayer();
            if (distance < viewRange && PlayerInOpenSpace())
            {
                movePoint.position = GetChasePosition();
            }
            else
            {
                if (Random.Range(0, 2) == 0)
                {
                    movePoint.position = GetRandomPosition();
                }
            }
        }
        float teleportSize = (1 - currentTeleportTimer / teleportInterval) / 2;
        movePoint.localScale = new Vector3(teleportSize, teleportSize, 1);
    }

    private Vector2 GetRandomPosition()
    {
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;
        int newX, newY;
        do
        {
            newX = Random.Range(x - teleportRange, x + teleportRange + 1);
            newY = Random.Range(y - teleportRange, y + teleportRange + 1);
        } while (!(U.InDungeon(newX, newY, dungeonTable.Length) && dungeonTable[newX][newY] == 1));
        return new Vector2(newX, newY);
    }

    private Vector2 GetChasePosition()
    {
        int eX = (int)transform.position.x;
        int eY = (int)transform.position.y;
        int pX = (int)playerMovePoint.position.x;
        int pY = (int)playerMovePoint.position.y;
        int r = 1;
        int triesLeftToIncreaseR = 10;
        if (Math.Abs(pX - eX) > teleportRange)
        {
            pX = eX + (teleportRange - r) * Math.Sign(pX - eX);
        }
        if (Math.Abs(pY - eY) > teleportRange)
        {
            pY = eY + (teleportRange - r) * Math.Sign(pY - eY);
        }

        int newX, newY;
        do
        {
            newX = Random.Range(pX - r, pX + r + 1);
            newY = Random.Range(pY - r, pY + r + 1);
            if (--triesLeftToIncreaseR == 0)
            {
                triesLeftToIncreaseR = 10;
                r++;
            }
        } while (!(U.InDungeon(newX, newY, dungeonTable.Length) && dungeonTable[newX][newY] == 1));
        return new Vector2(newX, newY);
    }
}