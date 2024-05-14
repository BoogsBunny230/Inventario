using Inventario.Models;


namespace Inventario.Data


{
    public class DA_Logica
    {

        public List<Usuario1> ListaUsuarios()
        {
            return new List<Usuario1>
            {
                new Usuario1 { Nombre = "Guillermo Leonel", Correo = "guille1212@gmail.com", Clave = "123123", Roles = new[] { "Administrador" } },
                new Usuario1 { Nombre = "Jose Jose", Correo = "guille2121@gmail.com", Clave = "321321", Roles = new[] { "Almacenista" } }

            };

        }


        public Usuario1 ValidarUsuario (String _correo, string _clave)
        {
            return ListaUsuarios().Where(item => item.Correo == _correo && item.Clave == _clave).FirstOrDefault();
        }


    }

}


