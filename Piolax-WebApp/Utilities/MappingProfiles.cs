using AutoMapper;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;

namespace Piolax_WebApp.Utilities
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            // Proyecto ↔ ProyectoDto
            CreateMap<Proyecto, ProyectoDTO>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.idProyecto))
                .ForMember(d => d.KeyPersonId, o => o.MapFrom(s => s.idKeyPerson))
                .ReverseMap()
                .ForMember(e => e.idProyecto, o => o.MapFrom(d => d.Id ?? 0))
                .ForMember(e => e.idKeyPerson, o => o.MapFrom(d => d.KeyPersonId));

            // ProyectoEtapa ↔ ProyectoEtapaDto
            CreateMap<ProyectoEtapa, ProyectoEtapaDTO>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.idEtapa))
                .ForMember(d => d.ProyectoId, o => o.MapFrom(s => s.idProyecto))
                .ReverseMap()
                .ForMember(e => e.idEtapa, o => o.MapFrom(d => d.Id ?? 0))
                .ForMember(e => e.idProyecto, o => o.MapFrom(d => d.ProyectoId));

            // EtapaActividad ↔ EtapaActividadDto
            CreateMap<EtapaActividad, EtapaActividadDTO>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.idActividad))
                .ForMember(d => d.EtapaId, o => o.MapFrom(s => s.idEtapa))
                .ForMember(d => d.EmpleadoMarcaId, o => o.MapFrom(s => s.idEmpleadoMarca))
                .ReverseMap()
                .ForMember(e => e.idActividad, o => o.MapFrom(d => d.Id ?? 0))
                .ForMember(e => e.idEtapa, o => o.MapFrom(d => d.EtapaId))
                .ForMember(e => e.idEmpleadoMarca, o => o.MapFrom(d => d.EmpleadoMarcaId));

            // EtapaComentario ↔ EtapaComentarioDto
            CreateMap<EtapaComentario, EtapaComentarioDTO>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.idComentario))
                .ForMember(d => d.EtapaId, o => o.MapFrom(s => s.idEtapa))
                .ForMember(d => d.EmpleadoId, o => o.MapFrom(s => s.idEmpleado))
                .ReverseMap()
                .ForMember(e => e.idComentario, o => o.MapFrom(d => d.Id ?? 0))
                .ForMember(e => e.idEtapa, o => o.MapFrom(d => d.EtapaId))
                .ForMember(e => e.idEmpleado, o => o.MapFrom(d => d.EmpleadoId));

            // ProyectoFirma ↔ ProyectoFirmaDto
            CreateMap<ProyectoFirma, ProyectoFirmaDTO>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.idFirma))
                .ForMember(d => d.ProyectoId, o => o.MapFrom(s => s.idProyecto))
                .ForMember(d => d.EmpleadoId, o => o.MapFrom(s => s.idEmpleado))
                .ReverseMap()
                .ForMember(e => e.idFirma, o => o.MapFrom(d => d.Id ?? 0))
                .ForMember(e => e.idProyecto, o => o.MapFrom(d => d.ProyectoId))
                .ForMember(e => e.idEmpleado, o => o.MapFrom(d => d.EmpleadoId));
        }
    }
}
