using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greggs.Products.Api.DataAccess;

public interface IDataAccess<T> where T : class
{
  Task<IEnumerable<T>> List(int? pageStart, int? pageSize);
}