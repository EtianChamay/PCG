using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PCG
{
    internal class Room
    {
        private const int MIN_PADDING = 3;

        private readonly Point _leftUpperPoint;
        private readonly Point _rightBottomPoint;
        private readonly Random _random;
        private readonly List<Room> _subRooms;

        private Line _line;

        public Room(Point leftUpperPoint, Point rightBottomPoint)
        {
            _leftUpperPoint = leftUpperPoint;
            _rightBottomPoint = rightBottomPoint;
            _random = new Random();
            _subRooms =  new List<Room>();
        }

        public void Split(int depth)
        {
            if (depth == 0)
            {
                return;
            }

            var splitDirection = CanSplit();
            if (splitDirection == SplitDirection.None)
            {
                return;
            }

            CalcLimitPoints(out var minStartPoint, out var minEndPoint);
            Point startPoint;
            Point endPoint;

            switch (splitDirection)
            {
                case SplitDirection.Horizontal:
                    CreateHorizontalLine(minStartPoint, minEndPoint, out startPoint, out endPoint);
                    break;
                case SplitDirection.Vertical:
                    CreateVerticalLine(minStartPoint, minEndPoint, out startPoint, out endPoint);
                    break;
                case SplitDirection.Both:
                    var randomSplitDirection = _random.Next(2);
                    if (randomSplitDirection == 0) // horizontal split
                    {
                        CreateHorizontalLine(minStartPoint, minEndPoint, out startPoint, out endPoint);
                    }
                    else // vertical split
                    {
                        CreateVerticalLine(minStartPoint, minEndPoint, out startPoint, out endPoint);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _line = new Line(startPoint,endPoint);

            var roomA = new Room(_leftUpperPoint, endPoint);
            var roomB = new Room(startPoint, _rightBottomPoint);

            _subRooms.Add(roomA);
            _subRooms.Add(roomB);

            if (_subRooms.Any())
            {
                --depth;
                foreach (var subRoom in _subRooms)
                {
                    subRoom.Split(depth);
                }
            }
        }

        private void CreateVerticalLine(Point minStartPoint, Point minEndPoint, out Point startPoint, out Point endPoint)
        {
            var xCoordination = _random.Next(minStartPoint.X, minEndPoint.X + 1);

            startPoint = new Point(xCoordination, _leftUpperPoint.Y);
            endPoint = new Point(xCoordination, _rightBottomPoint.Y);
        }

        private void CreateHorizontalLine(Point minStartPoint, Point minEndPoint, out Point startPoint, out Point endPoint)
        {
            var yCoordination = _random.Next(minStartPoint.Y, minEndPoint.Y + 1);

            startPoint = new Point(_leftUpperPoint.X, yCoordination);
            endPoint = new Point(_rightBottomPoint.X, yCoordination);
        }

        public SplitDirection CanSplit()
        {
            CalcLimitPoints(out var minStartPoint, out var minEndPoint);

            var canVertical = minStartPoint.X < minEndPoint.X;
            var canHorizontal = minStartPoint.Y < minEndPoint.Y;

            if (canVertical && canHorizontal)
            {
                return SplitDirection.Both;
            }
            else if (canVertical)
            {
                return SplitDirection.Vertical;
            }
            else if (canHorizontal)
            {
                return SplitDirection.Horizontal;
            }
            else
            {
                return SplitDirection.None;
            }
        }

        private void CalcLimitPoints(out Point minStartPoint, out Point minEndPoint)
        {
            minStartPoint = Point.Add(_leftUpperPoint, new Size(MIN_PADDING, MIN_PADDING));
            minEndPoint = Point.Subtract(_rightBottomPoint, new Size(MIN_PADDING, MIN_PADDING));
        }

        public void Draw(int[,] board, int id)
        {
            if (_subRooms.Any())
            {
                foreach (var subRoom in _subRooms)
                {
                    subRoom.Draw(board, id);
                }
            }
            else
            {
                DrawSelf(board, id);
            }

            DrawLine(board);
        }

        private void DrawSelf(int[,] board, int id)
        {
            for (var x = _leftUpperPoint.X; x < _rightBottomPoint.X; x++)
            {
                for (var y = _leftUpperPoint.Y; y < _rightBottomPoint.Y; y++)
                {
                    board[x, y] = id;
                }
            }
        }

        private void DrawLine(int[,] board)
        {
            if (_line != null)
            {
                for (var x = _line.StartPoint.X; x < _line.EndPoint.X; x++)
                {
                    board[x, _line.StartPoint.Y] = 0;
                }

                for (var y = _line.StartPoint.Y; y < _line.EndPoint.Y; y++)
                {
                    board[_line.StartPoint.X, y] = 0;
                }
            }
        }
    }

    internal enum SplitDirection
    {
        Horizontal,
        Vertical,
        Both,
        None
    }
}