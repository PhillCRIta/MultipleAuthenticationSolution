using ContractLibrary;
using MasterPlanProject_V2.API.Models;

namespace MasterPlanProject.WebApi.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly MasterPlanDataDbContext dbCon;
		private readonly MasterPlanIdentityDbContext dbIdCon;
		private readonly IServiceProvider currenProvider;
		private readonly IMapper mapper;
		private readonly UserManager<ApplicationIdentityUser> userManager;
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly string secret;
		private readonly string typeAuth;

		public UserRepository(
			MasterPlanDataDbContext dbCon,
			MasterPlanIdentityDbContext dbIdCon,
			IConfiguration conf,
			IServiceProvider CurrenProvider,
			IMapper mapper)
		{
			this.dbCon = dbCon;
			this.dbIdCon = dbIdCon;
			currenProvider = CurrenProvider;
			this.mapper = mapper;
			secret = conf.GetValue<string>("ApiSetting:Secret");
			typeAuth = conf.GetValue<string>("TypeOfAuthentication:Type");
			switch (typeAuth)
			{
				case "IDENTITY":
					this.userManager = currenProvider.GetService<UserManager<ApplicationIdentityUser>>();
					this.roleManager = currenProvider.GetService<RoleManager<IdentityRole>>();
					break;
			}
		}
		public bool IsUniqueUser(string username)
		{
			// BUILTIN, IDENTITY, IDENTITYSERVER
			bool result = false;
			switch (typeAuth)
			{
				case "BUILTIN":
					LocalUsers user = dbCon.LocalUsers.FirstOrDefault(u => u.Username == username);
					if (user == null)
					{
						result = true;
					}
					break;
				case "IDENTITY":
					ApplicationIdentityUser user2 = dbIdCon.ApplicationIdentityUsers.FirstOrDefault(u => u.UserName == username);
					if (user2 == null)
					{
						result = true;
					}
					break;
			}
			return result;
		}
		public async Task<TokenDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
		{
			bool result = false;
			string Id = "";
			string Name = "";
			string Email = "";
			List<string> roles = new();
			LocalUsersDTO userDTO = null;
			switch (typeAuth)
			{
				case "BUILTIN":
					var user = dbCon.LocalUsers.FirstOrDefault(u => u.Username == loginRequestDTO.Username && u.Password == loginRequestDTO.Password);
					if (user != null)
					{
						Id = user.Id.ToString();
						Name = user.Name.ToString();
						roles.Add(user.Role);
						result = true;
					}
					userDTO = mapper.Map<LocalUsersDTO>(user);
					if (roles.Count > 0)
						userDTO.Roles = roles;
					break;
				case "IDENTITY":
					var user2 = dbIdCon.ApplicationIdentityUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.Username.ToLower());
					bool isValid = await userManager.CheckPasswordAsync(user2, loginRequestDTO.Password);
					if (user2 != null && isValid == true)
					{
						Id = user2.Id;
						//per avere il ruolo passo da una funzione interna di identity
						roles = (List<string>)await userManager.GetRolesAsync(user2);
						Name = user2.Name.ToString();
						result = true;
						userDTO = mapper.Map<LocalUsersDTO>(user2);
						Email = user2.Email + "@mydomain.com";
					}
					if (roles != null && roles.Count > 0)
						userDTO.Roles = roles;
					break;
			}
			if (result == false)
			{
				return new TokenDTO()
				{
					AccessToken = ""
				};
			}
			string jwtTokenId = $"JTID{Guid.NewGuid()}";
			string refreshToken = await CreateNewRefreshToken(Id, jwtTokenId);
			TokenDTO response = GenerazioneToken(Id, Name, roles, Email, jwtTokenId, refreshToken);
			return response;
		}
		private TokenDTO GenerazioneToken(string Id, string Name, List<string> Roles,
										  string email, string jwtTokenId, string refreshToken,
										  string sesso = "maschio")
		{
			Byte[] key = Encoding.ASCII.GetBytes(secret);
			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
			ClaimsIdentity ci = new();
			ci.AddClaim(new Claim(ClaimTypes.Name, Name));
			ci.AddClaim(new Claim(ClaimTypes.NameIdentifier, Id));
			ci.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId));
			ci.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, Id));
			foreach (string role in Roles)
			{
				ci.AddClaim(new Claim(ClaimTypes.Role, role));
			}
			Dictionary<string, object> genericClaims = new Dictionary<string, object>();
			genericClaims.Add(ClaimTypes.Gender, sesso);
			genericClaims.Add(ClaimTypes.Email, email);
			SecurityTokenDescriptor tokenDesriptor = new SecurityTokenDescriptor
			{
				Subject = ci,
				Claims = genericClaims,
				Expires = DateTime.UtcNow.AddMinutes(10),
				SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			SecurityToken token = tokenHandler.CreateToken(tokenDesriptor);
			TokenDTO tokenDto = new TokenDTO()
			{
				AccessToken = tokenHandler.WriteToken(token),
				RefreshToken = refreshToken
			};
			return tokenDto;
		}
		public async Task<LocalUsersDTO> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
		{
			switch (typeAuth)
			{
				case "BUILTIN":
					LocalUsers user = new LocalUsers()
					{
						Username = registrationRequestDTO.Username,
						Password = registrationRequestDTO.Password,
						Name = registrationRequestDTO.Name,
						Role = registrationRequestDTO.Role,
						Email = registrationRequestDTO.Name + "@mydomain.com"
					};
					dbCon.LocalUsers.Add(user);
					await dbCon.SaveChangesAsync();
					user.Password = "";
					return mapper.Map<LocalUsersDTO>(user);
					break;
				case "IDENTITY":
					try
					{
						ApplicationIdentityUser user2 = new ApplicationIdentityUser()
						{
							UserName = registrationRequestDTO.Username,
							Email = registrationRequestDTO.Username,
							NormalizedEmail = registrationRequestDTO.Username.ToUpper(),
							Name = registrationRequestDTO.Name
						};
						IdentityResult result = await userManager.CreateAsync(user2, registrationRequestDTO.Password);
						var userReturn = dbIdCon.ApplicationIdentityUsers.FirstOrDefault(u => u.UserName == registrationRequestDTO.Username);
						if (result.Succeeded)
						{
							//verifico se il ruolo esiste, altrimenti lo inserisco
							if (await roleManager.RoleExistsAsync(registrationRequestDTO.Role) == false)
							{
								IdentityResult resultRole = await roleManager.CreateAsync(new IdentityRole(registrationRequestDTO.Role));
								if (resultRole.Succeeded == false)
								{
									throw new Exception("Errore nella creazione del ruolo " + string.Join(" ", resultRole.Errors));
								}
							}
							await userManager.AddToRoleAsync(user2, registrationRequestDTO.Role);
							return new LocalUsersDTO()
							{
								Id = userReturn.Id,
								Username = userReturn.UserName,
								Name = userReturn.Name,
								Roles = (List<string>)await userManager.GetRolesAsync(user2)
							};
						}
						else
						{
							throw new Exception("Errore nella creazione dell'utente " + string.Join(" ", result.Errors));
						}
					}
					catch (Exception ex)
					{
						throw new Exception(ex.Message);
					}
					break;
			}
			return null;
		}

		public async Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDto)
		{
			RefreshTokens exixstingRefreshToken = await dbCon.RefreshTokens.FirstOrDefaultAsync(t => t.Refresh_Token == tokenDto.RefreshToken);
			if (exixstingRefreshToken == null)
			{
				return new TokenDTO();
			}
			(bool IsSuccessful, string userId, string tokenId) accessTokenData = GetAccessTokenData(tokenDto.AccessToken);
			if (accessTokenData.IsSuccessful == false ||
				accessTokenData.userId != exixstingRefreshToken.UserId ||
				accessTokenData.tokenId != exixstingRefreshToken.JwtTokenId)
			{
				exixstingRefreshToken.IsValid = false;
				dbCon.SaveChanges();
				return new TokenDTO();
			}

			if (exixstingRefreshToken.IsValid == false)
			{
				List<RefreshTokens> chainRefreshTokens = dbCon.RefreshTokens.Where(t => t.UserId == exixstingRefreshToken.UserId &&
																   t.JwtTokenId == exixstingRefreshToken.JwtTokenId).ToList();
				foreach (RefreshTokens refToken in chainRefreshTokens)
				{
					refToken.IsValid = false;
				}
				dbCon.UpdateRange(chainRefreshTokens);
				dbCon.SaveChanges();
				return new TokenDTO();
			}

			if (exixstingRefreshToken.ExpiresAt < DateTime.UtcNow)
			{
				exixstingRefreshToken.IsValid = false;
				dbCon.SaveChanges();
				return new TokenDTO();
			}

			string newRefreshToken = await CreateNewRefreshToken(exixstingRefreshToken.UserId, exixstingRefreshToken.JwtTokenId);
			exixstingRefreshToken.IsValid = false;
			dbCon.SaveChanges();
			bool result = true;
			string Id = "";
			string Name = "";
			string Email = "";
			List<string> roles = new();
			switch (typeAuth)
			{
				case "BUILTIN":
					LocalUsers user = dbCon.LocalUsers.FirstOrDefault(u => u.Username == exixstingRefreshToken.UserId);
					if (user == null)
					{
						result = false;
					}
					else
					{
						Id = user.Id.ToString();
						Name = user.Name.ToString();
						roles.Add(user.Role);
						result = true;
					}
					break;
				case "IDENTITY":
					ApplicationIdentityUser appUser = dbIdCon.ApplicationIdentityUsers.FirstOrDefault(u => u.Id == exixstingRefreshToken.UserId)!;
					if (appUser == null)
					{
						result = false;
					}
					else
					{
						Id = appUser.Id.ToString();
						Name = appUser.Name.ToString();
						roles = (List<string>)await userManager.GetRolesAsync(appUser);
						Email = appUser.Email + "@mydomain.com";
						result = true;
					}
					break;
			}
			if (result == false)
				return new TokenDTO();
			TokenDTO newTokenDTO = GenerazioneToken(Id, Name, roles, Email, exixstingRefreshToken.JwtTokenId, newRefreshToken);
			return newTokenDTO;
		}

		private async Task<string> CreateNewRefreshToken(string userId, string tokenId)
		{
			RefreshTokens refreshToken = new()
			{
				IsValid = true,
				UserId = userId,
				JwtTokenId = tokenId,
				ExpiresAt = DateTime.UtcNow.AddMinutes(60),
				Refresh_Token = Guid.NewGuid() + "-" + Guid.NewGuid()
			};
			await dbCon.RefreshTokens.AddAsync(refreshToken);
			await dbCon.SaveChangesAsync();
			return refreshToken.Refresh_Token;
		}
		public async Task RevokerefreshToken(TokenDTO tokenDTO)
		{
			RefreshTokens? exixstingRefreshToken = await dbCon.RefreshTokens.FirstOrDefaultAsync(_ => _.Refresh_Token == tokenDTO.RefreshToken);
			if (exixstingRefreshToken == null)
				return;
			(bool IsSuccessful, string userId, string tokenId) tokenIsValid = GetAccessTokenData(tokenDTO.AccessToken);
			if (tokenIsValid.IsSuccessful == false)
			{
				return;
			}
			List<RefreshTokens> chainRefreshTokens = dbCon.RefreshTokens.Where(t => t.UserId == exixstingRefreshToken.UserId &&
																   t.JwtTokenId == exixstingRefreshToken.JwtTokenId).ToList();
			foreach (RefreshTokens refToken in chainRefreshTokens)
			{
				refToken.IsValid = false;
			}
			dbCon.UpdateRange(chainRefreshTokens);
			dbCon.SaveChanges();
		}
		private (bool IsSuccessful, string userId, string tokenId) GetAccessTokenData(string accessToken)
		{
			try
			{
				JwtSecurityTokenHandler handler = new();
				JwtSecurityToken jwt = handler.ReadJwtToken(accessToken);
				string jwtTokenId = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)!.Value;
				string userId = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
				return (true, userId, jwtTokenId);
			}
			catch (Exception)
			{
				return (false, null, null);
			}
		}


	}
}
