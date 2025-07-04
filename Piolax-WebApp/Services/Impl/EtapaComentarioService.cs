using AutoMapper;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Repositories;

namespace Piolax_WebApp.Services.Impl
{
    public class EtapaComentarioService: IEtapaComentarioService
    {
        private readonly IEtapaComentarioRepository _repo;
        private readonly IMapper _mapper;

        public EtapaComentarioService(IEtapaComentarioRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EtapaComentarioDTO>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<EtapaComentarioDTO>>(entities);
        }

        public async Task<EtapaComentarioDTO?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<EtapaComentarioDTO>(e);
        }

        public async Task<EtapaComentarioDTO> CreateAsync(EtapaComentarioDTO dto)
        {
            var entity = _mapper.Map<EtapaComentario>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return _mapper.Map<EtapaComentarioDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, EtapaComentarioDTO dto)
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
