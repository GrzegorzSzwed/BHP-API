using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BHP_API.DTOs;
using BHP_API.Data;
using AutoMapper;

namespace BHP_API.Mappings
{
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<Question, QuestionDTO>().ReverseMap();
            CreateMap<Question, QuestionCreateDTO>().ReverseMap();
        }
    }
}
