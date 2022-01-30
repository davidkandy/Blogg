using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blogg.Models.Comments
{
    public class MainComment : Comments
    {
        public List<SubComment> SubComments { get; set; }
    }
}
