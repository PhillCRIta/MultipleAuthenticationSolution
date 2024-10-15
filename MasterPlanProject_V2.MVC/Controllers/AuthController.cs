namespace MasterPlanProject.Mvc.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService authService;

		public AuthController(IAuthService authService)
		{
			this.authService = authService;
		}

		[HttpGet]
		public IActionResult Login()
		{
			LoginRequestDTO obj = new();
			return View(obj);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginRequestDTO obj)
		{
			APIResponse loginResponse = await authService.LoginAsync<APIResponse>(obj);
			if (loginResponse != null && loginResponse.IsSucces)
			{
				LoginResponseDTO model = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(loginResponse.Result));
				//leggo il token per leggere i valori che contiene, i valori possono essere passati anche tramite oggetto di ritorno
				//in questo caso è un altro modo per passare i dati, in questo caso NON SENSIBILI, SONO IN CHIARO
				JwtSecurityTokenHandler handler = new();
				JwtSecurityToken jwt = handler.ReadJwtToken(model.Token);
				string email = jwt.Claims.FirstOrDefault(c => c.Type == "email").Value;
				//setto la sessione in cui viene memorizzato il TOKEN da riusare
				HttpContext.Session.SetString(Constant.SessioneToken, model.Token);
				//una volta memorizzato il token, creo il cookie di sessione per garantire l'autorizzazione alle pagine protette
				ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
				identity.AddClaim(new Claim(ClaimTypes.Name, model.User.Username));
				foreach (string role in model.User.Roles)
				{
					identity.AddClaim(new Claim(ClaimTypes.Role, role));//se ci sono più ruoli posso passare un array, oppure inserire più volte la riga
				}
				identity.AddClaim(new Claim(ClaimTypes.Email, email));
				ClaimsPrincipal principal = new ClaimsPrincipal();
				principal.AddIdentity(identity);
				/*
				 * CLAIM > variabile che contiene una coppia CHIAVE-VALORE
				 * CLAIMS IDENTITY > un gruppo di claim che raggruppano i dati di identità di un SOGETTO, ad esempio un dispositivo che fa l'accesso, o un utente
				 * CLAIMS PRINCIPAL > un gruppo che contiene più contenitori ClaimsIDENTITY, ad esempio un c. Principal può contenere l'identity dell'utenza che ha fatto accesso
				 * e l'indentity del device con cui ha fatto accesso
				 * PRINCIPAL > PORTAFOGLIO
				 * IDENTITIES > DOCUMENTI, Driver's License, Passport, Credit Card, Google Account, Facebook Account, RSA SecurID, Finger print, Facial recognition,
				 * CLAIM > DATI SCRITTI SUL SINGOLO DOCUMENTO
				*/
				//salvo l'identity nel cookie di sessione dell'httpcontext
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
				return RedirectToAction("Index", "Home");
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
			HttpContext.Session.SetString(Constant.SessioneToken, "");
			return RedirectToAction("Index", "Home");
		}
		public IActionResult AccessDenied()
		{
			return View();
		}
	}
}
