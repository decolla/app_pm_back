using app_pm_back.Data;
using app_pm_back.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

public class AnnexService
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<AnnexModel> _annexModels;

    public AnnexService(AppDbContext context)
    {
        _context = context;
        _annexModels = context.Set<AnnexModel>();
    }

    public async Task<AnnexModel> UploadAnexo(IFormFile file, long referenciaId, string tipoReferencia)
    {
        if (file != null && file.Length > 0)
        {
            byte[] fileBytes = null;

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                stream.Position = 0;

                fileBytes = stream.ToArray();
            }

            if (fileBytes == null) return null;

            AnnexModel annexModel = new AnnexModel
            {
                NomeAnexo = file.FileName,
                MimeType = file.ContentType,
                Dados = fileBytes,
                ReferenciaId = referenciaId,
                TipoReferencia = tipoReferencia,
                DataUpload = DateTime.Now
            };

            _annexModels.Add(annexModel);
            await _context.SaveChangesAsync();

            return annexModel;
        }

        return null;
    }

    public async Task<AnnexModel> DownloadAnexo(long id)
    {
        var annexModel = await _annexModels.FindAsync(id);
        return annexModel;
    }

    public async Task<List<(long, string)>> BuscarAnexo(long referenciaId, string tipoReferencia)
    {
        List<(long, string)> list = new List<(long, string)>();

        var query = await _annexModels.Where(o => o.ReferenciaId == referenciaId &&
                                                  o.TipoReferencia == tipoReferencia).ToListAsync();

        foreach (var model in query)
        {
            list.Add((model.IdAnexo, model.NomeAnexo));   
        }
        
        return list;
    }
}