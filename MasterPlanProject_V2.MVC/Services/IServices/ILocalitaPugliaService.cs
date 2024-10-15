namespace MasterPlanProject.Mvc.Services.IServices
{
	public interface ILocalitaPugliaService
	{
		Task<T> GetAllAsync<T>(string token);
		Task<T> GetAsync<T>(int id, string token);
		Task<T> CreateAsync<T>(LocalitaPugliaDTO_Insert localitaInsertDTO, string token);
		Task<T> UpdateAsync<T>(LocalitaPugliaDTO_Update localitaUpdateDTO, string token);
		Task<T> DeleteAsync<T>(int id, string token);
	}
}
