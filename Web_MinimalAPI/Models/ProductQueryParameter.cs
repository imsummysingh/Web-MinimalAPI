namespace Web_MinimalAPI.Models
{
    //model class for - filtering data
    //deriver from queryParameter class -> Because of pagination model class
    public class ProductQueryParameter : QueryParameters
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
