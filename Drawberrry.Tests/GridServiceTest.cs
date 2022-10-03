using System.Linq;
using Drawberrry.Service;
using Drawberry.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace Drawberrry.Tests;

public class GridServiceTest
{
    [Test]
    public void GivenRectValuesWhenCreateRectangleThenReturnsCreatedRectangle()
    {
        var rectangle = new Rectangle();
        rectangle.Height = 2;
        rectangle.Width = 3;
        rectangle.PointStartX = 1;
        rectangle.PointStartY = 2;

        var rectPoints = rectangle.PopulatePointsOfShape();
        var stringPoint = "";
        rectangle.PointsForShape.ForEach(x => stringPoint += x.X + "," + x.Y + "|");
        stringPoint.Should().Be("1,2|1,3|2,2|2,3|3,2|3,3|");

        rectPoints.Should().HaveCount(6);
        rectangle.MaxPositionFromStartX.Should().Be(3);
        rectangle.MaxPositionFromStartY.Should().Be(3);
    }

    [TestCase(0, 0)]
    [TestCase(-2, 5)]
    [TestCase(-2, 0)]
    [TestCase(-2, -3)]
    [TestCase(2, 30)]
    public void GivenGridHeightWidthLessThanMinMax_WhenInitialiseGrid_ThenReturnsEmptyGrid(int w, int h)
    {
        var gridService = new GridService();
        var grid = gridService.InitialiseGrid(w, h);
        grid.Height.Should().Be(0);
        grid.Width.Should().Be(0);
        grid.Shapes.Should().BeNull();
        grid.GridOccupancy.Should().BeNull();
    }

    [TestCase(0, 0, 26, 26, 20, 20)]
    [TestCase(20, 20, 10, 5, 25, 25)]
    [TestCase(20, 20, 5, 10, 25, 25)]
    [TestCase(20, 20, 10, 10, 25, 25)]
    public void GivenRectOutsideOfGrid_WhenPlaceARectangle_ThenNothingIsPlaceOnGrid(
        int startX, int startY, int rectWidth, int rectHeight, int gridWidth, int gridHeight)
    {
        var gridService = new GridService();
        var grid = gridService.InitialiseGrid(rectWidth, rectHeight);
        var rectangle = new Rectangle();
        rectangle.Height = rectHeight;
        rectangle.Width = rectWidth;
        rectangle.PointStartX = startX;
        rectangle.PointStartY = startY;

        var isAdded = gridService.PlaceShapeOnGrid(rectangle, grid);
        isAdded.Should().BeFalse();
    }

    [TestCase(5, -5)]
    [TestCase(-5, 5)]
    [TestCase(-5, -5)]
    public void GivenNegativeRectPositions_WhenPlaceShapeOnGrid_ThenNothingIsPlaced(int x, int y)
    {
        var gridService = new GridService();
        var grid = gridService.InitialiseGrid(15, 15);
        var rectangle = new Rectangle();
        rectangle.Height = 10;
        rectangle.Width = 5;
        rectangle.PointStartX = x;
        rectangle.PointStartY = y;

        var isAdded = gridService.PlaceShapeOnGrid(rectangle, grid);
        isAdded.Should().BeFalse();
    }

    [TestCase(5, 10, 4, 9)]
    [TestCase(15, 15, 2, 3)]
    public void GivenOverlapWithExistingShape_WhenPlaceShapeOnGrid_ThenNothingIsPlace(int gridW, int gridH,
        int overlapX, int overlapY)
    {
        var gridService = new GridService();
        var grid = gridService.InitialiseGrid(15, 15);
        var rectangle = new Rectangle();
        rectangle.Height = gridH;
        rectangle.Width = gridW;
        rectangle.PointStartX = 0;
        rectangle.PointStartY = 0;

        var isAdded = gridService.PlaceShapeOnGrid(rectangle, grid);
        isAdded.Should().BeTrue();
        rectangle.PointsForShape.Should().HaveCount(rectangle.Width * rectangle.Height);

        var rectangle2 = new Rectangle();
        rectangle2.Height = 5;
        rectangle2.Width = 5;
        rectangle2.PointStartX = overlapX;
        rectangle2.PointStartY = overlapY;

        var isAddedShape2 = gridService.PlaceShapeOnGrid(rectangle2, grid);
        isAddedShape2.Should().BeFalse();
        grid.Shapes.Should().HaveCount(1);
    }

    [Test]
    public void GivenRectangles_WhenPlaceOnGrid_ThenReturnsRectanglesArePlace()
    {
        var gridService = new GridService();
        var grid = gridService.InitialiseGrid(15, 15);
        var rectangle = new Rectangle();
        rectangle.Height = 4;
        rectangle.Width = 3;
        rectangle.PointStartX = 0;
        rectangle.PointStartY = 0;

        var isAdded = gridService.PlaceShapeOnGrid(rectangle, grid);
        isAdded.Should().BeTrue();
        rectangle.PointsForShape.Should().HaveCount(rectangle.Width * rectangle.Height);

        var rectangle2 = new Rectangle();
        rectangle2.Height = 5;
        rectangle2.Width = 5;
        rectangle2.PointStartX = 4;
        rectangle2.PointStartY = 3;

        var isAddedShape2 = gridService.PlaceShapeOnGrid(rectangle2, grid);
        isAddedShape2.Should().BeTrue();
        rectangle2.PointsForShape.Should().HaveCount(rectangle2.Height * rectangle2.Width);
        grid.Shapes.Should().HaveCount(2);
        grid.Shapes[0].Should().BeEquivalentTo(rectangle);
        grid.Shapes[1].Should().BeEquivalentTo(rectangle2); 
    }

    [Test]
    public void GiveARectanglePositionOnGrid_WhenRemovingRect_ThenReturnsRemainingRectOnGrid()
    {
        var gridService = new GridService();
        var grid = gridService.InitialiseGrid(15, 15);
        var rectangle = new Rectangle();
        rectangle.Height = 4;
        rectangle.Width = 3;
        rectangle.PointStartX = 0;
        rectangle.PointStartY = 0;

        var isAdded = gridService.PlaceShapeOnGrid(rectangle, grid);
        isAdded.Should().BeTrue();
        rectangle.PointsForShape.Should().HaveCount(rectangle.Width * rectangle.Height);

        var rectangle2 = new Rectangle();
        rectangle2.Height = 5;
        rectangle2.Width = 5;
        rectangle2.PointStartX = 4;
        rectangle2.PointStartY = 3;

        var isAddedShape2 = gridService.PlaceShapeOnGrid(rectangle2, grid);
        isAddedShape2.Should().BeTrue();
        rectangle2.PointsForShape.Should().HaveCount(rectangle2.Height * rectangle2.Width);
        grid.Shapes.Should().HaveCount(2);

        var isRemoved = gridService.RemoveShapeFromGrid(grid, 4, 5);
        isRemoved.Should().BeTrue();
        grid.Shapes.Should().HaveCount(1);
        rectangle.Should().BeEquivalentTo(grid.Shapes[0]); 
    }

    [Test]
    public void GivenValidRectPositionOnGrid_WhenFindRect_ThenReturnsRectangle()
    {
        var gridService = new GridService();
        var grid = gridService.InitialiseGrid(15, 15);
        var rectangle = new Rectangle();
        rectangle.Height = 4;
        rectangle.Width = 3;
        rectangle.PointStartX = 0;
        rectangle.PointStartY = 0;

        var isAdded = gridService.PlaceShapeOnGrid(rectangle, grid);
        isAdded.Should().BeTrue();
        rectangle.PointsForShape.Should().HaveCount(rectangle.Width * rectangle.Height);

        var foundShape = gridService.FindShapeOnGrid(grid, 2, 2);
        foundShape.Should().BeEquivalentTo(rectangle); 
    }
    
    [Test]
    public void GivenInValidRectPositionOnGrid_WhenFindRect_ThenReturnsRectangle()
    {
        var gridService = new GridService();
        var grid = gridService.InitialiseGrid(15, 15);
        var rectangle = new Rectangle();
        rectangle.Height = 4;
        rectangle.Width = 3;
        rectangle.PointStartX = 0;
        rectangle.PointStartY = 0;

        var isAdded = gridService.PlaceShapeOnGrid(rectangle, grid);
        isAdded.Should().BeTrue();
        rectangle.PointsForShape.Should().HaveCount(rectangle.Width * rectangle.Height);

        var foundShape = gridService.FindShapeOnGrid(grid, 10, 2);
        foundShape.Should().BeNull(); 
    }
}