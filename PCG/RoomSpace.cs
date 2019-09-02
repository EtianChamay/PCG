using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PCG
{
    internal class RoomSpace
    {
        private static readonly int MinPadding = GlobalConfiguration.MinRoomSpacePadding ;
        private static readonly int MaxRoomSize= Math.Max(GlobalConfiguration.MaxRoomWidth, GlobalConfiguration.MaxRoomHeight);

        private readonly Point _leftUpperPoint;
        private readonly Point _rightBottomPoint;
        private readonly Random _random;
        private readonly List<RoomSpace> _subRooms;

        private Line _line;
        private Room _room;

        public RoomSpace(Point leftUpperPoint, Point rightBottomPoint)
        {
            _leftUpperPoint = leftUpperPoint;
            _rightBottomPoint = rightBottomPoint;
            _random = new Random();
            _subRooms =  new List<RoomSpace>();
        }

        public void Split(int depth)
        {
            if (depth == 0)
            {
                return;
            }

            var splitOptions = SplitOptions();
            if (splitOptions == SplitDirection.None)
            {
                return;
            }

            Point startPoint;
            Point endPoint;

            CalcLimitPoints(out var minStartPoint, out var minEndPoint, MinPadding);

            switch (splitOptions)
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
                case SplitDirection.None:
                    throw new Exception("Logic exception, shouldn't get here");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _line = new Line(startPoint,endPoint);

            var roomA = new RoomSpace(_leftUpperPoint, endPoint);
            var roomB = new RoomSpace(startPoint, _rightBottomPoint);

            _subRooms.Add(roomA);
            _subRooms.Add(roomB);

            --depth;
            foreach (var subRoom in _subRooms)
            {
                subRoom.Split(depth);
            }
        }

        public void Draw(int[,] board, int background, int room)
        {
            if (_subRooms.Any())
            {
                foreach (var subRoom in _subRooms)
                {
                    subRoom.Draw(board, background, room);
                }
            }
            else
            {
                DrawSelf(board, background, room);
            }

            DrawLine(board);
        }

        public void GenerateRoom()
        {
            if (_subRooms.Any())
            {
                foreach (var subRoom in _subRooms)
                {
                    subRoom.GenerateRoom();
                }
            }
            else
            {
                CreateRoom();
            }
        }

        private void CreateRoom()
        {
            CalcLimitPoints(out var minStartPoint, out var minEndPoint, MaxRoomSize + 1);

            var availableUpperLeftPoint = Point.Add(_leftUpperPoint, new Size(1, 1));
            var availableRightBottomPoint = Point.Subtract(_rightBottomPoint, new Size(MaxRoomSize, MaxRoomSize));
            if (availableUpperLeftPoint.X >= availableRightBottomPoint.X || availableUpperLeftPoint.Y >= availableRightBottomPoint.Y)
            {
                return;
            }

            var width = _random.Next(GlobalConfiguration.MinRoomWidth, GlobalConfiguration.MaxRoomWidth + 1);
            var height = _random.Next(GlobalConfiguration.MinRoomHeight, GlobalConfiguration.MaxRoomHeight + 1);

            var yCoordination = _random.Next(availableUpperLeftPoint.Y, availableRightBottomPoint.Y + 1);
            var xCoordination = _random.Next(availableUpperLeftPoint.X, availableRightBottomPoint.X + 1);

            var startPoint = new Point(xCoordination, yCoordination);
            var endPoint = Point.Add(startPoint, new Size(width, height));
            _room = new Room(startPoint, endPoint);


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

        private SplitDirection SplitOptions()
        {
            CalcLimitPoints(out var minStartPoint, out var minEndPoint,MinPadding);

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

        private void CalcLimitPoints(out Point minStartPoint, out Point minEndPoint,params int[] padding)
        {
            var accumulatePadding = padding.Sum();
            minStartPoint = Point.Add(_leftUpperPoint, new Size(accumulatePadding, accumulatePadding));
            minEndPoint = Point.Subtract(_rightBottomPoint, new Size(accumulatePadding, accumulatePadding));
        }

        private void DrawSelf(int[,] board, int background, int room)
        {
            DrawBackground(board, background);
            if (_room != null)
            {
                DrawRoom(board, room);
            }
        }

        private void DrawRoom(int[,] board, int room)
        {
            _room.Draw(board, room);
        }

        private void DrawBackground(int[,] board, int id)
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

    internal class Room
    {
        private readonly Point _startPoint;
        private readonly Point _endPoint;

        public Room(Point startPoint, Point endPoint)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
        }

        public void Draw(int[,] board, int room)
        {
            for (var x = _startPoint.X; x < _endPoint.X; x++)
            {
                for (var y = _startPoint.Y; y < _endPoint.Y; y++)
                {
                    board[x, y] = room;
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