using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public class EntityRating
    {
            public int Stars { get; set; }

            public EntityHeader User { get; set; }
            public string TimeStamp { get; set; }
    }
 }
