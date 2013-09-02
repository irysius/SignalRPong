using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace SignalRPong
{
    public static class PongServer
    {
        static PongServer()
        {
            Rooms = new List<PongRoom>();
            BallWidth = 23;
            BallHeight = 23;
            PaddleWidth = 13;
            PaddleHeight = 68;
            RoomWidth = 700;
            RoomHeight = 480;
            rnd = new Random();
            Thread t = new Thread(() => {
                PongServer.Start();
            });
            t.Start();
        }
        public static List<PongRoom> Rooms { get; set; }
        public static bool IsRunning { get; set; }
        public static int BallWidth { get; set; }
        public static int BallHeight { get; set; }
        public static int PaddleWidth { get; set; }
        public static int PaddleHeight { get; set; }
        public static int RoomWidth { get; set; }
        public static int RoomHeight { get; set; }
        static Random rnd;

        public static void Start()
        {
            IsRunning = true;
            while (IsRunning)
            {
                Update();
                Thread.Sleep(40);
            }
        }
        public static void Stop()
        {
            IsRunning = false;
        }
        public static void Update()
        {
            var roomsToUpdate = PongServer.Rooms.Where(r => r.RoomState == PongRoomState.InProgress);
            for (int i = 0; i < PongServer.Rooms.Count; ++i)
            {
                var room = PongServer.Rooms[i];
                switch (room.RoomState)
                {
                    case PongRoomState.Waiting:
                        int j = 0;
                        break;
                    case PongRoomState.InProgress:
                        UpdateRoom(room);
                        break;
                }
            }
        }
        private static void UpdateRoom(PongRoom room)
        {
            switch (room.BallState)
            {
                case PongBallState.InMotion:
                    room.BallX += room.BallVX;
                    room.BallY += room.BallVY;

                    if (room.BallY - BallHeight / 2 < 0)
                    {   // bounce top
                        room.BallY = BallHeight / 2;
                        room.BallVY = -room.BallVY;
                    }
                    if (room.BallY + BallHeight / 2 > RoomHeight)
                    {   // bounce bottom
                        room.BallY = RoomHeight - BallHeight / 2;
                        room.BallVY = -room.BallVY;
                    }

                    if (room.BallX - BallWidth / 2 < PaddleWidth)
                    {   // bounce left
                        double lowerBound = room.Paddle1 - BallHeight / 3;
                        double upperBound = room.Paddle1 + BallHeight * 4 / 3;
                        if (lowerBound < room.BallY && room.BallY < upperBound)
                        {
                            room.BallX = PaddleWidth + BallWidth / 2;

                            room.BallVY = VerticalUpdateHelper(room.BallVY);
                            room.BallVX = -room.BallVX + rnd.NextDouble() / 2;
                            if (room.BallVX < 0)
                                room.BallVX = 3;
                        }
                        else
                        {
                            room.BallState = PongBallState.Player1;
                            room.Score2++;
                            room.BallVX = 0;
                            room.BallVY = 0;
                            if (UpdateScore != null)
                            {
                                UpdateScore.Invoke(null, Tuple.Create(
                                    room.RoomID,
                                    room.Score1,
                                    room.Score2));
                            }
                        }
                    }
                    if (room.BallX + BallWidth / 2 > RoomWidth - PaddleWidth)
                    {   // bounce right
                        double lowerBound = room.Paddle1 - BallHeight / 3;
                        double upperBound = room.Paddle1 + BallHeight * 4 / 3;
                        if (lowerBound < room.BallY && room.BallY < upperBound)
                        {
                            room.BallX = RoomWidth - PaddleWidth - BallWidth / 2;

                            room.BallVY = VerticalUpdateHelper(room.BallVY);
                            room.BallVX = -room.BallVX - rnd.NextDouble() / 2;
                            if (room.BallVX > 0)
                                room.BallVX = -3;
                        }
                        else
                        {
                            room.BallState = PongBallState.Player2;
                            room.Score1++;
                            room.BallVX = 0;
                            room.BallVY = 0;
                            if (UpdateScore != null)
                            {
                                UpdateScore.Invoke(null, Tuple.Create(
                                    room.RoomID,
                                    room.Score1,
                                    room.Score2));
                            }
                        }
                    }
                    break;
                case PongBallState.Player1:
                    room.BallX = PaddleWidth + BallWidth / 2;
                    room.BallY = room.Paddle1 + PaddleHeight / 2;
                    break;
                case PongBallState.Player2:
                    room.BallX = RoomWidth - PaddleWidth + BallWidth / 2;
                    room.BallY = room.Paddle2 + PaddleHeight / 2;
                    break;
            }
            if (UpdatePositions != null)
            {
                UpdatePositions.Invoke(null, Tuple.Create(
                    room.RoomID,
                    room.Paddle1,
                    room.Paddle2,
                    room.BallX,
                    room.BallY));
            }
        }
        private static double VerticalUpdateHelper(double vy)
        {
            if (vy < 0)
            {
                vy = vy - rnd.NextDouble() + 0.5;
                if (vy > -0.5)
                    vy = -3;
            }
            else
            {
                vy = vy + rnd.NextDouble() - 0.5;
                if (vy < 0.5)
                    vy = 3;
            }

            return vy;
        }

        public static event EventHandler<Tuple<string, double, double, double, double>> UpdatePositions;
        public static event EventHandler<Tuple<string, int, int>> UpdateScore;

        public static void StartGame(string roomId)
        {
            var room = Rooms.FirstOrDefault(r => r.RoomID == roomId);
            if (room != null)
                room.RoomState = PongRoomState.InProgress;
        }
        public static void UpdatePaddle(string roomId, bool isPlayer1, int paddleY)
        {
            var room = Rooms.FirstOrDefault(r => r.RoomID == roomId);
            if (room != null)
                room.UpdatePaddle(isPlayer1, paddleY);
        }
        public static void ServeBall(string roomId, bool isPlayer1)
        {
            var room = Rooms.FirstOrDefault(r => r.RoomID == roomId);
            if (room != null)
                room.ServeBall(isPlayer1);
        }
    }
}