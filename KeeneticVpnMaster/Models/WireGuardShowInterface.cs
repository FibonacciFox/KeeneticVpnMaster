using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Windows.Input;
using KeeneticVpnMaster.Helpers;

namespace KeeneticVpnMaster.Models
{
    public class WireGuardShowInterface : ObservableObject
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        private int _index;
        [JsonPropertyName("index")]
        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        private string _interfaceName = string.Empty;
        [JsonPropertyName("interface-name")]
        public string InterfaceName
        {
            get => _interfaceName;
            set => SetProperty(ref _interfaceName, value);
        }

        private string _description = string.Empty;
        [JsonPropertyName("description")]
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _type = string.Empty;
        [JsonPropertyName("type")]
        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        private string _link = string.Empty;
        [JsonPropertyName("link")]
        public string Link
        {
            get => _link;
            set => SetProperty(ref _link, value);
        }

        private string _connected = string.Empty;
        [JsonPropertyName("connected")]
        public string Connected
        {
            get => _connected;
            set => SetProperty(ref _connected, value);
        }

        private string _state = string.Empty;
        [JsonPropertyName("state")]
        public string State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        private int _mtu;
        [JsonPropertyName("mtu")]
        public int Mtu
        {
            get => _mtu;
            set => SetProperty(ref _mtu, value);
        }

        private string _address = "-";
        [JsonPropertyName("address")]
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        private string _mask = string.Empty;
        [JsonPropertyName("mask")]
        public string Mask
        {
            get => _mask;
            set => SetProperty(ref _mask, value);
        }

        private long _uptime;
        [JsonPropertyName("uptime")]
        public long Uptime
        {
            get => _uptime;
            set => SetProperty(ref _uptime, value);
        }

        private WireGuardDetails _wireGuard = new();
        [JsonPropertyName("wireguard")]
        public WireGuardDetails WireGuard
        {
            get => _wireGuard;
            set => SetProperty(ref _wireGuard, value);
        }

        [JsonIgnore]
        public ICommand? ToggleConnectionCommand { get; set; }
    }

    public class WireGuardDetails : ObservableObject
    {
        private string _publicKey = string.Empty;
        [JsonPropertyName("public-key")]
        public string PublicKey
        {
            get => _publicKey;
            set => SetProperty(ref _publicKey, value);
        }

        private int _listenPort;
        [JsonPropertyName("listen-port")]
        public int ListenPort
        {
            get => _listenPort;
            set => SetProperty(ref _listenPort, value);
        }

        private string _status = string.Empty;
        [JsonPropertyName("status")]
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private List<WireGuardPeerDetails> _peer = new();
        [JsonPropertyName("peer")]
        public List<WireGuardPeerDetails> Peer
        {
            get => _peer;
            set => SetProperty(ref _peer, value);
        }
    }

    public class WireGuardPeerDetails : ObservableObject
    {
        private string _publicKey = string.Empty;
        [JsonPropertyName("public-key")]
        public string PublicKey
        {
            get => _publicKey;
            set => SetProperty(ref _publicKey, value);
        }

        private string _remote = string.Empty;
        [JsonPropertyName("remote")]
        public string Remote
        {
            get => _remote;
            set => SetProperty(ref _remote, value);
        }

        private int _remotePort;
        [JsonPropertyName("remote-port")]
        public int RemotePort
        {
            get => _remotePort;
            set => SetProperty(ref _remotePort, value);
        }

        private long? _rxBytes;
        [JsonPropertyName("rxbytes")]
        public long? RxBytes
        {
            get => _rxBytes;
            set => SetProperty(ref _rxBytes, value);
        }

        private long? _txBytes;
        [JsonPropertyName("txbytes")]
        public long? TxBytes
        {
            get => _txBytes;
            set => SetProperty(ref _txBytes, value);
        }

        private long? _lastHandshake;
        [JsonPropertyName("last-handshake")]
        public long? LastHandshake
        {
            get => _lastHandshake;
            set => SetProperty(ref _lastHandshake, value);
        }

        private bool _online;
        [JsonPropertyName("online")]
        public bool Online
        {
            get => _online;
            set => SetProperty(ref _online, value);
        }
    }
}
