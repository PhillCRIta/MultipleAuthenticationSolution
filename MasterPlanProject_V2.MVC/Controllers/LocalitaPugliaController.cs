namespace MasterPlanProject.Mvc.Controllers
{
	public class LocalitaPugliaController : Controller
	{
		private readonly ILocalitaPugliaService pugliaService;
		private readonly IMapper mapper;

		public LocalitaPugliaController(ILocalitaPugliaService pugliaService, IMapper mapper)
        {
			this.pugliaService = pugliaService;
			this.mapper = mapper;
		}
		[Authorize]
		public async Task<IActionResult> IndexLocalita()
		{
			List<LocalitaPugliaDTO> listaLocalita = new();
			APIResponse response = await pugliaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(Constant.SessioneToken));
			if (response != null && response.IsSucces)
			{
				listaLocalita = JsonConvert.DeserializeObject<List<LocalitaPugliaDTO>>(Convert.ToString(response.Result));
			}
			ResultApiModel m = new() { IsSucces = response.IsSucces, ListaErrori = response.ErrorMessages, ValoreRitornato = listaLocalita };
			return View(m);
		}
    }
}
