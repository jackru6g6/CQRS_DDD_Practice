using AutoMapper;

namespace SampleProject.Domain.Extensions
{
    /// <summary>
    /// AutoMapperExtension
    /// </summary>
    public class AutoMapperExtension
    {
        /// <summary>
        /// Domains the model to dto mapper.
        /// </summary>
        /// <returns></returns>
        public static IMapper DomainModelToDTOMapper()
        {
            var mappingConfig = new MapperConfiguration(cfg => cfg.AddProfile<ModelMappings>());
            return mappingConfig.CreateMapper();
        }
    }

    /// <summary>
    /// ModelMappings
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class ModelMappings : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelMappings"/> class.
        /// </summary>
        public ModelMappings()
        {
            OrderMap();
        }

        /// <summary>
        /// Orders the map.
        /// </summary>
        private void OrderMap()
        {
            //CreateMap<Order, OrderResponse>()
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OrderId))
            //    .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.OrderAmount));
        }
    }
}
