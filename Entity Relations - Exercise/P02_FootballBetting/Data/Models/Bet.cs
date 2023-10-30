﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_FootballBetting.Data.Models
{
    public class Bet
    {
        [Key]
        public int BetId { get; set; }

        public decimal Amount { get; set; }

        [MaxLength(50)]
        public string Prediction { get; set; } = null!;

        public DateTime DateTime { get; set; }

        public int UserId { get; set; }

        public int GameId { get; set; }

        [ForeignKey(nameof(GameId))]
        public virtual Game Game { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
