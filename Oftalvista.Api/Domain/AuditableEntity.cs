namespace Oftalvista.Api.Domain;

public abstract class AuditableEntity
{
    public int IdTblEstadoVigencia { get; set; }
    public Guid Guid { get; set; }
    public bool EsEliminado { get; set; }
    public string UsuarioCreacion { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public string IpCreacion { get; set; } = string.Empty;
    public string? UsuarioModificacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public string? IpModificacion { get; set; }
}
