using Backend.Dtos;

namespace Backend.Interfaces
{
    /// <summary>
    /// Interfaz de la funcion del envio de email
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// Validacion de la informacion del usuario
        /// </summary>
        /// <param name="hTMLMailDto"></param>
        /// <param name="templateUse"></param>
        /// <returns></returns>
        bool SendHTMLMail(HTMLMailDto hTMLMailDto, string templateUse);
    }
    
}
