using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using iplist.opencck.org.parser.Interfaces;
using iplist.opencck.org.parser.Models;

namespace iplist.opencck.org.parser
{
    /// <summary>
    /// Клиент для взаимодействия с API iplist.opencck.org.
    /// </summary>
    public class IplistClient : IIplistClient
    {
        private const string Url = "https://iplist.opencck.org/";
        private readonly HttpClient _httpClient;

        private static readonly Regex SelectRegex = new(
            @"<select[^>]*name\s*=\s*""site""[^>]*>(.*?)</select>",
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public IplistClient() => _httpClient = new HttpClient();

        /// <inheritdoc />
        public async Task<IEnumerable<SiteCategory>> GetCategoriesAsync()
        {
            var html = await _httpClient.GetStringAsync(Url);
            var selectContent = ExtractSelectContent(html);
            if (string.IsNullOrWhiteSpace(selectContent))
                throw new Exception("Не удалось найти блок <select name=\"site\"> в HTML.");

            var xmlContent = $"<root>{selectContent}</root>";
            var xDoc = XDocument.Parse(xmlContent);

            return xDoc.Descendants("optgroup")
                       .Select(optgroup => new SiteCategory
                       {
                           Name = optgroup.Attribute("label")?.Value ?? "Unknown",
                           Sites = optgroup.Elements("option")
                                           .Select(option => option.Attribute("value")?.Value)
                                           .Where(value => !string.IsNullOrEmpty(value))!
                       });
        }

        /// <inheritdoc />
        public async Task<SiteCidrInfo> GetCidrDataForSiteAsync(string site)
        {
            string url = $"{Url}?format=json&data=cidr4&site={site}";
            string jsonResponse = await _httpClient.GetStringAsync(url);

            var data = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonResponse);
            if (data == null || !data.ContainsKey(site))
                throw new Exception("Некорректный JSON или сайт не найден.");

            return new SiteCidrInfo
            {
                SiteName = site,
                Records = data[site]
                    .Select(entry =>
                    {
                        var parts = entry.Split('/');
                        if (parts.Length == 2 && int.TryParse(parts[1], out int cidr))
                        {
                            return new CidrRange
                            {
                                Original = entry,
                                Network = parts[0],
                                SubnetMask = CidrToSubnetMask(cidr)
                            };
                        }
                        return null!;
                    })
                    .Where(record => record != null)!
            };
        }

        private string ExtractSelectContent(string html)
        {
            var match = SelectRegex.Match(html);
            if (!match.Success)
                return string.Empty;

            return Regex.Replace(match.Groups[1].Value, @"&(?!amp;)", "&amp;");
        }

        private static string CidrToSubnetMask(int cidr)
        {
            uint mask = uint.MaxValue << (32 - cidr);
            return string.Join(".", BitConverter.GetBytes(mask).Reverse());
        }
    }
}
