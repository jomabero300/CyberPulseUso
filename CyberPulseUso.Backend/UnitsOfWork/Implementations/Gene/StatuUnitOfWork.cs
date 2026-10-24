using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Gene;

public class StatuUnitOfWork : GenericUnitOfWork<Statu>, IStatuUnitOfWork
{
    private readonly IStatuRepository _statuRepository;

    public StatuUnitOfWork(IGenericRepository<Statu> repository, IStatuRepository statuRepository) : base(repository)
    {
        _statuRepository = statuRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Statu>>> GetAsync()
        => await _statuRepository.GetAsync();

    public async Task<ActionResponse<Statu>> GetAsync(string name, int nivel)
        => await _statuRepository.GetAsync(name, nivel);

    public override async Task<ActionResponse<IEnumerable<Statu>>> GetAsync(PaginationDTO pagination)
        => await _statuRepository.GetAsync(pagination);

    public override async Task<ActionResponse<Statu>> GetAsync(int id)
        => await _statuRepository.GetAsync(id);

    public override async Task<ActionResponse<Statu>> DeleteAsync(int id)
        => await _statuRepository.DeleteAsync(id);

    // El código recibido no implementa AddAsync/UpdateAsync específicos para Statu.
    public async Task<IEnumerable<Statu>> GetComboAsync(int id)
        => await _statuRepository.GetComboAsync(id);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
        => await _statuRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<Statu>> UpdateAsync(StatuDTO entity) 
        => await _statuRepository.UpdateAsync(entity);

    public async Task<ActionResponse<Statu>> AddAsync(StatuDTO entity) 
        => await _statuRepository.AddAsync(entity);
}
