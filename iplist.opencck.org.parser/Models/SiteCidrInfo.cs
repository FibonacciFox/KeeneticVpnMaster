using System.Collections.Generic;
using System.Linq;

namespace iplist.opencck.org.parser.Models
{
    /// <summary>
    /// Описывает информацию о CIDR-диапазонах для конкретного сайта.
    /// </summary>
    public class SiteCidrInfo
    {
        /// <summary>
        /// Название сайта.
        /// </summary>
        public string SiteName { get; set; } = string.Empty;

        /// <summary>
        /// Записи CIDR.
        /// </summary>
        public IEnumerable<CidrRange> Records { get; set; } = Enumerable.Empty<CidrRange>();
    }
}