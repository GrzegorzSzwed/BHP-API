using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BHP_API.Data
{
    [Table("Question")]
    public partial class Question
    {
        public int Id { get; set; }

        public string type { get; set; }
        public string description { get; set; }

        public string url { get; set; }

        public virtual IList<Answer> Answers { get; set; }

    }
}
