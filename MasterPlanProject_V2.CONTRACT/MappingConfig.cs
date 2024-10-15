using AutoMapper;
using ContractLibrary.Models;
using ContractLibrary.Models.Dto;

namespace ContractLibrary
{
	public class MappingConfig : Profile
	{
		public MappingConfig()
		{
			//un metodo
			//ci sono anche metodi di estensione che permettono di mappare ancora in modo più preciso 
			CreateMap<LocalitaPuglia, LocalitaPugliaDTO>();
			CreateMap<LocalitaPugliaDTO, LocalitaPuglia>();
			//altro metodo col reverse
			CreateMap<LocalitaPuglia, LocalitaPugliaDTO_Insert>().ReverseMap();
			CreateMap<LocalitaPuglia, LocalitaPugliaDTO_Update>().ReverseMap();
			CreateMap<LocalitaPuglia, LocalitaPugliaDTO_Update>().ReverseMap();
			CreateMap<LocalUsers, LocalUsersDTO>().ReverseMap();
			
		}
	}
}
