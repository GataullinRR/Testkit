using AutoMapper;
using System;
using System.Linq;
using System.Reflection;
using UserService.API;
using Utilities.Extensions;

namespace Runner
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            typeof(MappingProfile).Assembly.FindAndRegisterMappingsTo(this);
            typeof(UserService.API.UserService).Assembly.FindAndRegisterMappingsTo(this);
        }
    }
}
