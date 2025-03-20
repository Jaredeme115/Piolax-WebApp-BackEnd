using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class ObservacionesMPService(IObservacionesMPRepository repository) : IObservacionesMPService
    {
        private readonly IObservacionesMPRepository _repository = repository;

        public async Task<ObservacionesMPDTO> AgregarObservacion(ObservacionesMPCrearDTO observacionDTO)
        {
            var nuevaObservacion = new ObservacionesMP
            {
                idMP = observacionDTO.idMP,
                observacion = observacionDTO.observacion,
                fechaObservacion = observacionDTO.fechaObservacion
            };

            var resultado = await _repository.AgregarObservacion(nuevaObservacion);

            return new ObservacionesMPDTO
            {
                idObservacionMP = resultado.idObservacionMP,
                idMP = resultado.idMP,
                observacion = resultado.observacion,
                fechaObservacion = resultado.fechaObservacion
            };
        }

        public async Task<IEnumerable<ObservacionesMPDTO>> ObtenerObservacionesPorMP(int idMP)
        {
            var resultados = await _repository.ObtenerObservacionesPorMP(idMP);

            return resultados.Select(obs => new ObservacionesMPDTO
            {
                idObservacionMP = obs.idObservacionMP,
                idMP = obs.idMP,
                observacion = obs.observacion,
                fechaObservacion = obs.fechaObservacion
            });
        }

        public async Task<ObservacionesMPDTO?> EliminarObservacion(int idObservacionMP)
        {
            var resultado = await _repository.EliminarObservacion(idObservacionMP);
            if (resultado == null)
                return null;

            return new ObservacionesMPDTO
            {
                idObservacionMP = resultado.idObservacionMP,
                idMP = resultado.idMP,
                observacion = resultado.observacion,
                fechaObservacion = resultado.fechaObservacion
            };
        }
    }
}
