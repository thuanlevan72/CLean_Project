using AutoMapper;

namespace Common.Extensions
{
    public static class MappingExtensions
    {
        public static TEntity ToEntity<TEntity>(this object source, IMapper mapper)
        {
            return mapper.Map<TEntity>(source);
        }
    }
}
