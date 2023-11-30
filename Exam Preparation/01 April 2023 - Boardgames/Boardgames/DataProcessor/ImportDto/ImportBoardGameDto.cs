﻿using Boardgames.Data.Models;
using Boardgames.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType(nameof(Boardgame))]
    public class ImportBoardGameDto
    {
        [Required]
        [MaxLength(20)]
        [MinLength(10)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(1.0, 10.0)]
        public double Rating { get; set; }

        [Required]
        [Range(2018, 2023)]
        public int YearPublished { get; set; }

        [Required]
        [Range(0, 4)]
        public int CategoryType { get; set; }

        [Required]
        public string Mechanics { get; set; } = null!;
    }
}
