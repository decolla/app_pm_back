using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;
using app_pm_back.Models.Domain;
using AutoMapper;

namespace app_pm_back.Profiles;

/// <summary>
/// Configuracao de mapeamentos de DTOs para modelos de dominio.
/// </summary>
public class AutoMapperProfile : Profile
{
    /// <summary>
    /// FAZ O MAPEAMENTO AUTOMÁTICO DE VARIÁVES DE MESMO NOME ENTRE DTO E MODEL
    /// </summary>
    /// <returns></returns>
    public AutoMapperProfile()
    {
        CreateMap<NotaManutencaoComgasDto, NotaManutencaoModel>()
            // A planilha pode enviar IdNota vazio e NumeroNota preenchido.
            .ForMember(dest => dest.IdNota,
                opt => opt.MapFrom(src => src.IdNota > 0 ? src.IdNota : src.NumeroNota))
            .ForMember(dest => dest.Itens, opt => opt.Ignore())
            .ForMember(dest => dest.Medidas, opt => opt.Ignore())
            .ForMember(dest => dest.Atividades, opt => opt.Ignore())
            .ForMember(dest => dest.Ordens, opt => opt.Ignore())
            .ForMember(dest => dest.Equipamento, opt => opt.Ignore())
            .ForMember(dest => dest.LocalInstalacao, opt => opt.Ignore())
            .ForMember(dest => dest.CriadoPorUsuario, opt => opt.Ignore());

        CreateMap<OrdemManutencaoComgasDto, OrdemManutencaoModel>()
            .ForMember(dest => dest.NotaRelacionada, opt => opt.Ignore())
            .ForMember(dest => dest.PlanoManutencao, opt => opt.Ignore())
            .ForMember(dest => dest.CriadaPorUsuario, opt => opt.Ignore())
            .ForMember(dest => dest.ModificadaPorUsuario, opt => opt.Ignore())
            .ForMember(dest => dest.Operacoes, opt => opt.Ignore());

        // CreateMap<AcaoNotaCongasComgasDto, AcaoNotaCongasModel>();

        CreateMap<ItemNotaManutencaoComgasDto, ItemNotaManutencaoModel>()
            .ForMember(dest => dest.Nota, opt => opt.Ignore());

        CreateMap<EquipamentoAtivoComgasDto, EquipamentoAtivoModel>()
            .ForMember(dest => dest.Fabricante, opt => opt.Ignore())
            .ForMember(dest => dest.Localizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Endereco, opt => opt.Ignore())
            .ForMember(dest => dest.Imobilizado, opt => opt.Ignore())
            .ForMember(dest => dest.Gpm, opt => opt.Ignore())
            .ForMember(dest => dest.CentroTrabalho, opt => opt.Ignore())
            .ForMember(dest => dest.LocalInstalacao, opt => opt.Ignore())
            .ForMember(dest => dest.Notas, opt => opt.Ignore())
            .ForMember(dest => dest.ItensPlano, opt => opt.Ignore());

        CreateMap<EstrategiaManutencaoComgasDto, EstrategiaManutencaoModel>()
            .ForMember(dest => dest.Planos, opt => opt.Ignore())
            .ForMember(dest => dest.Itens, opt => opt.Ignore());

        CreateMap<TipoEstrategiaComgasDto, TipoEstrategiaModel>()
            .ForMember(dest => dest.Estrategia, opt => opt.Ignore())
            .ForMember(dest => dest.TiposManutencao, opt => opt.Ignore());

        CreateMap<PlanoManutencaoComgasDto, PlanoManutencaoModel>()
            .ForMember(dest => dest.Prioridade,
                opt => opt.MapFrom(src => src.Prioridade.HasValue ? (Prioridade?)src.Prioridade.Value : null))
            .ForMember(dest => dest.GerarAutomaticamente,
                opt => opt.MapFrom(src => src.GerarAutomaticamente.HasValue
                    ? (GerarAutomaticamente?)src.GerarAutomaticamente.Value
                    : null))
            .ForMember(dest => dest.Estrategia, opt => opt.Ignore())
            .ForMember(dest => dest.TiposManutencao, opt => opt.Ignore())
            .ForMember(dest => dest.Ciclos, opt => opt.Ignore())
            .ForMember(dest => dest.Itens, opt => opt.Ignore())
            .ForMember(dest => dest.Ordens, opt => opt.Ignore());

        CreateMap<TipoManutencaoComgasDto, TipoManutencaoModel>()
            .ForMember(dest => dest.TipoTipo, opt => opt.MapFrom(src => (TipoTipo)src.TipoTipo))
            .ForMember(dest => dest.ListaTarefa, opt => opt.MapFrom(src => src.ListaTarefas))
            .ForMember(dest => dest.TipoEstrategia, opt => opt.Ignore())
            .ForMember(dest => dest.PlanoManutencao, opt => opt.Ignore());

        CreateMap<FerramentaTipoDto, FerramentasTipoModel>();
        CreateMap<ItemRenovacaoTipoDto, ItemRenovacaoTipoModel>();

        CreateMap<TarefaComgasDto, TarefaModel>();
        CreateMap<FerramentaTipoComgasDto, FerramentasTipoModel>();
        CreateMap<ItemRenovacaoTipoComgasDto, ItemRenovacaoTipoModel>();

        CreateMap<TipoManutencaoCepComgasDto, TipoManutencaoCepModel>()
            .ForMember(dest => dest.AcoesFalha,
                opt => opt.MapFrom(src => src.AcoesFalha.Select(a => (AcoesFalha)a).ToList()))
            .ForMember(dest => dest.PlanoManutencao, opt => opt.Ignore());
    }
}