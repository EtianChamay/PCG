using System.Drawing;

namespace PCG
{
    internal class Line
    {
        public Point StartPoint { get; }
        public Point EndPoint { get; }

        public Line(Point startPoint, Point endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }
    }
}