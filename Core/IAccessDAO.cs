using Backend.Core.Errors;

namespace Backend.Core
{
    /// <summary>
    /// Interface para acceso a datos
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAccessDAO<T> where T : class
    {
        /// <summary>
        /// Errores
        /// </summary>
        CustomError customError { get; set; }

        /// <summary>
        /// Obtiene todos los registros
        /// </summary>
        /// <returns></returns>
        Task<List<T>> ObtenerTodoAsync();

        /// <summary>
        /// Obtiene un registro de acuerdo a su Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> ObtenerPorIdAsync(string id);

        /// <summary>
        /// Agrega un registro
        /// </summary>
        /// <param name="registro">Datos del registro</param>
        /// <param name="reglas">Reglas para agregar</param>
        /// <returns></returns>
        Task<bool> AgregarAsync(T registro, List<IRegla> reglas);

        /// <summary>
        /// Modifica un registro
        /// </summary>
        /// <param name="id">Id del registro a modificar</param>
        /// <param name="registro">Datos del registro a modificar</param>
        /// <param name="reglas">Reglas a validar</param>
        /// <returns></returns>
        Task<bool> ModificarAsync(string id, T registro, List<IRegla> reglas);

        /// <summary>
        /// Borra un registro
        /// </summary>
        /// <param name="id">Id del registro</param>
        /// <param name="reglas">Reglas para borrar</param>
        /// <param name="nombreTabla">Nombre para la tabla de forma
        /// amigable para el usuario</param>
        /// <returns></returns>
        Task<bool> BorrarAsync(string id, List<IRegla> reglas, string nombreTabla);
    }
}
