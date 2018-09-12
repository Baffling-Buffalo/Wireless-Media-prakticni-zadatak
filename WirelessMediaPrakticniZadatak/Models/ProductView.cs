using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WirelessMediaPrakticniZadatak.Models
{  
    // Used as a model for create and edit views
    public class ProductView
    {
        public int? ProductId { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Names length must be between 3 and 30 characters")]
        public string Name { get; set; }
        [StringLength(70, MinimumLength = 3, ErrorMessage = "Desriptions length must be between 3 and 70 characters")]
        public string Description { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Category names length must be between 3 and 30 characters")]
        public string Category { get; set; }
        [Required]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Min 2 and max 40 characters")]
        public string Manufacturer { get; set; }
        [Required]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Min 2 and max 40 characters")]
        public string Supplier { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
