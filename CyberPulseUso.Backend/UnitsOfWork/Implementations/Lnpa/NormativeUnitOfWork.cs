using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Lnpa;

public class NormativeUnitOfWork : GenericUnitOfWork<Normative>, INormativeUnitOfWork
{
    private readonly INormativeRepository _repository;

    public NormativeUnitOfWork(INormativeRepository repository) : base(repository)
    {
        _repository = repository;
    }

    public async Task<ActionResponse<NormativeTreeDTO?>> GetTreeAsync(int id)
        => await _repository.GetTreeAsync(id);
}