using Microsoft.AspNetCore.Mvc;

using Inventario.Models;
using Inventario.Recursos;
using Inventario.Servicios.Contrato;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


namespace Inventario.Controllers;

    public class InicioController : Controller
    {
    private readonly IUsuarioService _usuarioServicio;    
    public InicioController(IUsuarioService usuarioServicio)
    {
        _usuarioServicio=usuarioServicio;
    }

        public IActionResult Registrarse()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registrarse(Usuario modelo)
    {
        modelo.Clave = Utilidades.EncriptarClave(modelo.Clave);

        Usuario usuario_creado = await _usuarioServicio.SaveUsuario(modelo);
        if (usuario_creado.IdUsuario > 0)
            return RedirectToAction("IniciarSesion", "Inicio");

        ViewData["Mensaje"] = "No se pudo crear el usuario";
        return View();
        }


        public IActionResult IniciarSesion()
        {
            return View();
    }
    [HttpPost]
    public async Task<IActionResult> IniciarSesion(string Correo, string Clave)
    {
        Usuario usuario_encontrado = await _usuarioServicio.GetUsuario(Correo, Utilidades.EncriptarClave(Clave));

        if (usuario_encontrado == null)
        {
            ViewData["Mensaje"] = "No se encontraron coincidencias";
            return View();
        }

        // Crear reclamación para el ID del usuario
        List<Claim> claims = new List<Claim>()
    {
        new Claim(ClaimTypes.NameIdentifier, usuario_encontrado.IdUsuario.ToString()), // Almacena el ID del usuario
        new Claim(ClaimTypes.Name, usuario_encontrado.Nombre)
    };

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        AuthenticationProperties propieties = new AuthenticationProperties()
        {
            AllowRefresh = true
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            propieties
        );

        return RedirectToAction("Index", "Home");
    }
}
