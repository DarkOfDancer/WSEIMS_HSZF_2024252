using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSEIMS_HSZF_2024252.Model
{
    public class Expens
    {
        public Expens(string category, int amount, string approvalStatus, DateTime expenseDate)
        {
            Id = Guid.NewGuid().ToString();
            this.category = category;
            this.amount = amount;
            this.approvalStatus = approvalStatus;
            this.expenseDate =expenseDate;
            subcategory = new HashSet<Subcategory>();
        }
        public Expens()
        {
            subcategory = new HashSet<Subcategory>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        // Kategória, összeg, stb.
        public string? category { get; set; }
        public int amount { get; set; }
        public string? approvalStatus { get; set; }
        public DateTime expenseDate { get; set; }

        // Idegen kulcs kapcsolódik a Budget entitáshoz
        public string BudgetId { get; set; }

        // Navigációs tulajdonság
        public virtual Budget Budget { get; set; }

        // Subcategory-k kapcsolatai
        public virtual ICollection<Subcategory> subcategory { get; set; }
    }
}

