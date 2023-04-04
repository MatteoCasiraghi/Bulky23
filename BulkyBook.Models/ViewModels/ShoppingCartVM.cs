namespace BulkyBook.Models.ViewModels
{
    public class ShoppingCartVM
    {
		public IEnumerable<ShoppingCart> ListCart { get; set; } = null!;
		//CartTotal è all'interno di OrderHeader con il nome di OrderTotal e può essere rimosso
		//public double CartTotal { get; set; }
		public OrderHeader OrderHeader { get; set; } = null!;

	}
}
