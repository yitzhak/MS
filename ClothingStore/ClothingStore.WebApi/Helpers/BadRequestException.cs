using ClothingStore.WebApi.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Helpers
{
    public class BadRequestException : Exception
    {
        public BadRequestException(InternalCode code, string message):base(message)
        {
            Code = code;
            ExceptionMessage = message;
        }

        public InternalCode Code { get; set; }
        public string ExceptionMessage { get; set; }
    }
}
