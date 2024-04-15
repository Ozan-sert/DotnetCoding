

using System.ComponentModel.DataAnnotations;

    namespace DotnetCoding.Core.Models
    {
        public class Product
        {
            [Required]
            public int Id { get; set; }
            public string Name { get; set; } 
            public string Description { get; set; }
            public int Price { get; set; }
            public bool IsActive { get; set; }
            public DateTime PostedDate { get; set; }
            
      
        }
    }
