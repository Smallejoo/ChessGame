using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChessGame
{
    public class NetworkManager
    {
        private TcpClient? _client;
        private StreamReader? _reader;
        private StreamWriter? _writer;

        // fromRow, fromCol, toRow, toCol
        public event Action<int, int, int, int>? OnMoveReceived;

        private const int Port = 5000; // use same port in host + join

        // HOST side: wait for one player to connect
        public void StartServer()
        {
            var listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();

            // this blocks until someone connects
            _client = listener.AcceptTcpClient();
            listener.Stop();

            SetupStreams();
            StartListening();
        }

        // JOIN side: connect to existing host
        public void ConnectToServer(string hostIp)
        {
            _client = new TcpClient(hostIp, Port);
            SetupStreams();
            StartListening();
        }

        private void SetupStreams()
        {
            if (_client == null) return;

            _reader = new StreamReader(_client.GetStream());
            _writer = new StreamWriter(_client.GetStream())
            {
                AutoFlush = true
            };
        }

        private void StartListening()
        {
            if (_reader == null) return;

            new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        string? line = _reader.ReadLine();
                        if (line == null) break;

                        var parts = line.Split(';');
                        if (parts.Length == 5 && parts[0] == "MOVE")
                        {
                            int fr = int.Parse(parts[1]);
                            int fc = int.Parse(parts[2]);
                            int tr = int.Parse(parts[3]);
                            int tc = int.Parse(parts[4]);

                            OnMoveReceived?.Invoke(fr, fc, tr, tc);
                        }
                    }
                }
                catch
                {
                    // connection closed, ignore for now
                }
            })
            {
                IsBackground = true
            }.Start();
        }

        public void SendMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            _writer?.WriteLine($"MOVE;{fromRow};{fromCol};{toRow};{toCol}");
        }
    }
}
