using System.Collections.Generic;
using System;
public class PathFinding
{
    private readonly int viewRange;
    private int walkRange;
    private int[] directions;
    private int currentDirectionNo;
    private int maxDirectionNo;
    private readonly int[][] dungeonTable;
    public PathFinding(int viewRange, int[][] dungeonTable)
    {
        this.viewRange = viewRange;
        this.directions = new int[walkRange];
        this.currentDirectionNo = 1;
        this.maxDirectionNo = 0;
        this.dungeonTable = dungeonTable;
    }

    public int Direction(int eX, int eY, int pX, int pY, bool closest)
    {
        walkRange = viewRange;
        if (currentDirectionNo > maxDirectionNo)
        {
            FindNewDirections(eX, eY, pX, pY, closest);
        }
        return directions[currentDirectionNo++];
    }

    private void FindNewDirections(int eX, int eY, int pX, int pY, bool closest)
    {
        List<Path> paths = InitializeSearch(eX, eY);
        int range = 1;
        for (; range < walkRange; range++)
        {
            foreach (Path path in paths)
            {
                if (path.x == pX && path.y == pY && closest)
                {
                    ChooseThisPath(path, range);
                    return;
                }
            }
            paths = ExtendSearch(paths, range);
        }
        if (closest)
        {
            ChooseThisPath(FindClosestPath(paths, pX, pY), range);
        }
        else
        {
            ChooseThisPath(FindFurthestPath(paths, pX, pY), 6);
        }
    }

    private List<Path> InitializeSearch(int eX, int eY)
    {
        List<Path> paths = new();
        int newX, newY;
        for (int direction = 1; direction <= 4; direction++)
        {
            newX = U.NX(eX, direction);
            newY = U.NY(eY, direction);
            if (dungeonTable[newX][newY] == 1)
            {
                paths.Add(new Path(newX, newY, direction, walkRange));
            }
        }

        return paths;
    }

    private List<Path> ExtendSearch(List<Path> oldPaths, int range)
    {
        List<Path> newPaths = new();
        int newX, newY;

        foreach (Path path in oldPaths)
        {
            for (int direction = 1; direction <= 4; direction++)
            {
                newX = U.NX(path.x, direction);
                newY = U.NY(path.y, direction);
                if (dungeonTable[newX][newY] == 1)
                {
                    if (PathIsNotDuplicate(newPaths, newX, newY))
                    {
                        newPaths.Add(new Path(newX, newY, path.directions, direction, walkRange, range));
                    }
                }
            }
        }

        return newPaths;
    }

    private Path FindClosestPath(List<Path> paths, float pX, float pY)
    {
        float minDistance = 21372137f;
        float distance;
        Path bestPath = null;

        foreach (Path path in paths)
        {
            distance = Math.Abs(path.x - pX) + Math.Abs(path.y - pY);
            if (distance < minDistance)
            {
                minDistance = distance;
                bestPath = path;
            }
        }

        return bestPath;
    }

    private Path FindFurthestPath(List<Path> paths, float pX, float pY)
    {
        float maxDistance = 0;
        float distanceToPlayer;
        Path bestPath = null;

        foreach (Path path in paths)
        {
            distanceToPlayer = Math.Abs(path.x - pX) + Math.Abs(path.y - pY);
            if (distanceToPlayer > maxDistance)
            {
                maxDistance = distanceToPlayer;
                bestPath = path;
            }
        }

        return bestPath;
    }

    private void ChooseThisPath(Path path, int range)
    {
        directions = path.directions;
        currentDirectionNo = 0;
        maxDirectionNo = range / 3;
    }

    public void ClearDirections()
    {
        currentDirectionNo = 2137;
    }

    private bool PathIsNotDuplicate(List<Path> paths, int x, int y)
    {
        foreach (Path path in paths)
        {
            if (path.x == x && path.y == y)
            {
                return false;
            }
        }
        return true;
    }
}


public class Path
{
    public int x;
    public int y;

    public int[] directions;

    public Path(int x, int y, int direction, int walkRange)
    {
        this.x = x;
        this.y = y;
        this.directions = new int[1];
        this.directions[0] = direction;
    }

    public Path(int x, int y, int[] directions, int direction, int walkRange, int range)
    {
        this.x = x;
        this.y = y;
        this.directions = new int[range + 1];
        for (int i = 0; i < range; i++)
        {
            this.directions[i] = directions[i];
        }
        this.directions[range] = direction;
    }
}

