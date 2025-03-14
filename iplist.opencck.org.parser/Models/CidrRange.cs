namespace iplist.opencck.org.parser.Models
{
    /// <summary>
    /// Отдельная запись CIDR-диапазона.
    /// </summary>
    public class CidrRange
    {
        /// <summary>
        /// Исходная запись в формате CIDR.
        /// </summary>
        public string Original { get; set; } = string.Empty;

        /// <summary>
        /// Сетевой адрес.
        /// </summary>
        public string Network { get; set; } = string.Empty;

        /// <summary>
        /// Маска подсети в формате 255.255.255.0.
        /// </summary>
        public string SubnetMask { get; set; } = string.Empty;
    }
}