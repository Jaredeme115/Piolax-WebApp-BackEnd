using AutoMapper;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class ProyectoEtapaService: IProyectoEtapaService
    {
        private readonly IProyectoEtapaRepository _repo;
        private readonly IMapper _mapper;

        public ProyectoEtapaService(IProyectoEtapaRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProyectoEtapaDTO>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ProyectoEtapaDTO>>(entities);
        }

        public async Task<ProyectoEtapaDTO?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<ProyectoEtapaDTO>(e);
        }

        public async Task<ProyectoEtapaDTO> CreateAsync(ProyectoEtapaDTO dto)
        {
            var entity = _mapper.Map<ProyectoEtapa>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return _mapper.Map<ProyectoEtapaDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, ProyectoEtapaDTO dto)
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
