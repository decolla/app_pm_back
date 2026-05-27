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
            .ForMember(dest => dest.Criticidade, opt => opt.MapFrom(src => src.Prioridade.HasValue ? src.Prioridade.Value.ToString() : src.Criticidade))
            .ForMember(dest => dest.Nota, opt => opt.Ignore())
            .ForMember(dest => dest.Anexos, opt => opt.Ignore())
            .ForMember(dest => dest.Operacoes, opt => opt.Ignore());

        CreateMap<EstrategiaManutencaoComgasDto, EstrategiaModel>();

        CreateMap<TipoEstrategiaComgasDto, TipoEstrategiaModel>()
            .ForMember(dest => dest.Estrategia, opt => opt.Ignore())
            .ForMember(dest => dest.TipoManutencao, opt => opt.Ignore())
            .ForMember(dest => dest.TipoCep, opt => opt.Ignore());

        CreateMap<PlanoManutencaoComgasDto, PlanoModel>()
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome ?? string.Empty))
            .ForMember(dest => dest.Estrategia, opt => opt.Ignore());

        CreateMap<TipoPlanoCongasDto, TipoPlanoModel>()
            .ForMember(dest => dest.TipoEstrategia, opt => opt.Ignore())
            .ForMember(dest => dest.Plano, opt => opt.Ignore())
            .ForMember(dest => dest.TipoManutencao, opt => opt.Ignore())
            .ForMember(dest => dest.TipoCep, opt => opt.Ignore());

        CreateMap<TipoManutencaoComgasDto, TipoManutencaoModel>();

        CreateMap<TarefaComgasDto, OperacaoModel>()
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao))
            .ForMember(dest => dest.Duracao, opt => opt.MapFrom(src => src.Duracao));

        CreateMap<TipoManutencaoCepComgasDto, TipoCepModel>()
            .ForMember(dest => dest.AcoesFalha, opt => opt.MapFrom(src => src.AcoesFalha));

        CreateMap<UsuarioSapComgasDto, UsuarioModel>()
            .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.IdUsuario))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.CPF, opt => opt.MapFrom(src => src.Cpf))
            .ForMember(dest => dest.Permissoes, opt => opt.MapFrom(src => src.Permissoes))
            .ForMember(dest => dest.IdEscopo, opt => opt.MapFrom(src => src.IdEscopo));

        CreateMap<ImobilizadoComgasDto, ImobilizadoModel>()
            .ForMember(dest => dest.IdImobilizado, opt => opt.MapFrom(src => src.IdImobilizado ?? 0))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome ?? string.Empty))
            .ForMember(dest => dest.Nserie, opt => opt.MapFrom(src => src.Nserie ?? string.Empty))
            .ForMember(dest => dest.Quantidade, opt => opt.MapFrom(src => src.Quantidade.HasValue ? src.Quantidade.Value : 0))
            .ForMember(dest => dest.DataAquisicao, opt => opt.MapFrom(src => src.DataAquisicao ?? DateTime.MinValue))
            .ForMember(dest => dest.DataIncorporacao, opt => opt.MapFrom(src => src.DataIncorporacao ?? DateTime.MinValue))
            .ForMember(dest => dest.ValorCompra, opt => opt.MapFrom(src => src.ValorCompra ?? 0))
            .ForMember(dest => dest.Moeda, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Moeda) ? "BRL" : src.Moeda))
            .ForMember(dest => dest.Depreciacao, opt => opt.MapFrom(src => src.Depreciacao ?? 0.0m))
            .ForMember(dest => dest.ValorAtual, opt => opt.MapFrom(src => src.ValorAtual ?? 0))
            .ForMember(dest => dest.InfoAdicionais, opt => opt.MapFrom(src => src.InfoAdicionais));

        CreateMap<CentroPlanejamentoManutencaoComgasDto, CpmModel>()
            .ForMember(dest => dest.IdCPM, opt => opt.MapFrom(src => src.IdCPM))
            .ForMember(dest => dest.NomeCentro, opt => opt.MapFrom(src => src.NomeCentro ?? string.Empty))
            .ForMember(dest => dest.Organizacao, opt => opt.MapFrom(src => src.Organizacao ?? string.Empty))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao));

        CreateMap<CentroTrabalhoComgasDto, CtmModel>()
            .ForMember(dest => dest.IdGPM, opt => opt.MapFrom(src => src.IdGpm))
            .ForMember(dest => dest.AreaCTM, opt => opt.MapFrom(src => src.AreaCTM))
            .ForMember(dest => dest.IdCPM, opt => opt.MapFrom(src => src.IdCpm ?? 0))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao));

        CreateMap<GrupoPlanejamentoManutencaoComgasDto, GpmModel>()
            .ForMember(dest => dest.IdGPM, opt => opt.MapFrom(src => src.IdGpm))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao))
            .ForMember(dest => dest.IdCPM, opt => opt.MapFrom(src => src.IdCpm));

        CreateMap<AlmoxarifadoComgasDto, AlmoxarifadoModel>();

        CreateMap<FerramentaTipoComgasDto, FerramentaModel>()
            .ForMember(dest => dest.ValorCompra, opt => opt.MapFrom(src => src.ValorCompra ?? 0))
            .ForMember(dest => dest.DataAquisicao, opt => opt.MapFrom(src => src.DataAquisicao ?? DateTime.MinValue));
    }

}