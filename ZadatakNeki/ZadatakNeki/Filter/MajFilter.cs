using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ZadatakNeki.Models;

namespace ZadatakNeki.Filter
{
    public class MajFilter : IResultFilter, IExceptionFilter
    {

        public void OnException(ExceptionContext context)
        {
                context.ExceptionHandled = true;

                VratiRezultat rezultat = new VratiRezultat()
                {
                    Error = new Error()
                    {
                        Message = "Desila se neka greska, snadji se videe sta je to.",
                        Exception = context.Exception.Message,
                        StackTrace = context.Exception.StackTrace
                    }
                };
                context.Result = new ObjectResult(rezultat);
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            ObjectResult result = (ObjectResult) context.Result;

            if (result.StatusCode >= 200 && result.StatusCode < 300)
            {
                VratiRezultat vrati = new VratiRezultat()
                {
                    Data = context.Result
                };
                context.Result = new ObjectResult(vrati);
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}

