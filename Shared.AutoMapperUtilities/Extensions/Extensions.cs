using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Extensions
{
    public static class Extensions
    {
        public static void FindAndRegisterMappingsTo(this Assembly assembly, Profile profile)
        {
            var types = assembly.DefinedTypes;
            var subjects = types
                .Select(t => (T: t,
                                From: t.GetCustomAttributes<AutoMapFromAttribute>().NullToEmpty(),
                                To: t.GetCustomAttributes<AutoMapToAttribute>().NullToEmpty()))
                .Where(i => i.From != null || i.To != null);
            foreach (var subject in subjects)
            {
                foreach (var from in subject.From)
                {
                    profile.CreateMap(from.From, subject.T);
                }
                foreach (var to in subject.To)
                {
                    profile.CreateMap(subject.T, to.To);
                }
            }
        }
    }
}
