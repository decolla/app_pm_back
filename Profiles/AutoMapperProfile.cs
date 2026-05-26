using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models.Domain;
using AutoMapper;

namespace app_pm_back.Profiles;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<NotaManutencaoComgasDto, NotaModel>()
            .ForMember(dest => dest.IdNota, opt => opt.MapFrom(src => src.IdNota > 0 ? src.IdNota : src.NumeroNota))
            .ForMember(dest => dest.Itens, opt => opt.Ignore())
            .ForMember(dest => dest.Variaveis, opt => opt.Ignore())
            .ForMember(dest => dest.Anexos, opt => opt.Ignore());

        CreateMap<OrdemManutencaoComgasDto, OrdemModel>()
            .ForMember(dest => dest.Criticidade, opt => opt.MapFrom(src => src.Prioridade.HasValue ? src.Prioridade.Value.ToString() : null))
            .ForMember(dest => dest.Nota, opt => opt.Ignore())
            .ForMember(dest => dest.Anexos, opt => opt.Ignore())
            .ForMember(dest => dest.Operacoes, opt => opt.Ignore());

        CreateMap<EstrategiaManutencaoComgasDto, EstrategiaModel>();

        CreateMap<TipoEstrategiaComgasDto, TipoEstrategiaModel>()
            .ForMember(dest => dest.Estrategia, opt => opt.Ignore())
            .ForMember(dest => dest.TipoManutencao, opt => opt.Ignore())
            .ForMember(dest => dest.TipoCep, opt => opt.Ignore());

        CreateMap<PlanoManutencaoComgasDto, PlanoModel>()
            .ForMember(dest => dest.IdPlano, opt => opt.MapFrom(src => src.CodigoPlano))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.NomePlano ?? string.Empty))
            .ForMember(dest => dest.DataInicios, opt => opt.MapFrom(src => src.DataInicio))
            .ForMember(dest => dest.Antecedencia, opt => opt.MapFrom(src => src.Antencedencia))
            .ForMember(dest => dest.IdEstrategia, opt => opt.MapFrom(src => src.EstrategiaId ?? 0))
            .ForMember(dest => dest.Prioridade, opt => opt.MapFrom(src => src.Prioridade.HasValue ? (Prioridade?)src.Prioridade.Value : null))
            .ForMember(dest => dest.GerarAutomaticamente, opt => opt.MapFrom(src => src.GerarAutomaticamente.HasValue ? (GerarAutomaticamente?)src.GerarAutomaticamente.Value : null))
            .ForMember(dest => dest.Estrategia, opt => opt.Ignore());

        CreateMap<TipoPlanoCongasDto, TipoPlanoModel>()
            .ForMember(dest => dest.Visivel, opt => opt.MapFrom(src => src.Visivel == Visivel.Visivel))
            .ForMember(dest => dest.TipoEstrategia, opt => opt.Ignore())
            .ForMember(dest => dest.Plano, opt => opt.Ignore())
            .ForMember(dest => dest.TipoManutencao, opt => opt.Ignore())
            .ForMember(dest => dest.TipoCep, opt => opt.Ignore());

        CreateMap<TipoManutencaoComgasDto, TipoManutencaoModel>();

        CreateMap<TarefaComgasDto, OperacaoModel>()
            .ForMember(dest => dest.IdOperacao, opt => opt.MapFrom(src => src.IdTarefa))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao))
            .ForMember(dest => dest.Duracao, opt => opt.MapFrom(src => src.Duracao));

        CreateMap<TipoManutencaoCepComgasDto, TipoCepModel>()
            .ForMember(dest => dest.IdTipoCep, opt => opt.MapFrom(src => src.IdTipoManutencaoCep))
            .ForMember(dest => dest.AcoesFalha, opt => opt.MapFrom(src => src.AcoesFalha.Select(a => (AcoesFalha)a).ToList()));

        CreateMap<UsuarioSapComgasDto, UsuarioModel>()
            .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.IdUsuario))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome));

        CreateMap<ImobilizadoComgasDto, ImobilizadoModel>()
            .ForMember(dest => dest.IdImobilizado, opt => opt.MapFrom(src => ParseIntOrDefault(src.TagImobilizado)))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.NomeImobilizado ?? string.Empty))
            .ForMember(dest => dest.Nserie, opt => opt.MapFrom(src => src.NumeroSerie ?? string.Empty))
            .ForMember(dest => dest.Quantidade, opt => opt.MapFrom(src => src.Quantidade.HasValue ? (int)src.Quantidade.Value : 0))
            .ForMember(dest => dest.DataAquisicao, opt => opt.MapFrom(src => src.DataAquisicao ?? DateTime.MinValue))
            .ForMember(dest => dest.DataIncorporacao, opt => opt.MapFrom(src => src.DataIncorporacao ?? DateTime.MinValue))
            .ForMember(dest => dest.ValorCompra, opt => opt.MapFrom(src => src.ValorCompra ?? 0))
            .ForMember(dest => dest.Moeda, opt => opt.MapFrom(src => "BRL"))
            .ForMember(dest => dest.Depreciacao, opt => opt.MapFrom(src => src.DepreciacaoAno.HasValue ? (double)src.DepreciacaoAno.Value : 0.0))
            .ForMember(dest => dest.ValorAtual, opt => opt.MapFrom(src => src.ValorAtual ?? 0))
            .ForMember(dest => dest.InfoAdicionais, opt => opt.MapFrom(src => src.InformacaoAdicional));

        CreateMap<CentroPlanejamentoManutencaoComgasDto, CpmModel>()
            .ForMember(dest => dest.IdCPM, opt => opt.MapFrom(src => src.IdCpm))
            .ForMember(dest => dest.NomeCentro, opt => opt.MapFrom(src => src.Nome ?? string.Empty))
            .ForMember(dest => dest.Organizacao, opt => opt.MapFrom(src => src.Organizacao ?? string.Empty))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao));

        CreateMap<CentroTrabalhoComgasDto, CtmModel>()
            .ForMember(dest => dest.IdGPM, opt => opt.MapFrom(src => src.IdCentroTrabalho))
            .ForMember(dest => dest.AreaCTM, opt => opt.MapFrom(src => src.Codigo))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao));

        CreateMap<GrupoPlanejamentoManutencaoComgasDto, GpmModel>()
            .ForMember(dest => dest.IdGPM, opt => opt.MapFrom(src => src.IdGpm))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao));

        CreateMap<AlmoxarifadoComgasDto, AlmoxarifadoModel>();

        CreateMap<FerramentaTipoComgasDto, FerramentaModel>()
            .ForMember(dest => dest.IdLI, opt => opt.MapFrom(src => src.IdLocalInstalacao ?? 0))
            .ForMember(dest => dest.IdEscopo, opt => opt.Ignore())
            .ForMember(dest => dest.IdAlmoxarifado, opt => opt.Ignore())
            .ForMember(dest => dest.ValorCompra, opt => opt.MapFrom(src => src.ValorCompra ?? 0))
            .ForMember(dest => dest.DataAquisicao, opt => opt.MapFrom(src => src.DataAquisicao ?? DateTime.MinValue));
    }

    private static int ParseIntOrDefault(string? s)
    {
        return int.TryParse(s, out var result) ? result : 0;
    }
}