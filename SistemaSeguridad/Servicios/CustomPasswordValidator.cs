using Microsoft.AspNetCore.Identity;
using SistemaSeguridad.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaSeguridad.Servicios
{
	public class CustomPasswordValidator : IPasswordValidator<UsuarioPrueba>
	{
		private readonly IRepositoyEmpresa _repositoryPasswordRules;

		public CustomPasswordValidator(IRepositoyEmpresa repositoryPasswordRules)
		{
			_repositoryPasswordRules = repositoryPasswordRules;
		}

		public async Task<IdentityResult> ValidateAsync(UserManager<UsuarioPrueba> manager, UsuarioPrueba user, string password)
		{
			// Obtener las reglas de validación de la base de datos
			var passwordRules = await _repositoryPasswordRules.ObtenerPasswordRules();

			var errors = new List<IdentityError>();

			// Verificar longitud mínima
			if (password.Length < passwordRules.PasswordLargo)
			{
				errors.Add(new IdentityError { Description = $"La contraseña debe tener al menos {passwordRules.PasswordLargo} caracteres." });
			}

			// Verificar si contiene el número mínimo de mayúsculas
			if (passwordRules.PasswordCantidadMayusculas > 0 && password.Count(char.IsUpper) < passwordRules.PasswordCantidadMayusculas)
			{
				errors.Add(new IdentityError { Description = $"La contraseña debe contener al menos {passwordRules.PasswordCantidadMayusculas} letras mayúsculas." });
			}

			// Verificar si contiene minúsculas
			if (passwordRules.PasswordCantidadMinusculas > 0 && password.Count(char.IsLower) < passwordRules.PasswordCantidadMinusculas)
			{
				errors.Add(new IdentityError { Description = $"La contraseña debe contener al menos {passwordRules.PasswordCantidadMinusculas} letras minúsculas." });
			}

			// Verificar si contiene dígitos
			if (passwordRules.PasswordCantidadNumeros > 0 && password.Count(char.IsDigit) < passwordRules.PasswordCantidadNumeros)
			{
				errors.Add(new IdentityError { Description = $"La contraseña debe contener al menos {passwordRules.PasswordCantidadNumeros} números." });
			}

			// Verificar si contiene caracteres especiales
			if (passwordRules.PasswordCantidadCaracteresEspeciales > 0 && password.Count(ch => !char.IsLetterOrDigit(ch)) < passwordRules.PasswordCantidadCaracteresEspeciales)
			{
				errors.Add(new IdentityError { Description = $"La contraseña debe contener al menos {passwordRules.PasswordCantidadCaracteresEspeciales} caracteres especiales." });
			}

			if (errors.Count == 0)
			{
				return IdentityResult.Success;
			}

			return IdentityResult.Failed(errors.ToArray());
		}
	}
}
