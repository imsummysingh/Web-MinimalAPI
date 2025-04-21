using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Web_MinimalAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Sku {  get; set; }=string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }

        [Required]
        public int CategoryId {  get; set; }

        //virtual -> want to point to the Category from the Product.
        [JsonIgnore]
        public virtual Category? Category { get; set; }
    }
}
