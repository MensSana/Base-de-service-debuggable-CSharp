using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace ServiceDemo
{
	static partial class Program
	{
		/// <summary>
		/// Configurations du service
		/// </summary>
		const string NomDuService = "ServiceDemo";
		const string NomDAffichage = "Démonstration de Service";
		const ServiceStartMode TypeDeDémarrage = ServiceStartMode.Manual;
		const ServiceAccount CompteDeService = ServiceAccount.LocalSystem;
		/// <summary>
		/// Observateur d'évènements
		/// </summary>
		const string NomDuJournal = "Compagnie";
		const string SourceDuJournal = "ServiceDemo";
		
		static void Main(string[] args)
		{
			if (Debugger.IsAttached)
				LancerEnModeDebuggable();
			else if (Environment.UserInteractive)
				LancerEnModeConsole(args);
			else
				LancerEnModeService();
		}
	}
}
