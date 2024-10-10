using System.Linq;
using Microsoft.Xna.Framework;
using SnowballSpin.Controllers;
using SnowballSpin.Network;
using SnowballSpin.Objects;

class NetworkPlayerController() : IPlayerController
{
    // private NetworkClient _networkClient;

    // public NetworkPlayerController(NetworkClient networkClient)
    // {
    //     _networkClient = networkClient;
    // }

    public void ControlPlayer(Player player)
    {
        //         var gameState = NetworkClient.GetGameState();
        // var playerData = gameState.Players.Where(x => x.PlayerId == player.Id).FirstOrDefault();
        // if (playerData == null) return;


        // // Use data received from the server to control player
        // // PlayerData data = _networkClient.GetGameState();
        // // if (data != null)
        // // {
        //     player.Body.Position = new Vector2(playerData.PositionX, playerData.PositionY);
        //     player.Rotation = playerData.Rotation;
        // // }
    }
}
