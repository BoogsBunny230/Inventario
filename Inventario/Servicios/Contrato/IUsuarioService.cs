using Microsoft.EntityFrameworkCore;
using Inventario.Models;

namespace Inventario.Servicios.Contrato
{
    public interface IUsuarioService
    {
        Task<Usuario> GetUsuario(string Correo, string Clave);

        Task<Usuario> SaveUsuario(Usuario modelo);

    }
}
