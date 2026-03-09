using System.Collections.Generic;

public class BinarySpacePartitioning
{
    public List<BoundsInt2D> GetRooms(BoundsInt2D firstRoom, int minWidth, int minHeight, int maxRooms)
    {
        int skippedRooms = 0;
        List<BoundsInt2D> rooms = new() {
            firstRoom
        };

        for (int i = 1; i < maxRooms; i++)
        {
            BoundsInt2D room = rooms[skippedRooms];
            int minSeparationX = room.x + minWidth;
            int maxSeparationX = room.xMax - minWidth;
            int minSeparationY = room.y + minHeight;
            int maxSeparationY = room.yMax - minHeight;
            bool canSeperateX = maxSeparationX >= minSeparationX;
            bool canSeperateY = maxSeparationY >= minSeparationY;
            if (!canSeperateX && !canSeperateY)
            {
                if (++skippedRooms == rooms.Count)
                {
                    break;
                }
                i--;
                continue;
            }

            BoundsInt2D newRoom1, newRoom2;
            if (canSeperateX && (!canSeperateY || UnityEngine.Random.Range(0, 2) == 0))
            {
                int separationX = UnityEngine.Random.Range(minSeparationX, maxSeparationX + 1);
                newRoom1 = new BoundsInt2D(room.x, room.y, separationX, room.yMax, 3, minWidth, minHeight);
                newRoom2 = new BoundsInt2D(separationX, room.y, room.xMax, room.yMax, 3, minWidth, minHeight);
            }
            else
            {
                int separationY = UnityEngine.Random.Range(minSeparationY, maxSeparationY + 1);
                newRoom1 = new BoundsInt2D(room.x, room.y, room.xMax, separationY, 3, minWidth, minHeight);
                newRoom2 = new BoundsInt2D(room.x, separationY, room.xMax, room.yMax, 3, minWidth, minHeight);
            }


            rooms.Remove(room);
            rooms.Add(newRoom1);
            rooms.Add(newRoom2);
        }

        return rooms;
    }
}