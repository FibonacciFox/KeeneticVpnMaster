using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Input;
using ReactiveUI;

namespace KeeneticVpnMaster.Models
{
    public class WireGuardShowInterface : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null!) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        private int _index;
        [JsonPropertyName("index")]
        public int Index
        {
            get => _index;
            set { _index = value; OnPropertyChanged(); }
        }

        private string _interfaceName;
        [JsonPropertyName("interface-name")]
        public string InterfaceName
        {
            get => _interfaceName;
            set { _interfaceName = value; OnPropertyChanged(); }
        }

        private string _description;
        [JsonPropertyName("description")]
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private string _type;
        [JsonPropertyName("type")]
        public string Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(); }
        }

        private string _link;
        [JsonPropertyName("link")]
        public string Link
        {
            get => _link;
            set { _link = value; OnPropertyChanged(); }
        }

        private string _connected;
        [JsonPropertyName("connected")]
        public string Connected
        {
            get => _connected;
            set { _connected = value; OnPropertyChanged(); }
        }

        private string _state;
        [JsonPropertyName("state")]
        public string State
        {
            get => _state;
            set { _state = value; OnPropertyChanged(); }
        }

        private int _mtu;
        [JsonPropertyName("mtu")]
        public int Mtu
        {
            get => _mtu;
            set { _mtu = value; OnPropertyChanged(); }
        }

        private string _address;
        [JsonPropertyName("address")]
        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(); }
        }

        private string _mask;
        [JsonPropertyName("mask")]
        public string Mask
        {
            get => _mask;
            set { _mask = value; OnPropertyChanged(); }
        }

        private long _uptime;
        [JsonPropertyName("uptime")]
        public long Uptime
        {
            get => _uptime;
            set { _uptime = value; OnPropertyChanged(); }
        }

        private WireGuardDetails _wireGuard;
        [JsonPropertyName("wireguard")]
        public WireGuardDetails WireGuard
        {
            get => _wireGuard;
            set { _wireGuard = value; OnPropertyChanged(); }
        }
        
        [JsonIgnore]
        public ICommand? ToggleConnectionCommand { get; set; }
    }

    public class WireGuardDetails : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string _publicKey;
        [JsonPropertyName("public-key")]
        public string PublicKey
        {
            get => _publicKey;
            set { _publicKey = value; OnPropertyChanged(); }
        }

        private int _listenPort;
        [JsonPropertyName("listen-port")]
        public int ListenPort
        {
            get => _listenPort;
            set { _listenPort = value; OnPropertyChanged(); }
        }

        private string _status;
        [JsonPropertyName("status")]
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        private List<WireGuardPeerDetails> _peer;
        [JsonPropertyName("peer")]
        public List<WireGuardPeerDetails> Peer
        {
            get => _peer;
            set { _peer = value; OnPropertyChanged(); }
        }
    }

    public class WireGuardPeerDetails : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string _publicKey;
        [JsonPropertyName("public-key")]
        public string PublicKey
        {
            get => _publicKey;
            set { _publicKey = value; OnPropertyChanged(); }
        }

        private string _remote;
        [JsonPropertyName("remote")]
        public string Remote
        {
            get => _remote;
            set { _remote = value; OnPropertyChanged(); }
        }

        private int _remotePort;
        [JsonPropertyName("remote-port")]
        public int RemotePort
        {
            get => _remotePort;
            set { _remotePort = value; OnPropertyChanged(); }
        }

        private long? _rxBytes;
        [JsonPropertyName("rxbytes")]
        public long? RxBytes
        {
            get => _rxBytes;
            set { _rxBytes = value; OnPropertyChanged(); }
        }

        private long? _txBytes;
        [JsonPropertyName("txbytes")]
        public long? TxBytes
        {
            get => _txBytes;
            set { _txBytes = value; OnPropertyChanged(); }
        }

        private long? _lastHandshake;
        [JsonPropertyName("last-handshake")]
        public long? LastHandshake
        {
            get => _lastHandshake;
            set { _lastHandshake = value; OnPropertyChanged(); }
        }

        private bool _online;
        [JsonPropertyName("online")]
        public bool Online
        {
            get => _online;
            set { _online = value; OnPropertyChanged(); }
        }

        // Вычисляемое поле для DataGrid
        public string PeerAddressPort => $"{Remote}:{RemotePort}";
    }
}
