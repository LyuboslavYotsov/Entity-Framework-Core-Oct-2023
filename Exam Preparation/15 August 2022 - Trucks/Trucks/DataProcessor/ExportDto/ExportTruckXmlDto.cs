using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Trucks.Data.Models;

namespace Trucks.DataProcessor.ExportDto
{
    [XmlType(nameof(Truck))]
    public class ExportTruckXmlDto
    {
        public string RegistrationNumber { get; set; } = null!;

        public string Make { get; set; } = null!;
    }
}
