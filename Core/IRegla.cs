namespace Backend.Core
{
    /// <summary>
    /// Permite agregar reglas
    /// </summary>
    public interface IRegla
    {
        /// <summary>
        /// Permite validar una regla
        /// </summary>
        /// <returns></returns>
        bool EsCorrecto();
    }
}
