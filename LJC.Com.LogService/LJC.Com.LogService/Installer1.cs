using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace LJC.Com.LogService
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller processInstaller;

        public Installer1()
        {
            InitializeComponent();

            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            var configname = Assembly.GetExecutingAssembly().FullName.Split(',')[0] + ".exe.config";
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(configname);
            var servicename = doc.DocumentElement.SelectSingleNode("//appSettings//add[@key='ServiceName']").Attributes["value"].Value;
            var dispalyname = doc.DocumentElement.SelectSingleNode("//appSettings//add[@key='ServiceDisplayName']").Attributes["value"].Value;
            var description = doc.DocumentElement.SelectSingleNode("//appSettings//add[@key='ServiceDescription']").Attributes["value"].Value;

            serviceInstaller.ServiceName = servicename;
            serviceInstaller.Description = description;
            serviceInstaller.DisplayName = dispalyname;

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }
    }
}
