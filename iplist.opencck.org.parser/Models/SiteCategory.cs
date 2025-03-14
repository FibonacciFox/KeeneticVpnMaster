using System.Collections.Generic;
using System.Linq;

namespace iplist.opencck.org.parser.Models
{
    /// <summary>
    /// Представляет категорию сайтов и список сайтов в этой категории.
    /// </summary>
    public class SiteCategory
    {
        /// <summary>
        /// Название категории.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Список сайтов в категории.
        /// </summary>
        public IEnumerable<string> Sites { get; set; } = Enumerable.Empty<string>();
    }
}