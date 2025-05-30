using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSEIMS_HSZF_2024252.Model
{

    public class TeamEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [StringLength(100)]
        public string? teamName { get; set; }
        public int year { get; set; }
        [StringLength(100)]
        public string? headquarters { get; set; } = "Unknown";
        [StringLength(100)]
        public string? teamPrincipal { get; set; }
        public int constructorsChampionshipWins { get; set; }
        public virtual BudgetEntity? budget { get; set; } 
    }

}
