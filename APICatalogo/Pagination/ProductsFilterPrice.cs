namespace APICatalogo.Pagination;

public class ProductsFilterPrice : QueryStringParameters
{
    public decimal? Price { get; set; }
    public string? PriceCriterion { get; set; } // "maior", "menor" ou "igual"

}
