using Backend.Core;
using Backend.Database;
using System.Data.Entity;

namespace Backend.DAO
{
    public class AccessDAO<TEntity> : IAccessDAO<TEntity> where TEntity : class
    {
        private readonly AppDbContext _context;
        
        public AccessDAO(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AgregarAsync(TEntity registro, List<IRegla> reglas)
        {
            foreach (var regla in reglas) 
            {
                if (!regla.EsCorrecto())
                {
                    return false;
                }
            }
            _context.Set<TEntity>().Add(registro);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BorrarAsync(string id, List<IRegla> reglas, string nombreTabla)
        {
            var registro = await ObtenerPorIdAsync(id);

            if (registro == null)
            {                
                return false;
            }

            foreach (var regla in reglas)
            {
                if (!regla.EsCorrecto())
                {
                    return false;
                }
            }

            _context.Set<TEntity>().Remove(registro);
            await _context.SaveChangesAsync();
            return true;
        }

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
                return false;
            }

            // Actualizar los valores de la entidad existente con los del nuevo registro
            _context.Entry(existingEntity).CurrentValues.SetValues(registro);

            // Guardar los cambios
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TEntity> ObtenerPorIdAsync(string id)
        {
            var result = await _context.Set<TEntity>().FindAsync(id);
            return result!;
        }

        public async Task<List<TEntity>> ObtenerTodoAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }
    }
}
