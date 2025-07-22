namespace CamCare.Interfaces.Services
{
    public interface IMapper
    {
        TDestination Map<TSource, TDestination>(TSource source) where TDestination : new();
        void Map<TSource, TDestination>(TSource source, TDestination destination);
    }
}