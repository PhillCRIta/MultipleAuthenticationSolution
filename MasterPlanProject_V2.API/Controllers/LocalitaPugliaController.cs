namespace MasterPlanProject.WebApi.Controllers
{
    [Route("api/BellezzePuglia")]
    [ApiController]
	
    public class LocalitaPugliaController : ControllerBase
    {
        private readonly ILocalitaPugliaRepository dbLocalita;
        private readonly IMapper mapper;
        protected APIResponse response;
        public LocalitaPugliaController(ILocalitaPugliaRepository dbLocalita, IMapper mapper)
        {
            this.dbLocalita = dbLocalita;
            this.mapper = mapper;
            response = new();
        }

        [HttpGet("GetLocalita")]
		[Authorize(Roles = "admin")]
		[Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetLocalita()
        {
            try
            {
                IEnumerable<LocalitaPuglia> listaLocalita = await dbLocalita.GetAllLocalitaAsync();
                response.Result = mapper.Map<IEnumerable<LocalitaPugliaDTO>>(listaLocalita);
                response.StatusCode = HttpStatusCode.OK;
				response.IsSucces = true;
				return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSucces = false;
                response.ErrorMessages = new List<string>() { ex.Message.ToString() };
            }
            return response;
        }


		[HttpGet("GetLocalitaNoAuth")]
		[Produces("application/json")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<ActionResult<APIResponse>> GetLocalitaNoAuth()
		{
			Debug.WriteLine(HttpContext.Request);
			Debug.WriteLine(HttpContext.User);
			Debug.WriteLine(User?.Identity?.Name);
			Debug.WriteLine(User.Identity.IsAuthenticated);
			try
			{
				IEnumerable<LocalitaPuglia> listaLocalita = await dbLocalita.GetAllLocalitaAsync();
				response.Result = mapper.Map<IEnumerable<LocalitaPugliaDTO>>(listaLocalita);
				response.StatusCode = HttpStatusCode.OK;
				response.IsSucces = true;
				return Ok(response);
			}
			catch (Exception ex)
			{
				response.IsSucces = false;
				response.ErrorMessages = new List<string>() { ex.Message.ToString() };
			}
			return response;
		}
















		/*
		 * 
		 * 
		 * 
		 * 
		 * 
		
		[Produces("application/json")]
		[HttpGet("{id:int}", Name = "GetLuogoDiretto")]
		//[ProducesResponseType(200, Type =typeof(LuoghiPugliaDTO)]	//INDICO I TIPI DI RISULTATI CHE PUO' AVERE L'API, posso anche specificare cosa ritorna, che comunque è già specificato nel <tipo> del result
		//[ProducesResponseType(400)]							    //INDICO I TIPI DI RISULTATI CHE PUO' AVERE L'API
		//[ProducesResponseType(404)]							    //INDICO I TIPI DI RISULTATI CHE PUO' AVERE L'API
		[ProducesResponseType(StatusCodes.Status200OK)]        //altro modo per definire l'uscita
		[ProducesResponseType(StatusCodes.Status404NotFound)]  //altro modo per definire l'uscita
		[ProducesResponseType(StatusCodes.Status400BadRequest)]//altro modo per definire l'uscita
		public ActionResult<APIResponse> GetLocalita(int id)
		{
			if (id == 0)
			{
				response.StatusCode = HttpStatusCode.BadRequest;
		 response.IsSucces = false;
				return BadRequest(response);
			}
			if (id < 0)
			{
				response.StatusCode = HttpStatusCode.NotFound;
		 response.IsSucces = false;
				return NotFound(response);
			}
			try
			{
				response.Result = new LocalitaPugliaDTO { Id = id, Area = "Salento", Localita = "Nardò" };
				response.StatusCode = HttpStatusCode.OK;
		 response.IsSucces = true;
				return Ok(response);
			}
			catch (Exception ex)
			{
				response.IsSucces = false;
				response.ErrorMessages = new List<string>() { ex.Message.ToString() };
			}
			return response;
		}
		/*
		 * 
		 * 
		 * 
		 * 
		 * 
		
		[HttpPost]//per inserire un dato
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<APIResponse> CreateLocalita([FromBody] LocalitaPugliaDTO localitaDTO) //l'oggetto è contenuto nel corpo della richiesta
		{
			//posso anche fare in questo modo se commento il APIController sopra
			//if(ModelState.IsValid == false)
			//{
			//	return BadRequest(ModelState);
			//}
			if (localitaDTO is null)
			{
				return BadRequest();
			}
			if (localitaDTO.Id < 0)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			//return Ok(new LuoghiPugliaDTO { Id = villaDTO.Id, Nome = villaDTO.Nome, Localita = villaDTO.Localita });
			//return CreatedAtRoute("GetLuogoDiretto", new { id = villaDTO.Id }, villaDTO);//link diretto alla risora creata -- RITORNA CODICE 201, punta alla risorse sopra nominata con GetLuogoDiretto
			try
			{
				response.Result = mapper.Map<LocalitaPugliaDTO>(localitaDTO);//va sostituita con un oggetto di ritorno da un insert nel db
				response.StatusCode = HttpStatusCode.Created;
				return CreatedAtRoute("GetLuogoDiretto", new { id = localitaDTO.Id }, response);//link diretto alla risora creata -- RITORNA CODICE 201, punta alla risorse sopra nominata con GetLuogoDiretto
			}
			catch (Exception ex)
			{
				response.IsSucces = false;
				response.ErrorMessages = new List<string>() { ex.Message.ToString() };
			}
			return response;
		}
		/*
		 * 
		 * 
		 * 
		 * 
		 * 
		 
		[HttpDelete("{id:int}", Name = "DeleteLuogoDiretto")]//il nome qui è ininfluente
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult DeleteLocalita(int id)
		{
			if (id == 0)
				return BadRequest();
			//if (luogo == null)
			//	return NotFound();
			return NoContent();//se faccio una cancellazione non devo ritornare nulla 204
		}
		*/
	}
}
