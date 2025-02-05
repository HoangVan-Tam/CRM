using AutoMapper;
using Entities.DTO;
using Entities.Models;

namespace SMSDOME_Standard_Contest_BlazorServer.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Contest, ContestOverView>().ReverseMap();
            CreateMap<Contest, NewContestInfomation>().ReverseMap();
            CreateMap<ContestFieldDetails, OnlinePageInfomation>().ForPath(dest => dest.Field.IsRequired, input => input.MapFrom(i => i.IsRequired))
                .ForPath(dest => dest.Field.FieldName, input => input.MapFrom(i => i.FieldName))
                .ForPath(dest => dest.Field.FieldType, input => input.MapFrom(i => i.FieldType)).ReverseMap();
            CreateMap<ContestFieldDetails, FieldsForNewContest>().ReverseMap().ForMember(p=>p.ID, opt => opt.Ignore());
            CreateMap<ContestFields, Field>().ReverseMap();
            CreateMap<ContestFields, FieldsForNewContest>().ReverseMap();
            CreateMap<FieldsForNewContest, Field>().ReverseMap();
            CreateMap<NewRegexValidation, RegexValidation>().ReverseMap();

        }
    }
}
