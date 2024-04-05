using AuthDemo.Contracts.DataTransferObjects.Request;
using AuthDemo.Contracts.DataTransferObjects.Response;
using AuthDemo.Infrastructure.Entities;
using AutoMapper;

namespace AuthDemo.Web.AutoMapper
{
    internal sealed class AuthDemoMappingProfile : Profile
    {
        public AuthDemoMappingProfile()
        {
            // Chore
            ConfigureChore();

            // User
            ConfigureUser();
        }

        private void ConfigureChore()
        {
            CreateMap<ChoreEditRequest, Chore>();
            CreateMap<Chore, ChoreResponse>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));
        }

        private void ConfigureUser()
        {
            CreateMap<User, UserResponse>();
            CreateMap<User, SimpleUserResponse>();
        }

    }
}
