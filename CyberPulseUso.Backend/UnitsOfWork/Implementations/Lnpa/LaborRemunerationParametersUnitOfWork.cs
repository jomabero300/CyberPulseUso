using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Lnpa;

public class LaborRemunerationParametersUnitOfWork : GenericUnitOfWork<LaborRemunerationParameters>, ILaborRemunerationParametersUnitOfWork
{
    private readonly ILaborRemunerationParametersRepository _repository;

    public LaborRemunerationParametersUnitOfWork(IGenericRepository<LaborRemunerationParameters> repository, ILaborRemunerationParametersRepository modelRepository)
        : base(repository)
    {
        _repository = modelRepository;
    }

    public override Task<ActionResponse<LaborRemunerationParameters>> GetAsync(int id) => _repository.GetAsync(id);
    public override Task<ActionResponse<IEnumerable<LaborRemunerationParameters>>> GetAsync(PaginationDTO pagination) => _repository.GetAsync(pagination);
    public override Task<ActionResponse<LaborRemunerationParameters>> DeleteAsync(int id) => _repository.DeleteAsync(id);
    public Task<ActionResponse<LaborRemunerationParameters>> AddAsync(LaborRemunerationParametersDTO entity) => _repository.AddAsync(entity);
    public Task<IEnumerable<LaborRemunerationParameters>> GetComboAsync() => _repository.GetComboAsync();
    public Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => _repository.GetTotalRecordsAsync(pagination);
    public Task<ActionResponse<LaborRemunerationParameters>> UpdateAsync(LaborRemunerationParametersDTO entity) => _repository.UpdateAsync(entity);
}
