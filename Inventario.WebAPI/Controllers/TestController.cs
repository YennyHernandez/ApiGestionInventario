using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario.Infrastructure.Context;
using System.Threading.Tasks;

namespace Inventario.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ContextSql _context;

        public TestController(ContextSql context)
        {
            _context = context;
        }

        [HttpGet("db-connection")]
        public async Task<IActionResult> CheckDbConnection()
        {
            try
             {
                await _context.Database.OpenConnectionAsync();
                await _context.Database.CloseConnectionAsync();
                return Ok(new { success = true, message = "Conexión exitosa a la base de datos." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error de conexión: {ex.Message}" });
            }
        }
    }
}