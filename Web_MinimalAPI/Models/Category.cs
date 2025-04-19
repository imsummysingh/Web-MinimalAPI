namespace Web_MinimalAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        //virtual->Because accessing another class and making relationship with it
        public virtual List<Product> Products { get; set; }
    }
}
