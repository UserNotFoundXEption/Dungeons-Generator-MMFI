using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class GridMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;
    public LayerMask obstacles;
    private int[][] dungeonTable;
    private bool dungeonGenerated = false;

    public void SetDungeonTable(int[][] dungeonTable)
    {
        this.dungeonTable = dungeonTable;
        dungeonGenerated = true;
    }

    void Update()
    {
        if (dungeonGenerated)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
            float distanceToMovePoint = Vector3.Distance(transform.position, movePoint.position);
            if (distanceToMovePoint < 0.05)
            {
                int horizontalInput = (int)Input.GetAxisRaw("Horizontal");
                int verticalInput = Math.Abs(horizontalInput) == 1 ? 0 : (int)Input.GetAxisRaw("Vertical");
                Vector3 moveVector = new(horizontalInput, verticalInput, 0f);
                bool movesIntoObstacle = Physics2D.OverlapCircle(movePoint.position + moveVector, .2f, obstacles);

                int x = (int)transform.position.x;
                int y = (int)transform.position.y;
                int newX = x + horizontalInput;
                int newY = y + verticalInput;
                bool opensDoorFromCorridor = dungeonTable[x][y] == 3 && dungeonTable[newX][newY] == 2;

                if (!movesIntoObstacle || opensDoorFromCorridor)
                {
                    movePoint.position += new Vector3(horizontalInput, verticalInput, 0f);
                    if (opensDoorFromCorridor)
                    {
                        FindObjectOfType<MapGenerator>().RemoveDoor(newX, newY);
                    }
                }
            }
        }
    }
}
