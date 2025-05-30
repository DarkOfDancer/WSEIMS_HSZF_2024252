using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSEIMS_HSZF_2024252.Model
{
    public class ExpensEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? category { get; set; }
        public int amount { get; set; }
        public string? approvalStatus { get; set; }
        public DateTime expenseDate { get; set; }

        public string? BudgetId { get; set; }

        public virtual BudgetEntity? Budget { get; set; }

        public virtual ICollection<SubcategoryEntity>? subcategory { get; set; }
    }
}

