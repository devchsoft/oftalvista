namespace Oftalvista.Api.Contracts;

public class PaginatedItemsResponse<T>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int Count { get; set; }
    public IReadOnlyCollection<T> Data { get; set; } = Array.Empty<T>();
}

public class CatalogoItem
{
    public int Value { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Correo { get; set; } = string.Empty;
    public string ClaveHash { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public int IdUsuario { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public int IdTipoUsuario { get; set; }
    public string TipoUsuario { get; set; } = string.Empty;
}
