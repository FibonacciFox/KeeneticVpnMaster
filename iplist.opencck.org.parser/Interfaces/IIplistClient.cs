using System.Collections.Generic;
using System.Threading.Tasks;
using iplist.opencck.org.parser.Models;

namespace iplist.opencck.org.parser.Interfaces
{
    /// <summary>
    /// Интерфейс клиента API iplist.opencck.org.
    /// </summary>
    public interface IIplistClient
    {
        /// <summary>
        /// Получает список категорий и сайтов.
        /// </summary>
        Task<IEnumerable<SiteCategory>> GetCategoriesAsync();

        /// <summary>
        /// Получает CIDR-диапазоны для указанного сайта.
        /// </summary>
        Task<SiteCidrInfo> GetCidrDataForSiteAsync(string site);
    }
}