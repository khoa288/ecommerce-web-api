namespace LoginForm.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public int Price { get; set; }

        public float Rating { get; set; }

        public string Brand { get; set; } = null!;

        public string Category { get; set; } = null!;

        public Uri Thumbnail { get; set; } = null!;
    }
}
