#pragma warning disable
using System.Collections.Generic;
using System.Linq;

public struct Index
{
    public int Row;
    public int Column;

    public Index(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public Index(Index index)
    {
        Row = index.Row;
        Column = index.Column;
    }

    public static bool operator == (Index a, Index b)
    {
        return (a.Row == b.Row) && (a.Column == b.Column);
    }

    public static bool operator !=(Index a, Index b)
    {
        return (a.Row != b.Row) || (a.Column != b.Column);
    }
}

public static class IndexExtentions
{
    public static Index Left(this Index x) => new Index(x.Row - 1, x.Column);
    public static Index Right(this Index x) => new Index(x.Row + 1, x.Column);
    public static Index Upper(this Index x) => new Index(x.Row, x.Column + 1);
    public static Index Lower(this Index x) => new Index(x.Row, x.Column - 1);
    public static Index UpperRight(this Index x) => new Index(x.Row + 1, x.Column + 1);
    public static Index UpperLeft(this Index x) => new Index(x.Row - 1, x.Column + 1);
    public static Index LowerRight(this Index x) => new Index(x.Row + 1, x.Column - 1);
    public static Index LowerLeft(this Index x) => new Index(x.Row - 1, x.Column - 1);

    public static Index IndexBySide(this Index currentNodeIndex, Side side) => side switch
    {
        Side.Left => currentNodeIndex.Left(),
        Side.Right => currentNodeIndex.Right(),
        Side.Upper => currentNodeIndex.Upper(),
        Side.Lower => currentNodeIndex.Lower(),
        Side.UpperLeft => currentNodeIndex.UpperLeft(),
        Side.UpperRight => currentNodeIndex.UpperRight(),
        Side.LowerLeft => currentNodeIndex.LowerLeft(),
        Side.LowerRight => currentNodeIndex.LowerRight()
    };

    public static Side? SideByIndex(this Index currentNodeIndex, Index neigboringIndex)
    {
        foreach (var side in System.Enum.GetValues(typeof(Side)).Cast<Side>())
        {
            if (currentNodeIndex.IndexBySide(side) == neigboringIndex)
                return side;
        }

        return null;
    }
}