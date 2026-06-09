using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrimManagerApplication.Application.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public Rank Teamrank { get; set; }
        public Region Teamregion { get; set; }
    }
}
