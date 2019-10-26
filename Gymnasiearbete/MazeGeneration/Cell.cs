namespace Gymnasiearbete.MazeGeneration
{
    class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }

        public bool IsVissited { get; set; }

        public bool WallTop { get; set; } = true;
        public bool WallRight { get; set; } = true;
        public bool WallBottom { get; set; } = true;
        public bool WallLeft { get; set; } = true;

        public void RemoveWall(int[] d)
        {
            if (d[0] == -1)
                WallLeft = false;
            else if (d[0] == 1)
                WallRight = false;
            if (d[1] == -1)
                WallTop = false;
            else if (d[1] == 1)
                WallBottom = false;
        }
    }
}