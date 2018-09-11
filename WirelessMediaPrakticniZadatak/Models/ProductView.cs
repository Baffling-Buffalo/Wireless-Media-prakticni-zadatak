using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WirelessMediaPrakticniZadatak.Models
{
    public class ProductView
    {
        public int? ProductId { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Names length must be between 3 and 30 characters")]
        public string Name { get; set; }
        [StringLength(70, MinimumLength = 3, ErrorMessage = "Desriptions length must be between 3 and 70 characters")]
        public string Description { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Manufacturer { get; set; }
        [Required]
        public string Supplier { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
