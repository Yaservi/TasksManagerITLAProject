namespace DomainLayer.Dto
{
    public class Response
    {
        public bool ThereIsError => Errors.Any();
        public long EntityId { get; set; }
        public bool Successful { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>(0);
    }

    public class Response<T> : Response where T : class
    {
        public IEnumerable<T> DataList { get; set; }
        public T SingleData { get; set; }

        public static Response<T> SuccessResponse(T data)
        {
            return new Response<T>
            {
                Successful = true,
                SingleData = data,
                Message = string.Empty
            };
        }

        public static Response<T> ErrorResponse(string message)
        {
            return new Response<T>
            {
                Successful = false,
                SingleData = null,
                Message = message,
                Errors = new List<string> { message }
            };
        }
    }
}
