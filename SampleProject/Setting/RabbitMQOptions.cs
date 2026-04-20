namespace SampleProject.API.Setting
{
    public class RabbitMQOptions
    {
        /// <summary>主機名稱</summary>
        public required string HostName { get; set; }

        /// <summary>連接埠</summary>
        public int Port { get; set; }

        /// <summary>使用者名稱</summary>
        public required string UserName { get; set; }

        /// <summary>密碼</summary>
        public required string Password { get; set; }
    }
}
