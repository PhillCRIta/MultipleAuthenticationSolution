namespace MasterPlanProject.WebApi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			string typeAuth = builder.Configuration.GetValue<string>("TypeOfAuthentication:Type");
			Console.WriteLine("TypeOfAuth: " + typeAuth);

			// Add services to the container.
			builder.Services.AddControllers(
				opt =>
				{
					opt.ReturnHttpNotAcceptable = true;//legame forzato con la tipologia indicata nell'header
				}
				)
			.AddXmlDataContractSerializerFormatters();//con questa funzionalità , se passo un json, ma voglio l'uscita in xml, questo lo fa in automatico
													  //EF Core
			builder.Services.AddDbContext<MasterPlanDataDbContext>(optionsBuilder =>
			{
				optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("MPDbConnection"));
				if (builder.Environment.IsDevelopment())
				{
					optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information)
								  .EnableSensitiveDataLogging() 
								  .EnableDetailedErrors();
				}
			});
			//IDENTITY DB CONTEXT
			builder.Services.AddDbContext<MasterPlanIdentityDbContext>(optionsBuilder =>
			{
				optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("MPIdentityConnection"));
				if (builder.Environment.IsDevelopment())
				{
					optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information)
								  .EnableSensitiveDataLogging() 
								  .EnableDetailedErrors();
				}
			});

			builder.Services.AddScoped<ILocalitaPugliaRepository, LocalitaPugliaRepository>();
			builder.Services.AddScoped<IUserRepository, UserRepository>();

			builder.Services.AddAutoMapper(new[] { typeof(MappingConfig), typeof(MappingConfigWebApi) });

			builder.Services.AddEndpointsApiExplorer();

			//gestione dello swagger
			builder.Services.AddSwaggerGen(opt =>
			{
				opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Description = "Questa sicurezza è basata su JWT Token, usando lo schema Bearer." + System.Environment.NewLine +
					"Inserisci Bearer [spazio] e il token restituito dall'api di login.",
					Name = "Authorization", 
					In = ParameterLocation.Header, 
					Scheme = "Bearer"
				});
				opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
				{
					{
						//qui inserisco le configurazioni dei requisiti dell'Open Api Secuirity
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							},
						},
						new List<string>()
					}
				});
			});

			if (typeAuth == Constant.AuthType.IDENTITY.ToString())
			{
				builder.Services.AddIdentity<ApplicationIdentityUser, IdentityRole>(opt =>
				{
					//disabilito la sicurezza sulla password
					opt.Password.RequireUppercase = false;
					opt.Password.RequireLowercase = false;
					opt.Password.RequireNonAlphanumeric = false;
					opt.Password.RequiredLength = 0;
					opt.Password.RequireDigit = false;
				})
				.AddEntityFrameworkStores<MasterPlanIdentityDbContext>();//va scaricato un pacchetto nuget
			}

			builder.Services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(opt =>
			{
				opt.RequireHttpsMetadata = false;
				opt.SaveToken = true;
				opt.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("ApiSetting:Secret"))),
					ValidateIssuer = false,
					ValidateAudience = false
				};
				opt.Events = new JwtBearerEvents()
				{
					OnMessageReceived = msg =>
					{
						var token = msg?.Request.Headers.Authorization.ToString();
						string path = msg?.Request.Path ?? "";
						if (!string.IsNullOrEmpty(token))

						{
							Debug.WriteLine("Access token");
							Debug.WriteLine($"URL: {path}");
							Debug.WriteLine($"Token: {token}\r\n");
						}
						else
						{
							Debug.WriteLine("Access token");
							Debug.WriteLine("URL: " + path);
							Debug.WriteLine("Token: No access token provided\r\n");
						}
						return Task.CompletedTask;
					},
					OnTokenValidated = ctx =>
					{
						Console.WriteLine();
						Debug.WriteLine("Claims from the access token");
						if (ctx?.Principal != null)
						{
							foreach (var claim in ctx.Principal.Claims)
							{
								Debug.WriteLine($"{claim.Type} - {claim.Value}");
							}
						}
						Debug.WriteLine("");
						return Task.CompletedTask;
					}
				};
				opt.MapInboundClaims = true;
			});
			var app = builder.Build();



			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			app.UseHttpsRedirection();
			app.UseAuthentication();//primo
			app.UseAuthorization();
			app.MapControllers();



			app.Run();
		}
	}
}
