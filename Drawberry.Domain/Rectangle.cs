namespace Drawberry.Domain;

public class Rectangle : IShape
{
    private int _maxPositionFromStartX, _maxPositionFromStartY;
    private List<Point> _pointsForShape;
    public int PointStartX { get; set; }
    public int PointStartY { get; set; }
    public int MaxPositionFromStartX
    {
        get => _maxPositionFromStartX; 
    }

    public int MaxPositionFromStartY
    {
        get => _maxPositionFromStartY; 
    }
    
    public int Width { get; set; }
    public int Height { get; set; }

    public List<Point> PointsForShape
    {
        get => _pointsForShape; 
    }
    public List<Point> PopulatePointsOfShape()
    {
        var pointsOfRect = new List<Point>(); 
        _maxPositionFromStartX = PointStartX + Width - 1;
        _maxPositionFromStartY = PointStartY + Height - 1;
        for (var x = PointStartX; x <= _maxPositionFromStartX; x++)
        {
            for (var y = PointStartY; y <= _maxPositionFromStartY; y++)
            {
                var point = new Point
                {
                    X = x, Y = y
                };
                pointsOfRect.Add(point);
            }
        }

        _pointsForShape = pointsOfRect; 
        return pointsOfRect; 
    }
}