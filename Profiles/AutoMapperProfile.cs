using app_pm_back.DTO;
using app_pm_back.Models;
using AutoMapper;

namespace app_pm_back.Profiles;

public class AutoMapperProfile : Profile
{
    /// <summary>
    /// FAZ O MAPEAMENTO AUTOMÁTICO DE VARIÁVES DE MESMO NOME ENTRE DTO E MODEL
    /// </summary>
    /// <returns></returns>
    public AutoMapperProfile()
    {
        /*
    // cria o mapa de CreateNotaDto para NotaModel
    CreateMap<CreateNotaDto, NotaModel>()
        // ensina que a propriedade DadosGeraisDTO do DTO vai para DadosGerais do Model
        .ForMember(dest => dest.DadosGerais, opt => opt.MapFrom(src => src.DadosGeraisDto))
        // ignora o _id pois ele é gerado pelo banco/novo
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        // ignora campos que o DTO não tem, para não dar erro
        .ForMember(dest => dest.IdOrdem, opt => opt.Ignore())
        .ForMember(dest => dest.NroOrdem, opt => opt.Ignore());
    */
        CreateMap<CreateNotaPetroDto, NotaPetroModel>();
        CreateMap<CreateOrdemPetroDto, OrdemPetroModel>();
    }
}