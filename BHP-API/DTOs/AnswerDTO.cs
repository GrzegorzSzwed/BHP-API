using BHP_API.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BHP_API.DTOs
{
    public class AnswerDTO
    {
        public int? QuestionId { get; set; }
        public int? Id { get; set; }
        public char? AnswerNo { get; set; }
        public string Description { get; set; }
        public bool isCorrect { get; set; }

        public virtual Question Question { get; set; }
    }

    public class AnswerCreateDTO
    {
        [Required]
        public int? QuestionId { get; set; }
        [Required]
        public int? Id { get; set; }
        [Required]
        public char? AnswerNo { get; set; }
        [Required]
        public string Description { get; set; }
        public bool isCorrect { get; set; }

    }

    public class AnswerUpdateDTO
    {
        [Required]
        public int? QuestionId { get; set; }
        [Required]
        public int? Id { get; set; }
        [Required]
        public char? AnswerNo { get; set; }
        [Required]
        public string Description { get; set; }
        public bool isCorrect { get; set; }
    }
}
