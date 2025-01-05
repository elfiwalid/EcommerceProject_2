namespace EcommerceProject.Models
{
    public class CartItem
    {
        public int ProduitId { get; set; }
        public string Nom { get; set; }
        public decimal Prix { get; set; }
        public int Quantite { get; set; }

        
    }
}
