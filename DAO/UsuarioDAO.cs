using Backend.Core;
using Backend.Database;
using Backend.Dtos;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.DAO
{
    public class UsuarioDAO
    {
        private readonly AppDbContext _context;
        private AccessDAO<Usuario> _usuarioDAO;
        private readonly UserManager<Authentication> _userManager;

        public UsuarioDAO(AppDbContext context, UserManager<Authentication> userManager)
        {
            _context = context;
            _usuarioDAO = new AccessDAO<Usuario>(_context);
            _userManager = userManager;
        }

        /// <summary>
        /// Obtenemos el los datos del usuario correspondiente con la ID de la tabla autenticacion
        /// </summary>
        /// <param name="id">Id de la tabla autenticacion</param>
        /// <returns></returns>
        public async Task<Usuario> GetUserId(string id) 
        {
            var result = await _context.Usuarios
                .SingleOrDefaultAsync(u => u.AuthenticationId == id);
            return result!;
        }

        public async Task<Usuario> ObtenerPorIdAsync(string id)
        {
            var result = await _usuarioDAO.ObtenerPorIdAsync(id);
            return result;
        }

        public async Task<bool> AgregarAsync(UsuarioDto usuarioDto, Authentication auth)
        {
            List<IRegla> reglas = new List<IRegla>();

            var data = new Usuario
            {
                DNI = usuarioDto.DNI,
                FechaNacimiento = usuarioDto.FechaNacimiento,
                Direccion = usuarioDto.Direccion,
                Ciudad = usuarioDto.Ciudad,
                Authentication = auth,
                AuthenticationId = auth.Id
            };

            var result = await _usuarioDAO.AgregarAsync(data, reglas);

            if (result)
            {
                auth.Usuario = data;
                await _userManager.UpdateAsync(auth);
                return true;
            } else
            {
                return false;
            }
        }

        public async Task<bool> ModificarAsync(string id, UsuarioDto usuarioDto, Authentication auth)
        {
            List<IRegla> reglas = new List<IRegla>();

            var user = new Usuario
            {
                Id = id,
                DNI = usuarioDto.DNI,
                FechaNacimiento = usuarioDto.FechaNacimiento,
                Direccion = usuarioDto.Direccion,
                Ciudad = usuarioDto.Ciudad,
                Authentication = auth,
                AuthenticationId = auth.Id
            };

            var result = await _usuarioDAO.ModificarAsync(id, user, reglas);

            if (result)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
