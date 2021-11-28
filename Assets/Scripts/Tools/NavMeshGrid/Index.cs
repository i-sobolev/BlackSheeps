#pragma warning disable
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

    public static Index NewIndexBySide(NeighboringNodeSide side, Index currentNodeIndex) => side switch
    {
        NeighboringNodeSide.Left => currentNodeIndex.Left(),
        NeighboringNodeSide.Right => currentNodeIndex.Right(),
        NeighboringNodeSide.Upper => currentNodeIndex.Upper(),
        NeighboringNodeSide.Lower => currentNodeIndex.Lower(),
        NeighboringNodeSide.UpperLeft => currentNodeIndex.UpperLeft(),
        NeighboringNodeSide.UpperRight => currentNodeIndex.UpperRight(),
        NeighboringNodeSide.LowerLeft => currentNodeIndex.LowerLeft(),
        NeighboringNodeSide.LowerRight => currentNodeIndex.LowerRight(),
        _ => new Index()
    };

    //public static NeighboringNodeSide SideByIndex(Index newNodeIndex, Index currentNodeIndex) => newNodeIndex switch
    //{

    //};
}

public static class IndexExtentions
{
    public static Index Left(this Index x) => new Index(x.Column, x.Row - 1);
    public static Index Right(this Index x) => new Index(x.Column, x.Row + 1);
    public static Index Upper(this Index x) => new Index(x.Column + 1, x.Row);
    public static Index Lower(this Index x) => new Index(x.Column - 1, x.Row);
    public static Index UpperRight(this Index x) => new Index(x.Column + 1, x.Row + 1);
    public static Index UpperLeft(this Index x) => new Index(x.Column + 1, x.Row - 1);
    public static Index LowerRight(this Index x) => new Index(x.Column - 1, x.Row + 1);
    public static Index LowerLeft(this Index x) => new Index(x.Column - 1, x.Row - 1);
}