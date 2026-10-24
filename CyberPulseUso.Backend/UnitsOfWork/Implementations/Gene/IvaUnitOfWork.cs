using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Gene;

public class IvaUnitOfWork : GenericUnitOfWork<Iva>, IIvaUnitOfWork
{
    private readonly IIvaRepository _ivaRepository;

    public IvaUnitOfWork(IGenericRepository<Iva> repository, IIvaRepository ivaRepository) : base(repository)
    {
        _ivaRepository = ivaRepository;
    }

    public override async Task<ActionResponse<Iva>> GetAsync(int id) => await _ivaRepository.GetAsync(id);

    // NOTA: La firma recibida en el código original no coincide con la firma genérica heredada.
    // Se conserva la firma compatible con GenericUnitOfWork<Iva>; confirme la firma real del repositorio.
    public override async Task<ActionResponse<IEnumerable<Iva>>> GetAsync(PaginationDTO pagination)
        => await _ivaRepository.GetAsync(pagination);

    public override async Task<ActionResponse<Iva>> DeleteAsync(int id) => await _ivaRepository.DeleteAsync(id);
    public async Task<ActionResponse<Iva>> AddAsync(IvaDTO entity) => await _ivaRepository.AddAsync(entity);
    public async Task<IEnumerable<Iva>> GetComboAsync() => await _ivaRepository.GetComboAsync();
    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _ivaRepository.GetTotalRecordsAsync(pagination);
    public async Task<ActionResponse<Iva>> UpdateAsync(IvaDTO entity) => await _ivaRepository.UpdateAsync(entity);
}
