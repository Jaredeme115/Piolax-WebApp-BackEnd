using AutoMapper;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class ProyectoService : IProyectoService
    {
        private readonly IProyectoRepository _repo;
        private readonly IMapper _mapper;

        public ProyectoService(IProyectoRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProyectoDTO>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ProyectoDTO>>(entities);
        }

        public async Task<ProyectoDTO?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<ProyectoDTO>(e);
        }

        public async Task<ProyectoDTO> CreateAsync(ProyectoDTO dto)
        {
            var entity = _mapper.Map<Proyecto>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return _mapper.Map<ProyectoDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, ProyectoDTO dto)
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
