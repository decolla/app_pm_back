using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using AutoMapper;

namespace app_pm_back.Profiles;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<NotaDto, NotaModel>()
            .ForMember(dest => dest.IdNota, opt => opt.Ignore())
            .ForMember(dest => dest.Ordens, opt => opt.Ignore());

        CreateMap<OrdemDto, OrdemModel>()
            .ForMember(dest => dest.Nota, opt => opt.Ignore());

        CreateMap<EstrategiaManutencaoDto, EstrategiaModel>();

        CreateMap<TipoEstrategiaDto, TipoEstrategiaModel>()
            .ForMember(dest => dest.Estrategia, opt => opt.Ignore())
            .ForMember(dest => dest.TipoManutencao, opt => opt.Ignore())
            .ForMember(dest => dest.TipoCep, opt => opt.Ignore());

        CreateMap<PlanoManutencaoDto, PlanoModel>()
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome ?? string.Empty))
            .ForMember(dest => dest.Estrategia, opt => opt.Ignore());

        CreateMap<TipoPlanoDto, TipoPlanoModel>()
            .ForMember(dest => dest.TipoEstrategia, opt => opt.Ignore())
            .ForMember(dest => dest.Plano, opt => opt.Ignore())
            .ForMember(dest => dest.TipoManutencao, opt => opt.Ignore())
            .ForMember(dest => dest.TipoCep, opt => opt.Ignore())
            .ForMember(dest => dest.Ordem, opt => opt.Ignore())
            .ForMember(dest => dest.Nota, opt => opt.Ignore());

        CreateMap<TipoManutencaoDto, TipoManutencaoModel>();

        CreateMap<TarefaDto, OperacaoModel>()
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao))
            .ForMember(dest => dest.Duracao, opt => opt.MapFrom(src => src.Duracao));

        CreateMap<OperacaoOrdemDto, OperacaoModel>()
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao))
            .ForMember(dest => dest.Duracao, opt => opt.MapFrom(src => src.Duracao));


        CreateMap<TipoManutencaoCepDto, TipoCepModel>()
            .ForMember(dest => dest.AcoesFalha, opt => opt.MapFrom(src => src.AcoesFalha));

        CreateMap<UsuarioDto, UsuarioModel>()
            .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.IdUsuario))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.CPF, opt => opt.MapFrom(src => src.Cpf))
            .ForMember(dest => dest.Permissoes, opt => opt.MapFrom(src => src.Permissoes))
            .ForMember(dest => dest.Escopos, opt => opt.MapFrom(src => src.Escopos));

        CreateMap<ImobilizadoDto, ImobilizadoModel>()
            .ForMember(dest => dest.IdImobilizado, opt => opt.MapFrom(src => src.IdImobilizado ?? 0))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome ?? string.Empty))
            .ForMember(dest => dest.Nserie, opt => opt.MapFrom(src => src.Nserie ?? string.Empty))
            .ForMember(dest => dest.Quantidade,
                opt => opt.MapFrom(src => src.Quantidade.HasValue ? src.Quantidade.Value : 0))
            .ForMember(dest => dest.DataAquisicao, opt => opt.MapFrom(src => src.DataAquisicao ?? DateTime.MinValue))
            .ForMember(dest => dest.DataIncorporacao,
                opt => opt.MapFrom(src => src.DataIncorporacao ?? DateTime.MinValue))
            .ForMember(dest => dest.ValorCompra, opt => opt.MapFrom(src => src.ValorCompra ?? 0))
            .ForMember(dest => dest.Moeda,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Moeda) ? "BRL" : src.Moeda))
            .ForMember(dest => dest.Depreciacao, opt => opt.MapFrom(src => src.Depreciacao ?? 0.0m))
            .ForMember(dest => dest.ValorAtual, opt => opt.MapFrom(src => src.ValorAtual ?? 0))
            .ForMember(dest => dest.InfoAdicionais, opt => opt.MapFrom(src => src.InfoAdicionais));

        CreateMap<CentroPlanejamentoManutencaoDto, CpmModel>()
            .ForMember(dest => dest.IdCPM, opt => opt.MapFrom(src => src.IdCPM))
            .ForMember(dest => dest.NomeCentro, opt => opt.MapFrom(src => src.NomeCentro ?? string.Empty))
            .ForMember(dest => dest.Organizacao, opt => opt.MapFrom(src => src.Organizacao ?? string.Empty))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao));

        CreateMap<CentroTrabalhoDto, CtmModel>()
            .ForMember(dest => dest.IdCTM, opt => opt.MapFrom(src => src.IdCtm))
            .ForMember(dest => dest.AreaCTM, opt => opt.MapFrom(src => src.AreaCTM))
            .ForMember(dest => dest.IdCPM, opt => opt.MapFrom(src => src.IdCpm ?? 0))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao));

        CreateMap<GrupoPlanejamentoManutencaoDto, GpmModel>()
            .ForMember(dest => dest.IdGPM, opt => opt.MapFrom(src => src.IdGpm))
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao))
            .ForMember(dest => dest.IdCPM, opt => opt.MapFrom(src => src.IdCpm));

        CreateMap<AlmoxarifadoDto, AlmoxarifadoModel>();

        CreateMap<FerramentaTipoDto, FerramentaModel>()
            .ForMember(dest => dest.ValorCompra, opt => opt.MapFrom(src => src.ValorCompra ?? 0))
            .ForMember(dest => dest.DataAquisicao, opt => opt.MapFrom(src => src.DataAquisicao ?? DateTime.MinValue));

        CreateMap<DadosRenovacaoDto, DadosRenovacaoModel>();

        CreateMap<ItemNotaManutencaoDto, ItemNotaModel>();

        CreateMap<VariavelNotaDto, VariavelNotaModel>();

        CreateMap<AnexoNotaDto, AnexoNotaModel>();

        CreateMap<AnexoOrdemDto, AnexoOrdemModel>();
        
        CreateMap<RegraClassificacaoDto, RegraClassificacaoModel>();
    }
}