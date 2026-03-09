using Random = UnityEngine.Random;

public class BoundsInt2D
{
    public int x;
    public int y;
    public int xMax;
    public int yMax;
    public int xCenter;
    public int yCenter;

    public BoundsInt2D(int x, int y, int xMax, int yMax)
    {
        this.x = x;
        this.y = y;
        this.xMax = xMax;
        this.yMax = yMax;
        xCenter = (xMax - x) / 2 + x;
        yCenter = (yMax - y) / 2 + y;
    }

    public BoundsInt2D(int x, int y, int xMax, int yMax, int maxTrim, int minWidth, int minHeight)
    {
        this.x = x;
        this.y = y;
        this.xMax = xMax;
        this.yMax = yMax;
        Trim(maxTrim, minWidth, minHeight);
        xCenter = (xMax - x) / 2 + x;
        yCenter = (yMax - y) / 2 + y;
    }

    private void Trim(int maxTrim, int minWidth, int minHeight)
    {
        int xSize = xMax - x;
        int ySize = yMax - y;
        int maxTrimX = xSize - maxTrim < minWidth ? xSize - minWidth : maxTrim;
        int maxTrimY = ySize - maxTrim < minHeight ? ySize - minHeight : maxTrim;
        if (Random.Range(1, 3) == 1)
        {
            x += Random.Range(0, maxTrimX);
        }
        else
        {
            xMax -= Random.Range(0, maxTrimX);
        }
        if (Random.Range(1, 3) == 1)
        {
            y += Random.Range(0, maxTrimY);
        }
        else
        {
            yMax -= Random.Range(0, maxTrimY);
        }
    }
}