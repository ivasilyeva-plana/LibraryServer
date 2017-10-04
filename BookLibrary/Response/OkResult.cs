using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Response
{
    public class OkResult<T> : Response<T>
    {
        public OkResult(T result)
        {
            IsSuccess = true;
            ErrorMessage = string.Empty;
            Result = result;
        }
    }
}
