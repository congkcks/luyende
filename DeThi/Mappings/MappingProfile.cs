using AutoMapper;
using DeThi.DTO;
using DeThi.Models;

namespace EnglishTestService.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Test, TestDto>();
        CreateMap<Question, QuestionDto>();
        CreateMap<Option, OptionDto>();
    }
}
