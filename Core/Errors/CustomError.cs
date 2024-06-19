namespace Backend.Core.Errors
{
    /// <summary>
    /// Mensjaes de error 
    /// </summary>
    public class CustomError
    {
        /// <summary>
        /// Titulo del error
        /// </summary>
        public string? title;

        /// <summary>
        /// Codigo del error
        /// </summary>
        public int status;

        /// <summary>
        /// Mensaje del error
        /// </summary>
        public Dictionary<string, List<string>>? errors { get; set; }

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="title"></param>
        /// <param name="status"></param>
        /// <param name="errors"></param>
        public CustomError(string? title, int status, Dictionary<string, List<string>>? errors)
        {
            this.title = title;
            this.status = status;
            this.errors = errors;
        }
    }
}
