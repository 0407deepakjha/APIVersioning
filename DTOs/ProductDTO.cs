namespace APIVersioning.DTOs
{
    // Request DTO for creating a product (v1)
    public class ProductCreateRequestV1
    {
        public string Name { get; set; } = null!;
    }

    // Response DTO for product info (v1)
    public class ProductResponseV1
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    // Request DTO for creating a product (v2)
    public class ProductCreateRequestV2
    {
        public string Name { get; set; } = null!;
        public double Price { get; set; }
    }

    // Response DTO for product info (v2)
    public class ProductResponseV2
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public double Price { get; set; }
    }
}