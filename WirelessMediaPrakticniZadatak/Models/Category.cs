using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WirelessMediaPrakticniZadatak
{
    [Table("Category")]
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(70)]
        public string Description { get; set; }

        [InverseProperty("Category")]
        public ICollection<Product> Products { get; set; }
    }
}
