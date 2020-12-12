using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BHP_API.DTOs
{
    public class QuestionDTO
    {
        public int Id { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string url { get; set; }
    }

    public class QuestionCreateDTO
    {
        [Required]
        public string type { get; set; }
        [Required]
        public string description { get; set; }

        public string url { get; set; }
    }

    public class QuestionUpdateDTO
    {
        public int Id { get; set; }
        [Required]
        public string type { get; set; }
        [Required]
        public string description { get; set; }

        public string url { get; set; }
    }
}
