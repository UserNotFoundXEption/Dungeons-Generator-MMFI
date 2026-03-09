public class RandomWalk
{
    readonly int[][] table;
    readonly int xSize;
    readonly int ySize;
    readonly System.Random random;

    public RandomWalk(int[][] table)
    {
        this.table = table;
        this.xSize = table.Length;
        this.ySize = table[0].Length;
        random = new System.Random();
    }

    public void SendWalker(int x, int y, int walkDistance, int iterations, int chanceForHorizontal, bool continueAfterIteration)
    {
        if (iterations == 0)
        {
            return;
        }

        int newX = x;
        int newY = y;
        int direction;
        for (int i = 0; i < walkDistance; i++)
        {
            if (random.Next(0, 100) < chanceForHorizontal)
            {
                direction = random.Next(1, 3) * 2;
            }
            else
            {
                direction = random.Next(1, 3) * 2 - 1;
            }

            newX = U.NX(newX, direction, 1);
            newY = U.NY(newY, direction, 1);
            if (newX < 1 || newY < 1 || newX >= xSize - 1 || newY >= ySize - 1)
                return;
            table[newX][newY] = 1;
        }

        if (!continueAfterIteration)
        {
            newX = x;
            newY = y;
        }
        SendWalker(newX, newY, walkDistance, iterations - 1, chanceForHorizontal, continueAfterIteration);
    }
}