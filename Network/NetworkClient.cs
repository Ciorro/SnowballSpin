using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SnowballSpin.Network
{
    internal static class NetworkClient
    {
        private static TcpClient _client;
        private static NetworkStream _stream;
        private static GameState _gameState = new();
        private static string _playerId;
        private static PlayerData _currentPlayerData;
        private static bool _playerJustLeft;
        private static List<SnowballData> _currentSnowballData = [];

        private static bool _connected;

        public static void ReportPlayerData(PlayerData data)
        {
            _currentPlayerData = data;
        }

        public static void ReportGameLeft()
        {
            _playerJustLeft = true;
        }

        public static void ReportSnowballData(SnowballData data)
        {
            _currentSnowballData.Add(data);
        }

        public static string GetPlayerId()
        {
            return _playerId;
        }

        public static string Connect()
        {
            try
            {
                _client = new TcpClient("127.0.0.1", 8080);
                _stream = _client.GetStream();
                using (var reader = new StreamReader(_stream, Encoding.UTF8, leaveOpen: true))
                {
                    string playerId = reader.ReadLine();
                    Console.WriteLine($"Your Player ID: {playerId}"); // Print the ID for debugging
                    _playerId = playerId;
                }
                _connected = true;
                Console.WriteLine("Connected to server");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to connect to server: " + ex.Message);
                _connected = false;
            }

            return _playerId;
        }

        public static void Update()
        {
            if (!_connected) return;

            if (_currentPlayerData != null)
            {
                SendData(new ReportPackage()
                {
                    Player = _currentPlayerData,
                    Snowballs = _currentSnowballData ?? [],
                    JustLeft = _playerJustLeft
                });
                _currentSnowballData.Clear();
            }

            if (_playerJustLeft)
            {
                Disconnect();
                _playerJustLeft = false;
                return;
            }
            ReceiveDataFromServer();
        }

        private static void SendData<T>(T data)
        {
            if (_client != null && _client.Connected)
            {
                string jsonData = JsonSerializer.Serialize(data);
                byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData + "\n");
                _stream.Write(dataBytes, 0, dataBytes.Length);
                // Console.WriteLine($"SENT: {jsonData}");
            }
        }

        private static readonly StringBuilder _jsonBuffer = new();
        internal static readonly char[] separator = ['\n'];

        private static void ReceiveDataFromServer()
        {
            if (_client != null && _client.Connected && _stream.DataAvailable)
            {
                byte[] data = new byte[256];
                int bytes = _stream.Read(data, 0, data.Length);
                _jsonBuffer.Append(Encoding.UTF8.GetString(data, 0, bytes));

                ProcessJsonBuffer();
            }
        }

        private static void ProcessJsonBuffer()
        {
            var jsonMessages = _jsonBuffer.ToString().Split(separator, StringSplitOptions.None);

            for (int i = 0; i < jsonMessages.Length - 1; i++)
            {
                string message = jsonMessages[i].Trim();

                if (string.IsNullOrEmpty(message))
                {
                    continue;
                }

                try
                {
                    // Console.WriteLine($"RECEIVED: {message}");
                    _gameState = JsonSerializer.Deserialize<GameState>(message);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Deserialization error: {ex.Message}");
                }
            }

            _jsonBuffer.Clear();
            if (!string.IsNullOrWhiteSpace(jsonMessages[^1]) && !jsonMessages[^1].EndsWith("\n"))
            {
                _jsonBuffer.Append(jsonMessages[^1]);
            }
        }


        public static GameState GetGameState()
        {
            var gameStateClone = new GameState()
            {
                Players = _gameState.Players,
                Snowballs = new List<SnowballData>(_gameState.Snowballs)
            };

            _gameState.Snowballs = new();
            return gameStateClone;
        }

        public static void Disconnect()
        {
            if (_client != null)
            {
                _client.Close();
                _connected = false;
            }
        }
    }
}
