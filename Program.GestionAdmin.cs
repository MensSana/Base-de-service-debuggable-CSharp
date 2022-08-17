using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceDemo
{
	static partial class Program
	{
		static void LancerEnModeDebuggable()
		{
			using LeService service = new();
			// Appeler la méthode qu'appellerait le service
			throw new NotImplementedException();
		}

		static void LancerEnModeConsole(string[] args)
		{
			string[] arguments = new[]
			{
				"-installer",
				"/installer",
				"-désinstaller",
				"/désinstaller"
			};
			string commande = arguments.Intersect(args).FirstOrDefault();

			if (commande != null)
				try
				{
					AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
					GérerLInstallation(commande);
				}
				catch (SecurityException)
				{
					RéExécuterEnTantQuAdmin(commande);
				}
			else
				Console.WriteLine("Ceci est un service et vous ne pouvez l'utiliser en ligne de commande qu'avec les arguments /installer et /désinstaller");
		}

		static void LancerEnModeService()
		{
			ServiceBase[] ServicesToRun = new ServiceBase[]
			{
				new LeService()
			};
			ServiceBase.Run(ServicesToRun);
		}

		[PrincipalPermissionAttribute(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
		static void GérerLInstallation(string commande)
		{
			string titre = "", message = "";
			switch (commande)
			{
				case "-installer":
				case "/installer":
					AjouterLaJournalisationÀLObservateur();
					InstallateurDeService.Install(new Hashtable());

					titre = "Installation";
					message = "Service installé et journal des évènements initialisé.";
					break;
				case "-désinstaller":
				case "/désinstaller":
					SupprimerLaJournalisationDeLObservateur();
					InstallateurDeService.Uninstall(null);

					titre = "Désinstallation";
					message = "Service désinstallé et journal des évènements supprimé.";
					break;
			}
			MessageBox.Show(
				caption: $"{titre} complétée",
				text: message
			);
		}

		static void RéExécuterEnTantQuAdmin(string commande)
		{
			try
			{
				ProcessStartInfo startInfo = new ProcessStartInfo()
				{
					UseShellExecute = true,
					FileName = Assembly.GetExecutingAssembly().Location,
					Arguments = commande,
					WindowStyle = ProcessWindowStyle.Hidden,
					Verb = "runas",
				};
				Process.Start(startInfo);
			}
			catch
			{
				Console.WriteLine("Les privilèges d'administrateur sont requis pour effectuer cette opération.");
			}
		}

		static Installer InstallateurDeService
		{
			get
			{
				var installateur = new TransactedInstaller();
				installateur.Installers.Add(new ServiceInstaller
				{
					ServiceName = NomDuService,
					DisplayName = NomDAffichage,
					StartType = TypeDeDémarrage
				});
				installateur.Installers.Add(new ServiceProcessInstaller
				{
					Account = CompteDeService
				});
				var installContext = new InstallContext(
						NomDuService + ".install.log", null);
				installContext.Parameters["assemblypath"] =
						Assembly.GetEntryAssembly().Location;
				installateur.Context = installContext;
				return installateur;
			}
		}
		
		/// <summary>
		/// Prépare la journalisation dans le EventViewer
		/// et dans le système de fichiers
		/// </summary>
		public static void AjouterLaJournalisationÀLObservateur()
		{
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			Console.WriteLine("");
			Console.WriteLine(" =====");
			Console.WriteLine($" ===== Démarrage ===== {DateTime.Now:yyyy-MM-dd HH:mm:ss} =====");
			Console.WriteLine(" =====");
			Console.WriteLine("");

			JournalÉvènementiel = new EventLog()
			{
				Source = SourceDuJournal,
				Log = NomDuJournal
			};
			if (Environment.UserInteractive)
			{
				_InclureLesEntêtes = true;
				try
				{
					if (!EventLog.SourceExists(LogSource))
					{
						try
						{
							EventLog.CreateEventSource(LogSource, LogName);
						}
						catch (Exception ex)
						{
							Trace.WriteLine(ex.Message);
						}
					}
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.Message);
				}
			}
		}

		public static void SupprimerLaJournalisationDeLObservateur()
		{
			EventLog.Delete(NomDuJournal);
		}
	}
}
