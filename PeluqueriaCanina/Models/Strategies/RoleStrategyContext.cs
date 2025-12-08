public class RoleStrategyContext
{
    private IUserRoleStrategy _strategy;

    public void SetStrategy(IUserRoleStrategy strategy)
    {
        _strategy = strategy;
    }

    public string Execute() => _strategy.GetRedirect();
}
