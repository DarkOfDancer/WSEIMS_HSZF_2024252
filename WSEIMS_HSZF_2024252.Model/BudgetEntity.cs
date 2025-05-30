using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSEIMS_HSZF_2024252.Model
{
    public class BudgetEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? TeamEntityId { get; set; }

        public virtual TeamEntity? TeamEntity { get; set; }

        public int totalBudget { get; set; }

        public virtual ICollection<ExpensEntity>? expenses { get; set; }
    }

}
