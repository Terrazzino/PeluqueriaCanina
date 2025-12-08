public class ClienteStrategy : IUserRoleStrategy
{
    public string GetRedirect() => "/Cliente/Dashboard";
}
