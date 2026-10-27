using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Gene;

public class LogAuditoriaUnitOfWork : GenericUnitOfWork<LogAuditoria>, ILogAuditoriaUnitOfWork
{
    private readonly ILogAuditoriaRepository _repository;

    public LogAuditoriaUnitOfWork(IGenericRepository<LogAuditoria> repository, ILogAuditoriaRepository modelRepository)
        : base(repository)
    {
        _repository = modelRepository;
    }

    public override Task<ActionResponse<LogAuditoria>> GetAsync(int id) => _repository.GetAsync(id);
    public override Task<ActionResponse<IEnumerable<LogAuditoria>>> GetAsync(PaginationDTO pagination) => _repository.GetAsync(pagination);
    public override Task<ActionResponse<LogAuditoria>> DeleteAsync(int id) => _repository.DeleteAsync(id);
    public Task<ActionResponse<LogAuditoria>> AddAsync(LogAuditoriaDTO entity) => _repository.AddAsync(entity);
    public Task<IEnumerable<LogAuditoria>> GetComboAsync() => _repository.GetComboAsync();
    public Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => _repository.GetTotalRecordsAsync(pagination);
    public Task<ActionResponse<LogAuditoria>> UpdateAsync(LogAuditoriaDTO entity) => _repository.UpdateAsync(entity);
}
