using System.ComponentModel.DataAnnotations;

namespace ProjLogin.Controllers.controllerResults
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public class ActionResultBasic<T>
    {
        public bool Success { get; set; } //default value
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public T? Value { get; set; }

    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}


