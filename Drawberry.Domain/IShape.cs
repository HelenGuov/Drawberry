namespace Drawberry.Domain;

public interface IShape
{
    int PointStartX { get; set; }
    int PointStartY { get; set; }

    int MaxPositionFromStartX { get;}
    int MaxPositionFromStartY { get;}
    List<Point> PointsForShape { get; }
    //all points that is convered
    List<Point> PopulatePointsOfShape();
}