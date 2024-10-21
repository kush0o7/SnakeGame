namespace Snake
{
    internal class Coord
    {
        private int x;
        private int y;

        public int X { get { return x; } }
        public int Y { get { return y; } }

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object? obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
                return false;

            Coord other = (Coord)obj;
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public Coord ApplyMovementDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return new Coord(x - 1, y);
                case Direction.Right:
                    return new Coord(x + 1, y);
                case Direction.Up:
                    return new Coord(x, y - 1);
                case Direction.Down:
                    return new Coord(x, y + 1);
                default:
                    return this;
            }
        }
    }
}
