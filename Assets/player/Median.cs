using System.Collections.Generic;

public class Median
{
    private float _sum = 0;
    private float _count = 0;

    public void Add(float val)
    {
        _sum += val;
        _count++;
    }
    public float GetMedian()
    {
        if (_count == 0)
        { return 0; }
        return _sum / _count;
    }
}

