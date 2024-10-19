using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;

namespace SistemaSeguridad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoCivilController : ControllerBase
    {
        private readonly IRepositoryEstadoCivil _repository;

        public EstadoCivilController(IRepositoryEstadoCivil repository)
        {
            _repository = repository;
        }

        // GET: api/EstadoCivil
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EstadoCivil>>> Get()
        {
            var estadosCiviles = await _repository.ObtenerTodos();
            return Ok(estadosCiviles);
        }

        // GET: api/EstadoCivil/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EstadoCivil>> Get(int id)
        {
            var estadoCivil = await _repository.ObtenerPorId(id);
            if (estadoCivil == null)
            {
                return NotFound();
            }
            return Ok(estadoCivil);
        }
    }
}
