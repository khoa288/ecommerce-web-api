﻿using EcommerceWebApi.Utilities;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApi.Entities
{
    public class Product
    {
        [Required(ErrorMessage = "{0} is required")]
        public int Id { get; set; }

        [Searchable]
        [Required(ErrorMessage = "{0} is required")]
        public string Title { get; set; } = null!;

        [Sortable]
        [Required(ErrorMessage = "{0} is required")]
        public float Price { get; set; }

        [Sortable]
        [Range(0, 5, ErrorMessage = "{0} must be between {1} and {2}")]
        public float Rating { get; set; }

        [Searchable]
        [Required(ErrorMessage = "{0} is required")]
        public string Brand { get; set; } = null!;

        [Searchable]
        public string Category { get; set; } = null!;

        public Uri Thumbnail { get; set; } = null!;

        [Sortable]
        [Required(ErrorMessage = "{0} is required")]
        public int Quantity { get; set; }
    }
}
