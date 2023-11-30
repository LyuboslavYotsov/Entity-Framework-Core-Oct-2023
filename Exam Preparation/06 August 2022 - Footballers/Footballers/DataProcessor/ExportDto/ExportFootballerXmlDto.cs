﻿using Footballers.Data.Models;
using Footballers.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ExportDto
{
    [XmlType(nameof(Footballer))]
    public class ExportFootballerXmlDto
    {
        public string Name { get; set; } = null!;

        public string Position { get; set; } = null!;

    }
}
