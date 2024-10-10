using System.Collections.Generic;

namespace SnowballSpin.Network;

class GameState
{
    public List<PlayerData> Players { get; set; } = [];
    public List<SnowballData> Snowballs { get; set; } = [];
}

class ReportPackage
{
    public PlayerData Player { get; set; }
    public List<SnowballData> Snowballs { get; set; } = [];
    public bool JustLeft { get; set; }
}

class PlayerData
{
    public string PlayerId { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float Rotation { get; set; }
}

class SnowballData
{
    public string PlayerId { get; set; }
    public float Rotation { get; set; }
}