public class AdministradorStrategy : IUserRoleStrategy
{
    public string GetRedirect() => "/Administrador/Dashboard";
}
