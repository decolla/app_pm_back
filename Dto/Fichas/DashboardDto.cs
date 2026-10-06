using app_pm_back.Models.Domain;

namespace app_pm_back.Dto.Fichas;

public class DashboardDto
{
    public object? RelacaoStatusOrdens { get; set; }
    
    public object? NotasAbertasRelacaoPrazo { get; set; }
    
    public Dictionary<string, (int Notas, int Ordens)> TopLiOcorrencias { get; set;}
    
    public List<object> RelacaoStatusOrdensSemana { get; set;}
    
    public List<object> NotasAbertasRelacaoPrazoSemana { get; set;}
    
    public List<((long?, TimeSpan?), (long?, TimeSpan?))> RankingTempoVida { get; set; }
    
    public Dictionary<StatusFichas, int> NotasUsuario { get; set; }
}