using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;
using System.Configuration;
using System.Data;
using System.Net;

namespace SistemaSeguridad.Servicios
{
	public class UsuarioStore : IUserStore<UsuarioPrueba>, IUserEmailStore<UsuarioPrueba>, IUserPasswordStore<UsuarioPrueba>
	{
		private readonly IRepositoryUsuarios repositoryUsuarios;
		private readonly string connectionString;

		public UsuarioStore(IRepositoryUsuarios repositoryUsuarios, IConfiguration configuration)
		{
			this.repositoryUsuarios = repositoryUsuarios;
			this.connectionString = configuration.GetConnectionString("DefaultConnection");
		}
		public async Task<IdentityResult> CreateAsync(UsuarioPrueba user, CancellationToken cancellationToken)
		{
			user.IdUsuario = await repositoryUsuarios.CrearUsuario(user);
			return IdentityResult.Success;
		}

		public Task<IdentityResult> DeleteAsync(UsuarioPrueba user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
		}

		public async Task<UsuarioPrueba> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
		{
			return await repositoryUsuarios.BuscarUsuarioEmail(normalizedEmail);
		}

		public async Task<UsuarioPrueba> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			return await repositoryUsuarios.BuscarUsuarioNombre(userId);
		}

		public async Task<UsuarioPrueba> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			return await repositoryUsuarios.BuscarUsuarioNombre(normalizedUserName);
		}

		public Task<string> GetEmailAsync(UsuarioPrueba user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.CorreoElectronico);
		}

		public Task<bool> GetEmailConfirmedAsync(UsuarioPrueba user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetNormalizedEmailAsync(UsuarioPrueba user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetNormalizedUserNameAsync(UsuarioPrueba user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetPasswordHashAsync(UsuarioPrueba user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.Password);
		}

		public Task<string> GetUserIdAsync(UsuarioPrueba user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.IdUsuario);
		}

		public Task<string> GetUserNameAsync(UsuarioPrueba user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.IdUsuario);
		}

		public Task<bool> HasPasswordAsync(UsuarioPrueba user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task SetEmailAsync(UsuarioPrueba user, string email, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task SetEmailConfirmedAsync(UsuarioPrueba user, bool confirmed, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task SetNormalizedEmailAsync(UsuarioPrueba user, string normalizedEmail, CancellationToken cancellationToken)
		{
			user.CorreoElectronico = normalizedEmail;
			return Task.CompletedTask;
		}

		public Task SetNormalizedUserNameAsync(UsuarioPrueba user, string normalizedName, CancellationToken cancellationToken)
		{
			//user.Nombre = normalizedName;
			return Task.CompletedTask;
		}

		public Task SetPasswordHashAsync(UsuarioPrueba user, string passwordHash, CancellationToken cancellationToken)
		{
			user.Password = passwordHash;
			return Task.CompletedTask;
		}

		public Task SetUserNameAsync(UsuarioPrueba user, string userName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public async Task<IdentityResult> UpdateAsync(UsuarioPrueba user, CancellationToken cancellationToken) //se agrego
		{
			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync(cancellationToken);

			int idStatusUsuario;

			if (user.IntentosDeAcceso >= 5)
			{
				idStatusUsuario = await ObtenerIdStatusUsuarioAsync("Bloqueado por intentos de acceso", cancellationToken);
				user.FechaDesbloqueo = DateTime.Now.AddMinutes(5);
			}
			else if (user.IntentosDeAcceso == 0)
			{
				idStatusUsuario = await ObtenerIdStatusUsuarioAsync("Activo", cancellationToken);
				user.FechaDesbloqueo = null;
			}
			else
			{
				idStatusUsuario = user.IdStatusUsuario; // Mantener el estado actual
			}

			// Asegurarse de que idStatusUsuario es válido
			if (idStatusUsuario == 0)
			{
				throw new InvalidOperationException("El IdStatusUsuario obtenido no es válido.");
			}

			// Actualizar los valores en la base de datos
			var query = @"UPDATE USUARIO 
                  SET IntentosDeAcceso = @IntentosDeAcceso, 
                      FechaDesbloqueo = @FechaDesbloqueo, 
                      IdStatusUsuario = @IdStatusUsuario 
                  WHERE IdUsuario = @IdUsuario";

			await connection.ExecuteAsync(query, new
			{
				user.IntentosDeAcceso,
				user.FechaDesbloqueo,
				IdStatusUsuario = idStatusUsuario,
				user.IdUsuario
			});

			return IdentityResult.Success;
		}



		public async Task<int> ObtenerIdStatusUsuarioAsync(string nombreStatus, CancellationToken cancellationToken)
		{
			using var connection = new SqlConnection(connectionString);
			await connection.OpenAsync(cancellationToken); ;

			var query = "SELECT IdStatusUsuario FROM Status_Usuario WHERE Nombre = @NombreStatus";
			return await connection.QuerySingleOrDefaultAsync<int>(query, new { NombreStatus = nombreStatus });
		}
	}
}
