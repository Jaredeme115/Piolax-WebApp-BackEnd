using AutoMapper;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class EtapaActividadService: IEtapaActividadService
    {
        private readonly IEtapaActividadRepository _repo;
        private readonly IMapper _mapper;

        public EtapaActividadService(IEtapaActividadRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EtapaActividadDTO>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<EtapaActividadDTO>>(entities);
        }

        public async Task<EtapaActividadDTO?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<EtapaActividadDTO>(e);
        }

        public async Task<EtapaActividadDTO> CreateAsync(EtapaActividadDTO dto)
        {
            var entity = _mapper.Map<EtapaActividad>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return _mapper.Map<EtapaActividadDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, EtapaActividadDTO dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            _mapper.Map(dto, entity);
            _repo.Update(entity);
            return await _repo.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            _repo.Remove(entity);
            return await _repo.SaveChangesAsync();
        }
    }
}
