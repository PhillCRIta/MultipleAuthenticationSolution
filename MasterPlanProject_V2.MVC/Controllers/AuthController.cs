using MasterPlanProject_V2.MVC.Services.IServices;

namespace MasterPlanProject.Mvc.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService authService;
		private readonly ITokenProvider tokenProvider;

		public AuthController(IAuthService authService, ITokenProvider tokenProvider)
		{
			this.authService = authService;
			this.tokenProvider = tokenProvider;
		}

		[HttpGet]
		public IActionResult Login()
		{
			LoginRequestDTO obj = new();
			return View(obj);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginRequestDTO obj, string returnUrl = null)
		{
			APIResponse loginResponse = await authService.LoginAsync<APIResponse>(obj);
			if (loginResponse != null && loginResponse.IsSucces)
			{
				TokenDTO model = JsonConvert.DeserializeObject<TokenDTO>(Convert.ToString(loginResponse.Result));

				JwtSecurityTokenHandler handler = new();
				JwtSecurityToken jwt = handler.ReadJwtToken(model.AccessToken);

				string email = jwt.Claims.FirstOrDefault(c => c.Type == "email").Value;
				string userName = jwt.Claims.FirstOrDefault(c => c.Type == "unique_name").Value;

				List<string> roles = jwt.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();

				ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
				identity.AddClaim(new Claim(ClaimTypes.Name, userName));
				foreach (string role in roles)
				{
					identity.AddClaim(new Claim(ClaimTypes.Role, role));//se ci sono più ruoli posso passare un array, oppure inserire più volte la riga
				}
				identity.AddClaim(new Claim(ClaimTypes.Email, email));
				ClaimsPrincipal principal = new ClaimsPrincipal();
				principal.AddIdentity(identity);
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

				/* CLAIM > variabile che contiene una coppia CHIAVE-VALORE
				 * CLAIMS IDENTITY > un gruppo di claim che raggruppano i dati di identità di un SOGETTO, ad esempio un dispositivo che fa l'accesso, o un utente
				 * CLAIMS PRINCIPAL > un gruppo che contiene più contenitori ClaimsIDENTITY, ad esempio un c. Principal può contenere l'identity dell'utenza che ha fatto accesso
				 * e l'indentity del device con cui ha fatto accesso
				 * PRINCIPAL > PORTAFOGLIO
				 * IDENTITIES > DOCUMENTI, Driver's License, Passport, Credit Card, Google Account, Facebook Account, RSA SecurID, Finger print, Facial recognition,
				 * CLAIM > DATI SCRITTI SUL SINGOLO DOCUMENTO */

				tokenProvider.SetToken(model);
				if (string.IsNullOrEmpty(returnUrl))
					return RedirectToAction("Index", "Home");
				else
					return LocalRedirect(returnUrl);
			}
			else
			{
				string errorString = string.Join(" ", loginResponse.ErrorMessages);
				//errorString += loginResponse.ErrorDescription.GetType().GetProperties().First(o => o.Name == "message").GetValue(obj, null);
				if (loginResponse.ErrorDescription != null)
				{
					errorString += " " + loginResponse.ErrorDescription.Message;
					errorString += " " + string.Join(" ", loginResponse.ErrorDescription.ListMessage);
				}
				ModelState.AddModelError("CustomError", errorString);
				return View(obj);
			}
		}
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegistrationRequestDTO regObj)
		{
			APIResponse regResult = await authService.RegisterAsync<APIResponse>(regObj);
			if (regResult != null && regResult.IsSucces)
			{
				return RedirectToAction("Login");
			}
			return View();
		}
		public async Task<IActionResult> Logout()
		{
			//quando l'utente esce devo cancellare tutte le sessioni, compreso il token
			await HttpContext.SignOutAsync();
			//HttpContext.Session.SetString(Constant.AccessTokenSession, "");
			await authService.LogoutAsync<APIResponse>(tokenProvider.GetToken());
			tokenProvider.ClearToken();
			return RedirectToAction("Index", "Home");
		}
		public IActionResult AccessDenied()
		{
			return View();
		}
	}
}
