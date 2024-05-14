using System;
using System.Collections.Generic;

namespace Inventario.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public string Rol { get; set; } = null!;    

    public string[] Roles { get; set; }      

    public int? IdRol { get; set; }

    public int? Status { get; set; }

    public virtual Role? IdRolNavigation { get; set; }

    public virtual Estado? StatusNavigation { get; set; }
}
