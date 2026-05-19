using Oftalvista.Api.Contracts;

namespace Oftalvista.Api.Helpers;

public static class Catalogos
{
    public static IReadOnlyCollection<CatalogoItem> TiposUsuario() =>
        new[]
        {
            new CatalogoItem { Value = 1, Text = "Administrador" },
            new CatalogoItem { Value = 2, Text = "Paciente" },
            new CatalogoItem { Value = 3, Text = "Medico" },
        };

    public static IReadOnlyCollection<CatalogoItem> TiposDocumento() =>
        new[]
        {
            new CatalogoItem { Value = 1, Text = "DNI" },
            new CatalogoItem { Value = 2, Text = "CE" },
            new CatalogoItem { Value = 3, Text = "Pasaporte" },
        };

    public static IReadOnlyCollection<CatalogoItem> EstadosVigencia() =>
        new[]
        {
            new CatalogoItem { Value = 1, Text = "Activo" },
            new CatalogoItem { Value = 2, Text = "Inactivo" },
        };

    public static IReadOnlyCollection<CatalogoItem> EstadosCita() =>
        new[]
        {
            new CatalogoItem { Value = 1, Text = "Programada" },
            new CatalogoItem { Value = 2, Text = "Confirmada" },
            new CatalogoItem { Value = 3, Text = "Atendida" },
            new CatalogoItem { Value = 4, Text = "Cancelada" },
        };

    public static IReadOnlyCollection<CatalogoItem> ModalidadesCita() =>
        new[]
        {
            new CatalogoItem { Value = 1, Text = "Presencial" },
            new CatalogoItem { Value = 2, Text = "Virtual" },
        };

    public static IReadOnlyCollection<CatalogoItem> MetodosPago() =>
        new[]
        {
            new CatalogoItem { Value = 1, Text = "Efectivo" },
            new CatalogoItem { Value = 2, Text = "Tarjeta" },
            new CatalogoItem { Value = 3, Text = "Transferencia" },
        };

    public static IReadOnlyCollection<CatalogoItem> EstadosPago() =>
        new[]
        {
            new CatalogoItem { Value = 1, Text = "Pendiente" },
            new CatalogoItem { Value = 2, Text = "Pagado" },
            new CatalogoItem { Value = 3, Text = "Anulado" },
        };

    public static IReadOnlyCollection<CatalogoItem> EstadosRecordatorio() =>
        new[]
        {
            new CatalogoItem { Value = 1, Text = "Pendiente" },
            new CatalogoItem { Value = 2, Text = "Enviado" },
            new CatalogoItem { Value = 3, Text = "Fallido" },
        };

    public static string TipoUsuarioTexto(int idTipoUsuario) =>
        TiposUsuario().FirstOrDefault(item => item.Value == idTipoUsuario)?.Text ?? "Usuario";
}
