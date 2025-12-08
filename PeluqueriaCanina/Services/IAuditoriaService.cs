using PeluqueriaCanina.Models.ClasesDeAdministrador;
using System.Threading.Tasks;

public interface IAuditoriaService
{
    Task RegistrarAsync(Auditoria auditoria);
}
