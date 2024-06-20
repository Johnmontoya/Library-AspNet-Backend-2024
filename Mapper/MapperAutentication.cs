using Backend.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Backend.Models;

namespace Backend.Mapper
{
    /// <summary>
    /// Mapeo de las respuestas las clases
    /// </summary>
    public class MapperAutentication: Profile
    {
        /// <summary>
        /// Mapeo de las repuestas
        /// </summary>
        public MapperAutentication()
        {
            CreateMap<Prestamo, PrestamoDtoResponse>()
                .ForMember(a => a.Autenticacion, opt => opt.MapFrom(src => src.Autenticacion))
                .ForMember(l => l.Libro, opt => opt.MapFrom(src => src.Libro))
                .ForAllMembers(p => p.ExplicitExpansion());

            CreateMap<IdentityUser, AutenticationDtoResponse>()
                .ForAllMembers(o => o.ExplicitExpansion());

            CreateMap<Libro, LibroDtoResponse>()
                .ForAllMembers(o => o.ExplicitExpansion());
        }
    }
}
