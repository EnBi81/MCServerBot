namespace Application.DAOs.Database
{
    /// <summary>
    /// Handles database setup.
    /// </summary>
    public interface IDatabaseSetup
    {
        /// <summary>
        /// Sets up the database from zero, if the database is not set up already
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public Task Setup(string connectionString);
        /// <summary>
        /// Resets all the data in the database
        /// </summary>
        /// <returns></returns>
        public Task ResetDatabase();
    }
}
