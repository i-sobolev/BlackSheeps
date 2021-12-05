namespace NavMeshGrid
{
    public enum Side
    {
        Left = 0,
        Right = 1,
        Upper = 2,
        Lower = 3,
        UpperLeft = 4,
        LowerRight = 5,
        UpperRight = 6,
        LowerLeft = 7
    }

    public static class SideExtentions
    {
        public static bool IsDiagonal(this Side side)
        {
            var isUpperLeft = side == Side.UpperLeft;
            var isUpperRight = side == Side.UpperRight;
            var isLowerRight = side == Side.LowerRight;
            var isLowerLeft = side == Side.LowerLeft;

            return isUpperLeft || isUpperRight || isLowerRight || isLowerLeft;
        }

        public static Side OppositeSide(this Side currentSide)
        {
            var currentSideId = (int)currentSide;
            return (Side)(currentSideId % 2 == 0 ? currentSideId + 1 : currentSideId - 1);
        }

    }
}