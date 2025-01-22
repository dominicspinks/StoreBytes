
using System.Data;

namespace StoreBytes.DataAccess.Databases
{
    public interface IPGSqlDataAccess
    {
        IDbTransaction BeginTransaction();
        T ExecuteTransaction<T>(Func<IDbTransaction, T> transactionalOperation);
        List<T> LoadData<T, U>(string sqlStatement, U parameters, dynamic? options = null, IDbTransaction? transaction = null);
        int SaveData<T>(string sqlStatement, T parameters, dynamic? options = null, IDbTransaction? transaction = null);
    }
}