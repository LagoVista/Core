// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 92e082ff041f008a7f37dec8560bcce21844945d41b055d142ba90c7d0efadea
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface IRatedEntity
    {
        double? Stars { get; set; }
        int RatingsCount { get; set; }
        List<EntityRating> Ratings { get; set; } 
    }

    public class RatedEntity : IRatedEntity
    {
        public double? Stars { get; set; }
        public int RatingsCount { get; set; }
        public List<EntityRating> Ratings { get; set; } = new List<EntityRating>();
    }
}
