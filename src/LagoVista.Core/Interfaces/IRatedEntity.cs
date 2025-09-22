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
