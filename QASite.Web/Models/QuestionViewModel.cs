using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QASite.Data;

namespace QASite.Web.Models
{
    public class QuestionViewModel
    {
        public Question Question { get; set; }
        public bool CantLike { get; set; }
    }
}
