using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KeeneticVpnMaster.Models
{
    /// <summary>
    /// Представляет конфигурацию WireGuard-интерфейса в Keenetic.
    /// </summary>
    public class WireGuardConfigurationInterface
    {
        /// <summary>
        /// Описание интерфейса (имя подключения).
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Настройки Dynamic DNS (DynDNS).
        /// </summary>
        [JsonPropertyName("dyndns")]
        public DynDns? Dyndns { get; set; }

        /// <summary>
        /// Уровень безопасности соединения.
        /// </summary>
        [JsonPropertyName("security-level")]
        public SecurityLevel? SecurityLevel { get; set; }

        /// <summary>
        /// Настройки IP (адрес, MTU, DNS, TCP).
        /// </summary>
        [JsonPropertyName("ip")]
        public IpConfiguration? Ip { get; set; }

        /// <summary>
        /// Настройки IPv6 (адреса и префиксы).
        /// </summary>
        [JsonPropertyName("ipv6")]
        public Ipv6Configuration? Ipv6 { get; set; }

        /// <summary>
        /// Параметры WireGuard (пиры, ключи, настройки ASC).
        /// </summary>
        [JsonPropertyName("wireguard")]
        public WireGuard? WireGuard { get; set; }

        /// <summary>
        /// Включено ли соединение.
        /// </summary>
        [JsonPropertyName("up")]
        public bool Up { get; set; }
    }

    /// <summary>
    /// Настройки Dynamic DNS (DynDNS).
    /// </summary>
    public class DynDns
    {
        /// <summary>
        /// Отключает привязку интерфейса к IP-адресу.
        /// </summary>
        [JsonPropertyName("nobind")]
        public bool Nobind { get; set; }
    }

    /// <summary>
    /// Уровень безопасности соединения.
    /// </summary>
    public class SecurityLevel
    {
        /// <summary>
        /// Флаг, указывающий, является ли соединение публичным.
        /// </summary>
        [JsonPropertyName("public")]
        public bool Public { get; set; }
    }

    /// <summary>
    /// Настройки IP-адресации и сети.
    /// </summary>
    public class IpConfiguration
    {
        /// <summary>
        /// Локальный IP-адрес и маска сети.
        /// </summary>
        [JsonPropertyName("address")]
        public IpAddress? Address { get; set; }

        /// <summary>
        /// Максимальный размер пакетов MTU.
        /// </summary>
        [JsonPropertyName("mtu")]
        public string? Mtu { get; set; }

        /// <summary>
        /// Глобальные настройки IP.
        /// </summary>
        [JsonPropertyName("global")]
        public GlobalSettings? Global { get; set; }

        /// <summary>
        /// Список DNS-серверов.
        /// </summary>
        [JsonPropertyName("name-server")]
        public List<NameServer>? NameServers { get; set; }

        /// <summary>
        /// Настройки TCP.
        /// </summary>
        [JsonPropertyName("tcp")]
        public TcpSettings? Tcp { get; set; }
    }

    /// <summary>
    /// Представляет IP-адрес и маску сети.
    /// </summary>
    public class IpAddress
    {
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("mask")]
        public string? Mask { get; set; }
    }

    /// <summary>
    /// Глобальные параметры IP.
    /// </summary>
    public class GlobalSettings
    {
        [JsonPropertyName("order")]
        public int Order { get; set; }

        [JsonPropertyName("priority")]
        public int Priority { get; set; }
    }

    /// <summary>
    /// DNS-сервер.
    /// </summary>
    public class NameServer
    {
        [JsonPropertyName("name-server")] public string? Name { get; set; }
    }

    /// <summary>
    /// Настройки TCP.
    /// </summary>
    public class TcpSettings
    {
        [JsonPropertyName("adjust-mss")] public AdjustMss? AdjustMss { get; set; }
    }

    public class AdjustMss
    {
        /// <summary>
        /// Включение Path MTU Discovery (PMTU).
        /// </summary>
        [JsonPropertyName("pmtu")]
        public bool Pmtu { get; set; }
    }

    /// <summary>
    /// Настройки IPv6 (адреса и префиксы).
    /// </summary>
    public class Ipv6Configuration
    {
        [JsonPropertyName("address")] public List<Ipv6Address>? Addresses { get; set; }

        [JsonPropertyName("prefix")] public List<Ipv6Prefix>? Prefixes { get; set; }
    }

    public class Ipv6Address
    {
        [JsonPropertyName("auto")] public bool Auto { get; set; }
    }

    public class Ipv6Prefix
    {
        [JsonPropertyName("auto")] public bool Auto { get; set; }
    }

    /// <summary>
    /// Настройки WireGuard.
    /// </summary>
    public class WireGuard
    {
        /// <summary>
        /// Настройки ASC (Adaptive Security Control).
        /// </summary>
        [JsonPropertyName("asc")]
        public AscSettings? Asc { get; set; }

        /// <summary>
        /// Список пиринговых соединений.
        /// </summary>
        [JsonPropertyName("peer")]
        public List<WireGuardPeer>? Peers { get; set; }
    }

    public class AscSettings
    {
        [JsonPropertyName("jc")] public string? Jc { get; set; }

        [JsonPropertyName("jmin")] public string? Jmin { get; set; }

        [JsonPropertyName("jmax")] public string? Jmax { get; set; }

        [JsonPropertyName("s1")] public string? S1 { get; set; }

        [JsonPropertyName("s2")] public string? S2 { get; set; }

        [JsonPropertyName("h1")] public string? H1 { get; set; }

        [JsonPropertyName("h2")] public string? H2 { get; set; }

        [JsonPropertyName("h3")] public string? H3 { get; set; }

        [JsonPropertyName("h4")] public string? H4 { get; set; }
    }

    /// <summary>
    /// Представляет пир WireGuard.
    /// </summary>
    public class WireGuardPeer
    {
        [JsonPropertyName("key")] public string? Key { get; set; }

        [JsonPropertyName("comment")] public string? Comment { get; set; }

        [JsonPropertyName("endpoint")] public Endpoint? Endpoint { get; set; }

        [JsonPropertyName("keepalive-interval")]
        public KeepaliveInterval? KeepaliveInterval { get; set; }

        [JsonPropertyName("preshared-key")] public string? PresharedKey { get; set; }

        [JsonPropertyName("allow-ips")] public List<IpAddress>? AllowIps { get; set; }
    }

    /// <summary>
    /// Настройки конечной точки соединения WireGuard.
    /// </summary>
    public class Endpoint
    {
        [JsonPropertyName("address")] public string? Address { get; set; }
    }

    /// <summary>
    /// Интервал KeepAlive для WireGuard.
    /// </summary>
    public class KeepaliveInterval
    {
        [JsonPropertyName("interval")] public int Interval { get; set; }
    }
}
