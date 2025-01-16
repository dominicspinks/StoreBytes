
namespace StoreBytesLibrary.Databases
{
    public interface IPGSqlDataAccess
    {
        List<T> LoadData<T, U>(string sqlStatement, U parameters, dynamic? options = null);
        void SaveData<T>(string sqlStatement, T parameters, dynamic? options = null);
    }
}