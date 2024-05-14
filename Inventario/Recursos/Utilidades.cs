using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Inventario.Recursos
{
    public class Utilidades
    {

        public static string EncriptarClave (String clave)
        {
            StringBuilder sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;

                byte[] result = hash.ComputeHash(enc.GetBytes(clave));

                foreach (byte b in result)
                    sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

    }
}
