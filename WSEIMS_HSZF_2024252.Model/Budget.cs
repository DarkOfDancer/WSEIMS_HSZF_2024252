using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSEIMS_HSZF_2024252.Model
{
    public class Budget
    {
        public Budget()
        {
            expenses = new HashSet<Expens>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        // Az idegen kulcs a TeamEntity-hez
        public string TeamEntityId { get; set; }

        // Navigációs tulajdonság a TeamEntity kapcsolat létrehozásához
        public virtual TeamEntity TeamEntity { get; set; }

        // A teljes költségvetés összege
        public int totalBudget { get; set; }

        // Az Expense-k tárolása
        public virtual ICollection<Expens> expenses { get; set; }
    }

}
