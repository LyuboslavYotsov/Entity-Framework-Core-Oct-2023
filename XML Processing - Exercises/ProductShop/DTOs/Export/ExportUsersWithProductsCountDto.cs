using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.DTOs.Export
{
    [XmlType("Users")]
    public class ExportUsersWithProductsCountDto
    {
        [XmlElement("count")]
        public int ProductsCount { get; set; }

        
        [XmlArray("users")]
        public ExportUsersWithProductsDto[] Users { get; set; } = null!;
    }
}
