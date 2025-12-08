namespace PeluqueriaCanina.Services
{
    public class RoleStrategyContext
    {
        private IUserRoleStrategy _strategy;

        public void SetStrategy(IUserRoleStrategy strategy)
        {
            _strategy = strategy;
        }

        public string GetDashboardRedirect()
        {
            return _strategy.GetRedirect();
        }
    }
}
