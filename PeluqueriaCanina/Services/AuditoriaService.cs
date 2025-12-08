using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.ClasesDeAdministrador;

public class AuditoriaService : IAuditoriaService
{
    private readonly ContextoAcqua _context;

    public AuditoriaService(ContextoAcqua context)
    {
        _context = context;
    }

    public async Task RegistrarAsync(Auditoria auditoria)
    {
        _context.Auditorias.Add(auditoria);
        await _context.SaveChangesAsync();
    }
}
