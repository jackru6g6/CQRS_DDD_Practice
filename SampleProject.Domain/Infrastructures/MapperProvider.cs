using AutoMapper;

namespace SampleProject.Domain.Infrastructures
{
    public class MapperProvider
    {
        private static IMapper _mapper;

        public MapperProvider(IMapper mapper) => _mapper = mapper;

        //public static void Init() => _mapper = AutoMapperExtension.DomainModelToDTOMapper();

        public static TDestination Map<TDestination>(object source) => _mapper.Map<TDestination>(source);
    }
}
