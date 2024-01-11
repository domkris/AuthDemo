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
        }

        private void ConfigureChore()
        {
            CreateMap<ChoreRequest, Chore>();
            CreateMap<Chore, ChoreResponse>();
        }
    }
}
