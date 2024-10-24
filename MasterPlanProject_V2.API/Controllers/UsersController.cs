﻿using Microsoft.IdentityModel.Tokens;

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
				if (tokenDto == null || string.IsNullOrEmpty(tokenDto.AccessToken))
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
		[HttpPost("refresh")]
		public async Task<IActionResult> GetNewTokenFromRefreshToken([FromBody] TokenDTO tokenDto)
		{
			if (ModelState.IsValid)
			{
				TokenDTO tokenDTOResponse = await userRepo.RefreshAccessToken(tokenDto);
				if (tokenDTOResponse == null || string.IsNullOrEmpty(tokenDTOResponse.AccessToken))
				{
					response.StatusCode = HttpStatusCode.BadRequest;
					response.IsSucces = false;
					response.ErrorMessages.Add("Token non valido");
					return BadRequest(response);
				}
				response.StatusCode = HttpStatusCode.OK;
				response.IsSucces = true;
				response.Result = tokenDTOResponse;
				return Ok(response);
			}
			else
			{
				response.StatusCode = HttpStatusCode.BadRequest;
				response.IsSucces = false;
				response.ErrorMessages.Add("Invalid input");
				return BadRequest(response);
			}
		}
		[HttpPost("revoke")]
		public async Task<IActionResult> RevokeRefreshToken([FromBody] TokenDTO tokenDTO)
		{
			if (ModelState.IsValid)
			{
				await userRepo.RevokerefreshToken(tokenDTO);
				response.StatusCode = HttpStatusCode.OK;
				response.IsSucces = true;
				return Ok(response);
			}
			response.IsSucces = false;
			response.Result = "Invalid input";
			return BadRequest(response);
		}
	}
}
