using AutoMapper;
using Entities.DTO;
using Entities.Helper;
using Entities.Models;

namespace SMSDOME_Standard_Contest_BlazorServer.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Contest, ContestOverView>().ReverseMap();
            CreateMap<Contest, NewContestInfomation>().ReverseMap();
            CreateMap<ContestFieldDetails, FormField>().ForPath(dest => dest.Pattern, input => input.MapFrom(i => i.RegexValidation.Pattern)).ReverseMap();
            CreateMap<ContestFieldDetails, FieldsForNewContest>().ReverseMap().ForMember(p => p.FieldDetailID, opt => opt.Ignore());
//CreateMap<ContestFields, Field>().ReverseMap();
            //CreateMap<FieldsForNewContest, Field>().ReverseMap();
            CreateMap<NewRegexValidation, RegexValidation>().ReverseMap();

        }
    }
}
