using System;
using System.Configuration;
using System.ServiceProcess;
using Nancy.Hosting.Self;

namespace LimeProxy
{
    public partial class Service1 : ServiceBase
    {
        private NancyHost _nancyHost;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _nancyHost = new NancyHost(new Uri(ConfigurationManager.AppSettings["listen"]));
            _nancyHost.Start();
        }

        protected override void OnStop()
        {
            _nancyHost.Stop();
        }
    }
}
