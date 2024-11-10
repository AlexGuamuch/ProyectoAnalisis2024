using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SistemaSeguridad.Servicios
{
    public class CierreService 
    { private readonly string _connectionString; 
        public CierreService(IConfiguration configuration) {
            _connectionString = configuration.GetConnectionString("DefaultConnection"); }
        public async Task EjecutarProcedimientoCierre(DateTime inicio, DateTime fin)
        
        { using (SqlConnection conn = new SqlConnection(_connectionString)) 
            
            { using (SqlCommand cmd = new SqlCommand("[dbo].[cierremes]", conn)) 
                
                { 
                    cmd.CommandType = CommandType.StoredProcedure; 
                    cmd.Parameters.AddWithValue("@inicio", inicio); 
                    cmd.Parameters.AddWithValue("@fin", fin); await conn.OpenAsync(); await 
                        cmd.ExecuteNonQueryAsync(); 
                } 
            }
        }
    }
}
