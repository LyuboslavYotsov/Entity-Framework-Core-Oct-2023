using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Export
{
    public class ExportCustomerDto
    {
        public string Name { get; set; }

        public string BirthDate { get; set; }

        public  bool IsYoungDriver { get; set; }
    }
}
