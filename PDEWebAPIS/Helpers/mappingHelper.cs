using AutoMapper;
using System.Linq.Expressions;

namespace PDEWebAPIS.Helpers
{
    public static class mappingHelper
    {
        public static IMappingExpression<TSource, TDestination> MapNA<TSource, TDestination>(
           this IMappingExpression<TSource, TDestination> map,
           Expression<Func<TDestination, object>> member)
        {
            return map.ForMember(member, opt => opt.MapFrom(_ => "NA"));
        }
        public static IMappingExpression<TSource, TDestination> MapZero<TSource, TDestination>(
           this IMappingExpression<TSource, TDestination> map,
           Expression<Func<TDestination, object>> member)
        {
            return map.ForMember(member, opt => opt.MapFrom(_ => 0));
        }
    }
}
