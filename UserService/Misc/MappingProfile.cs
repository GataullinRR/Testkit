using Utilities.Extensions;
using AutoMapper;

namespace UserService
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            typeof(MappingProfile).Assembly.FindAndRegisterMappingsTo(this);
        }
    }
}
