namespace Web_MinimalAPI.Models
{
    //model class for - filtering data
    //deriver from queryParameter class -> Because of pagination model class
    public class ProductQueryParameter : QueryParameters
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string Sku { get; set; }=string.Empty;
        public string Name { get; set; } = string.Empty;

        public string SearchTerm { get; set; } = string.Empty;
    }
}
