using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_FootballBetting.Data.Models
{
    public class Player
    {
        [Key]
        public int PlayerId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public int SquadNumber { get; set; }

        public int TeamId { get; set; }

        public int TownId {  get; set; }

        public int PositionId { get; set; }

        public bool IsInjured { get; set; }

        [ForeignKey(nameof(PositionId))]
        public virtual Position Position { get; set; } = null!;

        [ForeignKey(nameof(TeamId))]
        public virtual Team Team { get; set; } = null!;

        [ForeignKey(nameof(TownId))]
        public virtual Town Town { get; set; } = null!;

        public virtual ICollection<PlayerStatistic> PlayersStatistics { get; set; } = new List<PlayerStatistic>();



    }
}
