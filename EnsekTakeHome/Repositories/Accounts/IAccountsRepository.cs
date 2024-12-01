namespace EnsekTakeHome.Repositories.Accounts
{
    public interface IAccountsRepository
    {
        public (int valid, int invalid) HandleCsv(Stream stream);
    }
}
