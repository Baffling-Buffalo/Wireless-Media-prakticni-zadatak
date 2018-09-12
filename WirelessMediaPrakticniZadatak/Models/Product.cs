using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WirelessMediaPrakticniZadatak
{
    [Table("Product")]
    public partial class Product
    {
        // Annotations are put in ProductView class which is used as a model for create and edit views

        public int ProductId { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(70)]
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int ManufacturerId { get; set; }
        public int SupplierId { get; set; }
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("Products")]
        public Category Category { get; set; }
        [ForeignKey("ManufacturerId")]
        [InverseProperty("ProductManufacturers")]
        public Company Manufacturer { get; set; }
        [ForeignKey("SupplierId")]
        [InverseProperty("ProductSuppliers")]
        public Company Supplier { get; set; }
    }
}
