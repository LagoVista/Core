﻿using LagoVista.Core.Models;
using System;

namespace LagoVista.Core.Networking.WiFi
{
    public class WiFiConnection : ModelBase
    {
        public WiFiConnection(string id, string ssid, string bssid, bool isOpen)
        {
            Id = id;
            Ssid = id;
            Bssid = bssid;
            IsOpen = isOpen;
        }

        public bool IsOpen { get; }

        public String Id { get; }

        public String Ssid { get; }

        public string Bssid { get; set; }

        private double _signalDb;
        public double SignalDB
        {
            get { return _signalDb; }
            set { Set(ref _signalDb, value); }
        }

        private int _signalBars;
        public int SignalBars
        {
            get { return _signalBars; }
            set { Set(ref _signalBars, value); }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set { Set(ref _isConnected, value); }
        }
    }
}
