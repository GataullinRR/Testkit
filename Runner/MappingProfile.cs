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
            CreateMap<CSRegisterRequest, RegisterRequest>();
            CreateMap<CSLoginRequest, LoginRequest>();

            var types = Assembly.GetExecutingAssembly().DefinedTypes;
            var subjects = types
                .Select(t => (  T: t, 
                                From: t.GetCustomAttributes<AutoMapFrom>().NullToEmpty(), 
                                To: t.GetCustomAttributes<AutoMapTo>().NullToEmpty()))
                .Where(i => i.From != null || i.To != null);
            foreach (var subject in subjects)
            {
                foreach (var from in subject.From)
                {
                    CreateMap(from.From, subject.T);
                }
                foreach (var to in subject.To)
                {
                    CreateMap(subject.T, to.To);
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AutoMapFrom : Attribute
    {
        public AutoMapFrom(Type from)
        {
            From = from;
        }

        public Type From { get; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AutoMapTo : Attribute
    {
        public AutoMapTo(Type to)
        {
            To = to;
        }

        public Type To { get; }
    }
}
