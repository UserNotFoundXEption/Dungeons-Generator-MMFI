using System.Collections.Generic;
using UnityEngine;

public class OpenDoors : MonoBehaviour
{
    public LayerMask enemyLayer;
    private MapGenerator mapGenerator;
    private bool dungeonIsGenerated = false;
    private List<BoundsInt2D> allRoomBounds;
    void Update()
    {
        if (dungeonIsGenerated)
        {
            Vector2 center, size;
            BoundsInt2D openedRoom = null;
            foreach (BoundsInt2D roomBounds in allRoomBounds)
            {
                center = new(roomBounds.xCenter, roomBounds.yCenter);
                size = new(roomBounds.xMax - roomBounds.x, roomBounds.yMax - roomBounds.y);
                if (!Physics2D.OverlapBox(center, size, 0, enemyLayer))
                {
                    mapGenerator.OpenRoom(roomBounds);
                    openedRoom = roomBounds;
                }
            }
            allRoomBounds.Remove(openedRoom);
        }
    }

    public void Initiate(List<BoundsInt2D> allRoomBounds)
    {
        this.allRoomBounds = allRoomBounds;
        mapGenerator = FindAnyObjectByType<MapGenerator>();
        dungeonIsGenerated = true;
    }
}
