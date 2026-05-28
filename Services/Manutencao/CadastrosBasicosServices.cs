using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services.Manutencao;

public class UsuarioService : BaseCrudService<UsuarioModel>
{
 public UsuarioService(AppDbContext context) : base(context)
 {
 }
}

public class CpmService : BaseCrudService<CpmModel>
{
 public CpmService(AppDbContext context) : base(context)
 {
 }
}

public class CtmService : BaseCrudService<CtmModel>
{
 public CtmService(AppDbContext context) : base(context)
 {
 }
}

public class GpmService : BaseCrudService<GpmModel>
{
 public GpmService(AppDbContext context) : base(context)
 {
 }
}
