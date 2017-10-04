using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Response
{
    public class ErrorResult : Response<string>
    {
        public ErrorResult(string message)
        {
            IsSuccess = false;
            ErrorMessage = message;
            Result = null;
        }
    }
}
