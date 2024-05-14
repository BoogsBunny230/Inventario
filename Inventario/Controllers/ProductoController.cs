using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Inventario.Controllers
{
    public class ProductoController : Controller
    {
        private readonly Inventario1Context _context;

        public ProductoController(Inventario1Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string filtro)
        {
            // Recargar la lista de productos si se realizó una salida
            if (TempData.ContainsKey("SalidaRealizada"))
            {
                TempData.Remove("SalidaRealizada"); // Limpiar TempData
                                                    // Recargar la lista de productos
                var productosQuery = _context.Productos.AsQueryable();

                switch (filtro)
                {
                    case "Activos":
                        productosQuery = productosQuery.Where(p => p.Activo);
                        break;
                    case "Inactivos":
                        productosQuery = productosQuery.Where(p => !p.Activo);
                        break;
                    case "Todos":
                        // No aplicamos ningún filtro adicional, mostramos todos los productos
                        break;
                    default:
                        break;
                }

                var productos = await productosQuery.ToListAsync();
                return View(productos);
            }

            // Si no se realizó una salida, mostrar la vista Index normalmente
            var todosLosProductos = await _context.Productos.ToListAsync();
            return View(todosLosProductos);


        }




        // GET: Producto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Producto/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Producto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NombreProd, Activo")] Producto producto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    producto.Cantidad = 0; // Establecer la cantidad inicial en cero
                    _context.Add(producto);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                // Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "No se pudo guardar el nuevo producto. Intenta de nuevo.");
            }
            return View(producto);
        }



        // GET: Producto/Edit/5
        public async Task<IActionResult> Edit(int? id, string accion)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["accion"] = accion;
            return View(producto);
        }

        // POST: Producto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreProd,Cantidad,Activo")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index)); // Redireccionar al índice después de guardar los cambios
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(producto);
        }


        // GET: Producto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Producto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                // Dar de baja el producto en lugar de eliminarlo
                producto.Activo = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Producto/Entrada/5
        public async Task<IActionResult> Entrada(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Producto/Entrada/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Entrada(int id, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                if (cantidad > 0)
                {
                    if (cantidad <= producto.Cantidad)
                    {
                        // Mostrar un mensaje de error si la cantidad de entrada es mayor que la cantidad actual
                        TempData["Error"] = $"La cantidad de entrada ({cantidad}) no puede ser mayor que la cantidad actual ({producto.Cantidad}).";
                        return RedirectToAction(nameof(Entrada), new { id = id });
                    }

                    // Realizar la entrada de productos
                    producto.Cantidad = cantidad;
                    await _context.SaveChangesAsync();

                    // Agregar registro al historial de movimientos
                    var usuarioId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Obtener el ID del usuario actual
                    var movimientoEntrada = new MovimientoProducto
                    {
                        ProductoId = producto.Id,
                        Tipo = TipoMovimiento.Entrada,
                        Cantidad = cantidad,
                        FechaHora = DateTime.Now, // Fecha y hora actual
                        UsuarioId = usuarioId, // ID del usuario actual
                        RealizadoPor = usuarioId // Cambiar "Usuario" por el nombre del usuario actual si lo tienes disponible
                    };
                    _context.MovimientosProductos.Add(movimientoEntrada);
                    await _context.SaveChangesAsync();

                    // Mostrar un mensaje de confirmación
                    TempData["Mensaje"] = $"Se han agregado {cantidad} unidades al inventario del producto '{producto.NombreProd}'.";

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // La cantidad debe ser mayor que cero, mostrar mensaje de error
                    TempData["Error"] = "La cantidad debe ser mayor que cero.";
                    return RedirectToAction(nameof(Entrada), new { id = id });
                }
            }
            return RedirectToAction(nameof(Index));
        }


        // GET: Producto/Salida/5
        public async Task<IActionResult> Salida(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            ViewBag.ProductId = id; // Pasar el Id del producto a la vista
            ViewBag.CantidadActual = producto.Cantidad; // Pasar la cantidad actual del producto a la vista

            return View(producto);
        }

        // POST: Producto/Salida/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Salida(int id, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                if (cantidad > 0 && cantidad <= producto.Cantidad)
                {
                    // Restar la cantidad de productos del inventario
                    producto.Cantidad -= cantidad;
                    await _context.SaveChangesAsync(); // Guardar los cambios en la base de datos

                    // Agregar registro al historial de movimientos
                    var usuarioId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Obtener el ID del usuario actual
                    var movimientoSalida = new MovimientoProducto
                    {
                        ProductoId = producto.Id,
                        Tipo = TipoMovimiento.Salida,
                        Cantidad = cantidad,
                        FechaHora = DateTime.Now, // Fecha y hora actual
                        UsuarioId = usuarioId, // ID del usuario actual
                        RealizadoPor = usuarioId // Cambiar "Usuario" por el nombre del usuario actual si lo tienes disponible
                    };
                    _context.MovimientosProductos.Add(movimientoSalida);
                    await _context.SaveChangesAsync();

                    // Mostrar un mensaje de confirmación
                    TempData["Mensaje"] = $"Se han sacado {cantidad} unidades del inventario del producto '{producto.NombreProd}'.";

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // La cantidad de salida no es válida, mostrar mensaje de error
                    TempData["Error"] = "La cantidad de salida no es válida.";
                    return RedirectToAction(nameof(Salida), new { id = id });
                }
            }
            return RedirectToAction(nameof(Index));
        }






        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }

        // GET: Producto/HistorialMovimientos
        public IActionResult HistorialMovimientos()
        {
            var movimientos = _context.MovimientosProductos.Include(m => m.Producto).ToList();
            return View(movimientos);
        }

        // POST: Producto/FiltrarMovimientos
        [HttpPost]
        public IActionResult FiltrarMovimientos(string tipoMovimiento)
        {
            if (Enum.TryParse(tipoMovimiento, out TipoMovimiento tipo))
            {
                var movimientos = _context.MovimientosProductos.Include(m => m.Producto)
                                    .Where(m => m.Tipo == tipo)
                                    .ToList();
                return View("HistorialMovimientos", movimientos);
            }
            else
            {
                // Manejo de error: La cadena no pudo convertirse a un valor de TipoMovimiento válido
                return RedirectToAction(nameof(Index)); // Otra acción adecuada en caso de error
            }
        }


        // GET: Producto/DarDeBaja/5
        public async Task<IActionResult> DarDeBaja(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Producto/DarDeBaja/5
        [HttpPost, ActionName("DarDeBaja")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarDarDeBaja(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                // Dar de baja el producto actualizando su estado
                producto.Activo = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        // GET: Producto/ActivosInactivos
        public async Task<IActionResult> ActivosInactivos()
        {
            var productosActivos = await _context.Productos.Where(p => p.Activo).ToListAsync();
            var productosInactivos = await _context.Productos.Where(p => !p.Activo).ToListAsync();

            var viewModel = new Tuple<List<Producto>, List<Producto>>(productosActivos, productosInactivos);

            return View(viewModel);
        }
    }
}
