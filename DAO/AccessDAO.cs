﻿using Backend.Core;
using Backend.Core.Errors;
using Backend.Database;
using Backend.Resources;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data.Entity;

namespace Backend.DAO
{
    /// <summary>
    /// Funciones para acceder a cualquier table de la base de datos
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class AccessDAO<TEntity> : IAccessDAO<TEntity> where TEntity : class
    {
        private readonly AppDbContext _context;
        private readonly LocService _locService;

        /// <summary>
        /// Mensaje de error personalizado
        /// </summary>
        public CustomError? customError;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="context"></param>
        /// <param name="localizer"></param>
        public AccessDAO(AppDbContext context, LocService localizer)
        {
            _context = context;
            _locService = localizer;
        }

        CustomError IAccessDAO<TEntity>.customError { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Permite agregar nuevos registros
        /// </summary>
        /// <param name="registro"></param>
        /// <param name="reglas"></param>
        /// <returns></returns>
        public async Task<bool> AgregarAsync(TEntity registro, List<IRegla> reglas)
        {
            foreach (var regla in reglas) 
            {
                if (!regla.EsCorrecto())
                {
                    customError = regla.customError;
                    return false;
                }
            }
            _context.Set<TEntity>().Add(registro);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Permite eliminar un registro por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reglas"></param>
        /// <param name="nombreTabla"></param>
        /// <returns></returns>
        public async Task<bool> BorrarAsync(string id, List<IRegla> reglas, string nombreTabla)
        {
            var registro = await ObtenerPorIdAsync(id);

            if (registro == null)
            {
                customError = new CustomError("No encontrado", 404, new Dictionary<string, List<string>>
                {
                    { "NotFound", new List<string>{ String.Format(_locService.GetLocalizedString("NotFound"))} }
                });
                return false;
            }

            foreach (var regla in reglas)
            {
                if (!regla.EsCorrecto())
                {
                    customError = regla.customError;
                    return false;
                }
            }

            _context.Set<TEntity>().Remove(registro);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Permite actualizar un registro por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="registro"></param>
        /// <param name="reglas"></param>
        /// <returns></returns>
        public async Task<bool> ModificarAsync(string id, TEntity registro, List<IRegla> reglas)
        {
            foreach (var regla in reglas)
            {
                if (!regla.EsCorrecto())
                {
                    return false;
                }
            }

            var existingEntity = await _context.Set<TEntity>().FindAsync(id);
            if (existingEntity == null)
            {
                customError = new CustomError("No encontrado", 404, new Dictionary<string, List<string>>
                {
                    { "NotFound", new List<string>{ String.Format(_locService.GetLocalizedString("NotFound")) } }
                });
                return false;
            }

            // Actualizar los valores de la entidad existente con los del nuevo registro
            _context.Entry(existingEntity).CurrentValues.SetValues(registro);

            // Guardar los cambios
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Permite obtener un registro por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TEntity> ObtenerPorIdAsync(string id)
        {
            var result = await _context.Set<TEntity>().FindAsync(id);
            return result!;
        }

        /// <summary>
        /// Permite obtener todos los registros almacenados
        /// </summary>
        /// <returns></returns>
        public async Task<List<TEntity>> ObtenerTodoAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }
    }
}
