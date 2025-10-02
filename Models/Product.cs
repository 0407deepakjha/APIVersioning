namespace APIVersioning.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // Added in version 2
        public double? Price { get; set; }
    }
}