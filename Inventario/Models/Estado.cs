using System;
using System.Collections.Generic;

namespace Inventario.Models;

public partial class Estado
{
    public int Status { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
