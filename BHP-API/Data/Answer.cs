using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BHP_API.Data
{
    [Table("Answer")]
    public class Answer
    {
        public int? QuestionId { get; set; }
        public int? Id { get; set; }
        public char? AnswerNo { get; set; }
        public string Description { get; set; }
        public bool isCorrect { get; set; }

        public virtual Question  Question { get; set; }
    }
}
