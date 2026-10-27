using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Lnpa;

public class LaborRemunerationParametersValueUnitOfWork : GenericUnitOfWork<LaborRemunerationParametersValue>, ILaborRemunerationParametersValueUnitOfWork
{
    private readonly ILaborRemunerationParametersValueRepository _repository;

    public LaborRemunerationParametersValueUnitOfWork(IGenericRepository<LaborRemunerationParametersValue> repository, ILaborRemunerationParametersValueRepository modelRepository)
        : base(repository)
    {
        _repository = modelRepository;
    }

    public override Task<ActionResponse<LaborRemunerationParametersValue>> GetAsync(int id) => _repository.GetAsync(id);
    public override Task<ActionResponse<IEnumerable<LaborRemunerationParametersValue>>> GetAsync(PaginationDTO pagination) => _repository.GetAsync(pagination);
    public override Task<ActionResponse<LaborRemunerationParametersValue>> DeleteAsync(int id) => _repository.DeleteAsync(id);
    public Task<ActionResponse<LaborRemunerationParametersValue>> AddAsync(LaborRemunerationParametersValueDTO entity) => _repository.AddAsync(entity);
    public Task<IEnumerable<LaborRemunerationParametersValue>> GetComboAsync() => _repository.GetComboAsync();
    public Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => _repository.GetTotalRecordsAsync(pagination);
    public Task<ActionResponse<LaborRemunerationParametersValue>> UpdateAsync(LaborRemunerationParametersValueDTO entity) => _repository.UpdateAsync(entity);
}
