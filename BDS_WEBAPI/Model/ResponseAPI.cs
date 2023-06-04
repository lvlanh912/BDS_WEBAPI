namespace BDS_WEBAPI.Model
{
    public class ResponseAPI<T>
    {
        public bool Success { get; set; }
        public string message { get; set; }
        public T data { get; set; }

        public ResponseAPI(bool Success, string mess, T data)
        {
            this.Success = Success;
            this.message = mess;
            this.data = data;
        }

    }
}
