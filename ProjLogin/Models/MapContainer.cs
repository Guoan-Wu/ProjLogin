using AutoMapper;
using AutoMapper.Configuration.Conventions;
using ProjLogin.DTO;

namespace ProjLogin.Models
{
    public class MapContainer
    {
        public static MapperConfiguration? _config;
        public static MapperConfiguration Configure()
        {
            if(_config != null) return _config;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegisterDTO, User>();
                cfg.CreateMap<LoginDTO, User>();
            });

            _config = config;
            return _config;
        }
        public static  TDestination Map<TSource,TDestination>(TSource source)
        {
            IMapper iMapper = Configure().CreateMapper();            
            TDestination dest = iMapper.Map<TSource, TDestination>(source);
            return dest;
            
        }
        public static void MapTest()
        {
            
            RegisterDTO source1 = new RegisterDTO { Email = "abc@12.nz", Password = "qwert12345", Name = "Name1" };

            var dest1 = Map<RegisterDTO, User>(source1);
            Console.WriteLine(dest1);

            LoginDTO source = new LoginDTO { Email = "12a@gmail.nz", Password = "1234er" };
            var dest2 = Map<LoginDTO, User>(source);
            Console.WriteLine(dest2);
        }
    }
}
