using System.Diagnostics.SymbolStore;
using System.Reflection.Metadata;
using Drawberry.Domain;

namespace Drawberrry.Service;

public class GridService
{
    public Grid InitialiseGrid(int width, int height)
    {
        var grid = new Grid(); 
        if (width < Constants.MinGridLength || height > Constants.MaxGridLength)
        {
            Console.WriteLine($"Grid width/height is out of range");
            return grid; 
        }

        grid.Height = height;
        grid.Width = width;
        grid.GridOccupancy = new Dictionary<string, bool>();
        grid.Shapes = new List<IShape>(); 
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var xy = ToPointString(x, y); 
                if (!grid.GridOccupancy.ContainsKey(xy))
                {
                    grid.GridOccupancy[xy] = false; 
                }
            }
        }

        return grid; 
    }

    public bool PlaceShapeOnGrid(IShape shape, Grid grid)
    {
        var pointsOfShape = shape.PopulatePointsOfShape(); 
        var isValidPlacement = IsValidPlacement(grid, shape);
        if (!isValidPlacement) return false; 

        var pointsForShape = shape.PopulatePointsOfShape();
        foreach (var point in pointsForShape)
        {
            var gridPoint = ToPointString(point.X, point.Y);
            grid.GridOccupancy[gridPoint] = true;
        }

        grid.Shapes.Add(shape);
        return true; 
    }

    private string ToPointString(int x, int y) => x + "," + y; 
    
    private bool IsValidPlacement(Grid grid, IShape shape)
    {
        var isNegativeStartXY =  shape.PointStartX < 0 || shape.PointStartY < 0 ? true : false;
        if (isNegativeStartXY) return false; 
        
        var isShapeOutsideOfGrid = shape.MaxPositionFromStartX > grid.Width || shape.MaxPositionFromStartY > grid.Height
            ? true
            : false;
        if (isShapeOutsideOfGrid) return false;
        
        var isOverlap = IsOverlap(grid.GridOccupancy, ToPointString(shape.PointStartX, shape.PointStartY));
        if (isOverlap) return false;

        return true; 
    }
    
    private bool IsOverlap(Dictionary<string, bool> gridOccupancy, string pointOnGrid)
    {
        var isOverlap = false;
        if (gridOccupancy.ContainsKey(pointOnGrid))
        {
            isOverlap = gridOccupancy[pointOnGrid]; 
        }
        if (isOverlap) Console.WriteLine("overlap");
        return isOverlap; 
    }

    public bool RemoveShapeFromGrid(Grid grid, int positionX, int positionY)
    {
        var shapeToRemove = grid.Shapes.FirstOrDefault(s => s.PointsForShape.Any(p => p.X == positionX && p.Y == positionY));
        if (shapeToRemove == null) return false;
        return grid.Shapes.Remove(shapeToRemove); 
    }

    public IShape FindShapeOnGrid(Grid grid, int positionX, int positionY)
    {
        var pointOnGrid = ToPointString(positionX, positionY);
        if (grid.GridOccupancy.ContainsKey(pointOnGrid) && grid.GridOccupancy[pointOnGrid])
        {
            return grid.Shapes.FirstOrDefault(s => s.PointsForShape.Any(p => p.X == positionX && p.Y == positionY));
        }

        return null; 
    }
}