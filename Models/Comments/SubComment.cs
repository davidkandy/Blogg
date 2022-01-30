using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blogg.Models.Comments
{
    public class SubComment : Comments
    {
        public int MainCommentId { get; set; }
    }
}
