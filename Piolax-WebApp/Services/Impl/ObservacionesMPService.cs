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
                idHistoricoMP = observacionDTO.idHistoricoMP,
                observacion = observacionDTO.observacion,
                fechaObservacion = observacionDTO.fechaObservacion
            };

            var resultado = await _repository.AgregarObservacion(nuevaObservacion);

            return new ObservacionesMPDTO
            {
                idObservacionMP = resultado.idObservacionMP,
                idHistoricoMP = resultado.idHistoricoMP,
                observacion = resultado.observacion,
                fechaObservacion = resultado.fechaObservacion
            };
        }

        public async Task<IEnumerable<ObservacionesMPDTO>> ObtenerObservacionesPorMP(int idHistoricoMP)
        {
            var resultados = await _repository.ObtenerObservacionesPorMP(idHistoricoMP);

            return resultados.Select(obs => new ObservacionesMPDTO
            {
                idObservacionMP = obs.idObservacionMP,
                idHistoricoMP = obs.idHistoricoMP,
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
                idHistoricoMP = resultado.idHistoricoMP,
                observacion = resultado.observacion,
                fechaObservacion = resultado.fechaObservacion
            };
        }
    }
}
