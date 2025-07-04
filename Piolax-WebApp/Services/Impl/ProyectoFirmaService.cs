using AutoMapper;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class ProyectoFirmaService: IProyectoFirmaService
    {
        private readonly IProyectoFirmaRepository _repo;
        private readonly IMapper _mapper;

        public ProyectoFirmaService(IProyectoFirmaRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProyectoFirmaDTO>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ProyectoFirmaDTO>>(entities);
        }

        public async Task<ProyectoFirmaDTO?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<ProyectoFirmaDTO>(e);
        }

        public async Task<ProyectoFirmaDTO> CreateAsync(ProyectoFirmaDTO dto)
        {
            var entity = _mapper.Map<ProyectoFirma>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return _mapper.Map<ProyectoFirmaDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, ProyectoFirmaDTO dto)
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
