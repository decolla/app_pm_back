using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain.Diretorios;

public class DiretorioModel
{
    /// <summary>
    /// Identificador do diretório
    /// </summary>
    [Key]
    public long IdDiretorio { get; set; }

    /// <summary>
    /// Nome diretório
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    public long? IdDiretorioPai { get; set; }

    [ForeignKey(nameof(IdDiretorioPai))]
    public virtual DiretorioModel? DiretorioPai { get; set; }

    public virtual ICollection<DiretorioModel> Subdiretorios { get; set; } = new List<DiretorioModel>();

    public virtual ICollection<NotaModel> NotasVinculadas { get; set; } = new List<NotaModel>();
    public virtual ICollection<DiretorioOrdemModel> OrdensVinculadas { get; set; } = new List<DiretorioOrdemModel>();
}

public class DiretorioOrdemModel
{
    public long IdDiretorio { get; set; }
    public virtual DiretorioModel Diretorio { get; set; }

    public long IdOrdem { get; set; }
    public virtual OrdemModel Ordem { get; set; }
}