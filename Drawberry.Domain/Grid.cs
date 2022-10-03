namespace Drawberry.Domain;

public class Grid
{
    public int Width { get; set; }
    public int Height { get; set; }
    public Dictionary<string, bool> GridOccupancy { get; set; }
    public List<IShape> Shapes { get; set; }
}