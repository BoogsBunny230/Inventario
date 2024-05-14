using Microsoft.EntityFrameworkCore;
using Inventario.Models;
using Inventario.Servicios.Contrato;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Inventario.Servicios.Implementacion
{
    public class UsuarioService : IUsuarioService
    {

        private readonly Inventario1Context _dbContext;

        public UsuarioService(Inventario1Context dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Usuario> GetUsuario(string correo, string clave)
        {
            Usuario usuario_encontrado = await _dbContext.Usuarios.Where(u =>u.Correo==correo&&u.Clave==clave)
            .FirstOrDefaultAsync();

            return usuario_encontrado;
        }

        public async Task<Usuario> SaveUsuario(Usuario modelo)
        {
            _dbContext.Usuarios.Add(modelo);
            await _dbContext.SaveChangesAsync();
            return modelo;
        }
    }
}
