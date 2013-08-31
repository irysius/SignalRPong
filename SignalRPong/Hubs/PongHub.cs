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
    /// </remarks>
    [HubName("PongHub")]
    public class PongHub : Hub
    {
        public void JoinGame(string displayName)
        {

        }

        public void LeaveGame()
        {
 
        }

        public void StartGame()
        {
 
        }
    }
}