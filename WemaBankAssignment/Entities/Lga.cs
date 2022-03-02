using System;
using System.Collections.Generic;
using System.Text;
using WemaBankAssignment.Entities.common;

namespace WemaBankAssignment.Entities
{
    public partial class Lga : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public State State { get; set; }
        public int StateId { get; set; }
    }
}
