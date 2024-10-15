namespace MasterPlanProject.WebApi.Repository
{
	public class LocalitaPugliaRepository : ILocalitaPugliaRepository
	{
		private readonly MasterPlanDataDbContext dbCont;

		public LocalitaPugliaRepository(MasterPlanDataDbContext dbCont)
		{
			this.dbCont = dbCont;
		}
		public async Task CreateAsync(LocalitaPuglia entity)
		{
			await dbCont.AddAsync(entity);
			await SaveAsync();
		}
		public async Task<List<LocalitaPuglia>> GetAllLocalitaAsync(Expression<Func<LocalitaPuglia, bool>> filter = null)
		{
			IQueryable<LocalitaPuglia> query = dbCont.LocalitaPuglia;
			if (filter != null)
				query = query.Where(filter);
			return await query.ToListAsync();
		}
		public async Task<LocalitaPuglia> GetLocalitaAsync(Expression<Func<LocalitaPuglia, bool>> filter = null, bool tracked = true)
		{
			IQueryable<LocalitaPuglia> query = dbCont.LocalitaPuglia;
			if (filter != null)
				query = query.Where(filter);
			if (tracked == true)
				query = query.AsNoTracking();
			return await query.FirstOrDefaultAsync();
		}
		public async Task RemoveAsync(LocalitaPuglia entity)
		{
			dbCont.Remove(entity);
			await SaveAsync();
		}
		public async Task UpdateAsync(LocalitaPuglia entity)
		{
			dbCont.Update(entity);
			await SaveAsync();
		}
		public async Task SaveAsync()
		{
			await dbCont.SaveChangesAsync();
		}
	}
}
