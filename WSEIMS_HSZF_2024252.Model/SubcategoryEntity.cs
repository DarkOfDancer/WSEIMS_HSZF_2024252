using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace WSEIMS_HSZF_2024252.Model
{
    public class SubcategoryEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        // A subcategory neve és összege
        [StringLength(100)]
        public string? name { get; set; }
        public int amount { get; set; }
    }
}
