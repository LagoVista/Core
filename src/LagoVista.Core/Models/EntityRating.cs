// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 530603701bd095f9b7a145bc28f3cc692daf231a335d2844f43935eeb6daf267
// IndexVersion: 2
// --- END CODE INDEX META ---
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
