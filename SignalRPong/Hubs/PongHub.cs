using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SignalRPong.Hubs
{
    /// <summary>
    /// SignalR hub class for the pong game.
    /// </summary>
    /// <remarks>
    /// Valid javascript methods:
    /// updateMessage(string message);
    /// updatePositions(double p1, double p2, double bx, double by);
    /// updateScore(int s1, int s2);
    /// onRoomRegistered(string roomId, int playerId);
    /// onGameStartable();
    /// onGameUnstartable();
    /// onGameStarted();
    /// </remarks>
    [HubName("PongHub")]
    public class PongHub : Hub
    {
        public PongHub()
        {
            PongServer.UpdatePositions += PongServer_UpdatePositions;
            PongServer.UpdateScore += PongServer_UpdateScore;
        }

        private void PongServer_UpdateScore(object sender, Tuple<string, int, int> e)
        {
            Clients.Group(e.Item1).updateScore(e.Item2, e.Item3);
        }

        private void PongServer_UpdatePositions(object sender, Tuple<string, double, double, double, double> e)
        {
            Clients.Group(e.Item1).updatePositions(e.Item2, e.Item3, e.Item4, e.Item5);
        }

        public void JoinGame(string displayName)
        {
            var fillableRoom = PongServer.Rooms.FirstOrDefault(r => r.HasVacancies);
            if (fillableRoom != null)
            {
                int playerNumber = 1;
                if (String.IsNullOrEmpty(fillableRoom.Player1))
                    fillableRoom.Player1 = displayName;
                else 
                {
                    fillableRoom.Player2 = displayName;
                    playerNumber = 2;
                }

                Groups.Add(Context.ConnectionId, fillableRoom.RoomID);
                Clients.Caller.onRoomRegistered(fillableRoom.RoomID, playerNumber);
                Clients.Group(fillableRoom.RoomID).updateMessage(displayName + " joined.");

                if (!fillableRoom.IsVacant)
                    Clients.Group(fillableRoom.RoomID).onGameStartable();
            }
            else
            {
                var newRoom = new PongRoom();
                newRoom.Player1 = displayName;
                PongServer.Rooms.Add(newRoom);
                Groups.Add(Context.ConnectionId, newRoom.RoomID);
                Clients.Caller.onRoomRegistered(newRoom.RoomID, 1);
                Clients.Group(newRoom.RoomID).updateMessage(displayName + " created room.");
            }
        }

        public void LeaveGame(string roomId)
        {
            var room = PongServer.Rooms.FirstOrDefault(r => r.RoomID == roomId);
            if (room != null)
            {
                Groups.Remove(Context.ConnectionId, room.RoomID);
            }
        }

        public void StartGame(string roomId)
        {
            PongServer.StartGame(roomId);
            Clients.Group(roomId).onGameStarted();
        }

        public void ServeBall(string roomId, bool isPlayer1)
        {
            PongServer.ServeBall(roomId, isPlayer1);
        }
        public void UpdatePaddle(string roomId, bool isPlayer1, int paddleY)
        {
            PongServer.UpdatePaddle(roomId, isPlayer1, paddleY);
        }
    }
}