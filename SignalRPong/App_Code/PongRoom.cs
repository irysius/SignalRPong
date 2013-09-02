using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRPong
{
    public enum PongRoomState
    {
        Waiting,
        InProgress,
        Ended
    }

    public enum PongBallState
    {
        Player1,
        Player2,
        InMotion
    }

    public class PongRoom
    {
        public PongRoom()
        {
            RoomID = Guid.NewGuid().ToString();
            rnd = new Random();
        }

        Random rnd;
        public string RoomID { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public double Paddle1 { get; set; }
        public double Paddle2 { get; set; }
        public double BallX { get; set; }
        public double BallY { get; set; }
        public double BallVX { get; set; }
        public double BallVY { get; set; }
        public PongRoomState RoomState { get; set; }
        public PongBallState BallState { get; set; }

        public bool HasVacancies
        {
            get { return String.IsNullOrEmpty(Player1) || String.IsNullOrEmpty(Player2); }
        }
        public bool IsVacant
        {
            get { return String.IsNullOrEmpty(Player1) && String.IsNullOrEmpty(Player2); }
        }

        public void UpdatePaddle(bool isPlayer1, double paddleY)
        {
            // Server side checking for bounds as well.
            var temp = paddleY - PongServer.PaddleHeight / 2;
            if (temp < 0)
                temp = 0;
            if (temp > PongServer.RoomHeight - PongServer.PaddleHeight)
                temp = PongServer.RoomHeight - PongServer.PaddleHeight;

            if (isPlayer1)
                Paddle1 = temp;
            else
                Paddle2 = temp;
        }

        public void ServeBall(bool isPlayer1)
        {
            if (BallState != PongBallState.InMotion)
            {
                if (BallState == PongBallState.Player1)
                {
                    BallVX = rnd.NextDouble() * 2 + 1;
                    BallVY = rnd.NextDouble() + 1; 
                    if (rnd.NextDouble() < 0.5)
                        BallVY = -BallVY;
                }
                else
                {
                    BallVX = -1 - rnd.NextDouble() * 2;
                    BallVY = rnd.NextDouble() + 1;
                    if (rnd.NextDouble() < 0.5)
                        BallVY = -BallVY;
                }
                BallState = PongBallState.InMotion;
            }
        }
    }

    
}