namespace MasterPlanProject.WebApi.Repository.Interface
{
	public interface ILocalitaPugliaRepository
	{
		Task<List<LocalitaPuglia>> GetAllLocalitaAsync(Expression<Func<LocalitaPuglia, bool>> filter = null);
		Task<LocalitaPuglia> GetLocalitaAsync(Expression<Func<LocalitaPuglia, bool>> filter = null, bool tracked = true);
		Task CreateAsync(LocalitaPuglia entity);
		Task RemoveAsync(LocalitaPuglia entity);
		Task UpdateAsync(LocalitaPuglia entity);
		Task SaveAsync();
	}
}
