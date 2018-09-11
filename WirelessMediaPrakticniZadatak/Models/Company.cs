using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WirelessMediaPrakticniZadatak
{
    // As company can be manufacturer and suplier I used table Company for both
    [Table("Company")]
    public partial class Company
    {
        public Company()
        {
            ProductManufacturers = new HashSet<Product>();
            ProductSuppliers = new HashSet<Product>();
        }

        public int CompanyId { get; set; }
        [Required]
        [StringLength(40)]
        public string Name { get; set; }

        [InverseProperty("Manufacturer")]
        public ICollection<Product> ProductManufacturers { get; set; }
        [InverseProperty("Supplier")]
        public ICollection<Product> ProductSuppliers { get; set; }
    }
}
