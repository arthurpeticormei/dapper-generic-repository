using AutoMapper;
using Domain.Entities;
using Domain.Models;

namespace Domain.Config
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig()
        {
            CreateMap<Entity, Model>().ReverseMap();
        }
    }
}
