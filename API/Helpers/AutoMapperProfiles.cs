using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {

            CreateMap<AppUser, MemberDTO>()
                .ForMember(dest => dest.PhotoUrl, opt => opt
                    .MapFrom(src => src.Photos.Where(x => x.isMain == true)
                                                .FirstOrDefault().url))
                .ForMember(dest => dest.Age, opt => opt
                    .MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<Photo, PhotoDTO>();
            CreateMap<MemberUpdateDTO, AppUser>();
            CreateMap<RegisterDTO, AppUser>();

            CreateMap<Message, MessageDTO>()
                .ForMember(x => x.SenderPhotoUrl, o => o
                    .MapFrom(s => s.Sender.Photos
                        .FirstOrDefault(p => p.isMain).url))
                .ForMember(x => x.RecipientPhotoUrl, o => o
                    .MapFrom(s => s.Recipient.Photos
                        .FirstOrDefault(p => p.isMain).url));

            CreateMap<DateTime,DateTime>().ConvertUsing(date=> DateTime.SpecifyKind(date, DateTimeKind.Utc));
            CreateMap<DateTime?, DateTime?>().ConvertUsing(date => date.HasValue ? DateTime.SpecifyKind(date.Value, DateTimeKind.Utc) : null);


        }
    }
}
