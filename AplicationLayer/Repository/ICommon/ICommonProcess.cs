using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationLayer.Repository.ICommon
{
    public interface ICommonProcess<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetIdAsync(int id);
        Task<(bool IsSuccess, string Message)> AddAsync(T entry);
        Task<(bool IsSuccess, string Message)> UpdateAsync(T entry);
        Task<(bool IsSuccess, string Message)> DeleteAsync(int id);
    }
}
