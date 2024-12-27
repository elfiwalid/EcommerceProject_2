namespace EcommerceProject.Models
{
    public class Produit
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public decimal Prix { get; set; }
        public string ImageUrl { get; set; }
        public string Categorie { get; set; }
    }
}
