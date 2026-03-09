using UnityEngine;
public abstract class EnemyWalker : Enemy
{
    public float moveSpeed = 3f;
    private float currentIdleTime = 0;
    protected IEnemyStatus enemyStatus;
    protected PathFinding pathFinding;
    protected EnemyStatusAggressive enemyStatusAggressive;
    protected EnemyStatusIdle enemyStatusIdle;
    protected EnemyStatusWandering enemyStatusWandering;

    public new void Start()
    {
        Initiate();
        pathFinding = new(viewRange, dungeonTable);
        enemyStatusAggressive = new();
        enemyStatusIdle = new();
        enemyStatusWandering = new();
    }

    protected void Walk(bool chaser)
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        if (transform.position == movePoint.position)
        {
            int direction = 0;
            float eX = transform.position.x;
            float eY = transform.position.y;
            float pX = playerMovePoint.position.x;
            float pY = playerMovePoint.position.y;
            float distance = DistanceToPlayer();
            if (distance <= viewRange && PlayerInOpenSpace())
            {
                if (enemyStatus is not EnemyStatusAggressive)
                {
                    enemyStatus = enemyStatusAggressive;
                }
                direction = pathFinding.Direction((int)eX, (int)eY, (int)pX, (int)pY, chaser);
            }
            else
            {
                pathFinding.ClearDirections();
                if (enemyStatus is EnemyStatusIdle)
                {
                    currentIdleTime -= Time.deltaTime;
                    if (currentIdleTime < 0)
                    {
                        currentIdleTime = idleTime;
                        enemyStatus = enemyStatusWandering;
                    }
                }
                else
                {
                    if (Random.Range(0, 3) == 0)
                    {
                        enemyStatus = enemyStatusIdle;
                    }
                    else
                    {
                        if (enemyStatus is not EnemyStatusWandering)
                        {
                            enemyStatus = enemyStatusWandering;
                        }
                    }
                }
            }
            enemyStatus.Move(movePoint, direction, dungeonTable);
        }
    }
    protected interface IEnemyStatus
    {
        public void Move(Transform movePoint, int direction, int[][] t);
    }


    protected class EnemyStatusIdle : IEnemyStatus
    {
        public void Move(Transform movePoint, int direction, int[][] t)
        {
            return;
        }
    }


    protected class EnemyStatusAggressive : IEnemyStatus
    {
        public void Move(Transform movePoint, int direction, int[][] t)
        {
            int x = (int)movePoint.position.x;
            int y = (int)movePoint.position.y;
            int newx = U.NX(x, direction);
            int newy = U.NY(y, direction);

            movePoint.position = new Vector3(newx, newy, 0);
        }
    }

    protected class EnemyStatusWandering : IEnemyStatus
    {
        private int currentWalkDistance = 0;
        private int currentWalkPointer = 0;
        private static readonly int maxWalkDistance = 8;
        private int direction = 0;

        public void Move(Transform movePoint, int ignored, int[][] dungeonTable)
        {
            int x = (int)movePoint.position.x;
            int y = (int)movePoint.position.y;
            int newX = U.NX(x, direction);
            int newY = U.NY(y, direction);
            while (direction == 0 || currentWalkPointer == currentWalkDistance || dungeonTable[newX][newY] != 1)
            {
                direction = Random.Range(1, 5);
                currentWalkDistance = Random.Range(2, maxWalkDistance + 1);
                currentWalkPointer = 0;
                newX = U.NX(x, direction);
                newY = U.NY(y, direction);
            }
            currentWalkPointer++;
            movePoint.position = new Vector3(newX, newY, 0);
        }
    }

}