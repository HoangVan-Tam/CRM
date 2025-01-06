namespace Entities.DTO
{
    public class FunctionResults<T> where T : class
    {
        public T Data;
        public string Message;
        public bool IsSuccess = true;
        public string Error;
    }
}
