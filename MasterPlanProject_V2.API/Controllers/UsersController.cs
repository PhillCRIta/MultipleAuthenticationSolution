namespace MasterPlanProject.WebApi.Controllers
{
	[Route("api/UserAuth")]
	[ApiController]
	public class UsersController : Controller
	{
		private readonly IUserRepository userRepo;
		private readonly IMapper mapper;
		protected APIResponse response;

		public UsersController(IUserRepository userRepo, IMapper mapper)
		{
			this.userRepo = userRepo;
			this.mapper = mapper;
			response = new();
		}

		[HttpPost("login")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
		{
			try
			{
				TokenDTO tokenDto = await userRepo.LoginAsync(model);
				if (tokenDto  == null || string.IsNullOrEmpty(tokenDto.AccessToken))
				{
					ModelState.AddModelError("ListMessage", "Invalid credential2");
					ModelState.AddModelError("ListMessage", "Messaggio2");
					return BadRequest(ModelState);
				}
				response.Result = tokenDto;
				response.StatusCode = HttpStatusCode.OK;
				return Ok(response);
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = "Errore generico login." });
			}
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO m)
		{
			bool isUnique = userRepo.IsUniqueUser(m.Username);
			if (isUnique == false)
			{
				response.StatusCode = HttpStatusCode.BadRequest;
				response.IsSucces = false;
				response.ErrorMessages.Add("Errore utenza già esistente.");
				return BadRequest(response);
			}
			var user = await userRepo.RegisterAsync(m);
			if (user == null)
			{
				response.StatusCode = HttpStatusCode.BadRequest;
				response.IsSucces = false;
				response.ErrorMessages.Add("Errore in fase di registrazione, contattare l'assistenza tecnica");
				return BadRequest(response);
			}
			response.StatusCode = HttpStatusCode.OK;
			response.IsSucces = true;
			return Ok(response);
		}

	}
}
