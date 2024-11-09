using System.Security.Claims;

namespace SistemaSeguridad.Servicios
{

    public interface IServicioUsuarios
    {
        string ObtenerUsuarioId();
    }
    public class ServicioUsuarios: IServicioUsuarios
    {
        private readonly HttpContext httpContext;
        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext;
        }
        public string ObtenerUsuarioId() {

            if (httpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = httpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                var idUsuario = idClaim.Value;
                return idUsuario;
            }
            else 
            {
                throw new ApplicationException("El usuairo no esta autenticado");
            }
        }
    }
}
