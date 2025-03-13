using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IniParser;
using IniParser.Model;
using KeeneticVpnMaster.Models;

namespace KeeneticVpnMaster.Helpers;

public static class WireGuardConfigParser
{
    public static WireGuardConfigurationInterface ParseWireGuardConf(string filePath)
    {
        var parser = new FileIniDataParser();
        IniData data = parser.ReadFile(filePath);

        var config = new WireGuardConfigurationInterface
        {
            Description = $"{Path.GetFileNameWithoutExtension(filePath)}1", // Название файла как описание
            Ip = new IpConfiguration
            {
                Address = new IpAddress
                {
                    Address = data["Interface"]["Address"]?.Split('/')[0], // Извлекаем IP
                    Mask = data["Interface"]["Address"]?.Split('/').Skip(1).FirstOrDefault()
                },
                NameServers = data["Interface"]["DNS"]?
                    .Split(',')
                    .Select(dns => new NameServer { Name = dns.Trim() })
                    .ToList()
            },
            WireGuard = new WireGuard
            {
                Asc = new AscSettings
                {
                    Jc = data["Interface"]["Jc"],
                    Jmin = data["Interface"]["Jmin"],
                    Jmax = data["Interface"]["Jmax"],
                    S1 = data["Interface"]["S1"],
                    S2 = data["Interface"]["S2"],
                    H1 = data["Interface"]["H1"],
                    H2 = data["Interface"]["H2"],
                    H3 = data["Interface"]["H3"],
                    H4 = data["Interface"]["H4"]
                },
                Peers = new List<WireGuardPeer>
                {
                    new WireGuardPeer
                    {
                        Key = data["Peer"]["PublicKey"],
                        PresharedKey = data["Peer"]["PresharedKey"],
                        Endpoint = new Endpoint { Address = data["Peer"]["Endpoint"] },
                        KeepaliveInterval = new KeepaliveInterval
                        {
                            Interval = int.TryParse(data["Peer"]["PersistentKeepalive"], out var interval) ? interval : 0
                        },
                        AllowIps = data["Peer"]["AllowedIPs"]?
                            .Split(',')
                            .Select(ip => new IpAddress
                            {
                                Address = ip.Trim().Split('/')[0],
                                Mask = ip.Trim().Split('/').Skip(1).FirstOrDefault()
                            })
                            .ToList()
                    }
                }
            }
        };

        Console.WriteLine($"Конфигурация загружена: {config.Description}");
        return config;
    }
}