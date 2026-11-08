using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Lnpa;

public class LaborRemunerationParametersOtherValueUnitOfWork : GenericUnitOfWork<LaborRemunerationParametersOtherValue>, ILaborRemunerationParametersOtherValueUnitOfWork
{
    private readonly ILaborRemunerationParametersOtherValueRepository _repository;

    public LaborRemunerationParametersOtherValueUnitOfWork(IGenericRepository<LaborRemunerationParametersOtherValue> repository, ILaborRemunerationParametersOtherValueRepository modelRepository)
        : base(repository)
    {
        _repository = modelRepository;
    }

    public override async Task<ActionResponse<IEnumerable<LaborRemunerationParametersOtherValue>>> GetAsync() => await _repository.GetAsync();

    public override Task<ActionResponse<LaborRemunerationParametersOtherValue>> GetAsync(int id) => _repository.GetAsync(id);
    public override Task<ActionResponse<IEnumerable<LaborRemunerationParametersOtherValue>>> GetAsync(PaginationDTO pagination) => _repository.GetAsync(pagination);
    public override Task<ActionResponse<LaborRemunerationParametersOtherValue>> DeleteAsync(int id) => _repository.DeleteAsync(id);
    public Task<ActionResponse<IEnumerable<LaborRemunerationParametersOtherValueCRUDDTO>>> AddAsync(IEnumerable<LaborRemunerationParametersOtherValueCRUDDTO> entity) => _repository.AddAsync(entity);
    public Task<IEnumerable<LaborRemunerationParametersOtherValue>> GetComboAsync() => _repository.GetComboAsync();
    public Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => _repository.GetTotalRecordsAsync(pagination);
    public Task<ActionResponse<LaborRemunerationParametersOtherValue>> UpdateAsync(LaborRemunerationParametersOtherValueDTO entity) => _repository.UpdateAsync(entity);
}
