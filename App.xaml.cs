using System.Configuration;
using System.Data;
using System.Net;
using System.Windows;


namespace ShoesStoreApp
{
    public partial class App : Application
    {
        public App()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }
    }
}
