using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceProcess;

namespace Temps_du_laser
{
	public partial class LeService : ServiceBase
	{
		static readonly Locker LockerBackgroundWorker = new();
		
		public LeService()
		{
			InitializeComponent();
			InitialiserLeBackgroundWorker();
		}

		protected override void OnStart(string[] args)
		{
			MainBackgroundWorker.RunWorkerAsync();
		}

		protected override void OnStop()
		{
			MainBackgroundWorker.CancelAsync();
		}

		void InitialiserLeBackgroundWorker()
		{
			MainBackgroundWorker.DoWork += MainBackgroundWorker_DoWork;
			MainBackgroundWorker.RunWorkerCompleted += MainBackgroundWorker_RunWorkerCompleted;
		}

		private void MainBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (LockerBackgroundWorker.EstArrêté)
			{
				LockerBackgroundWorker.EstArrêté = false;
				MainBackgroundWorker.CancelAsync();
				MainBackgroundWorker.Dispose();
        // message: "Arrêt de [...] par l'utilisateur."
			}
			else
			{
        // message: "Fin de [...] qui a été complétée avec succès."
			}
			if (e.Error != null)
			{
				// message: $"Une erreur est survenue avec le MainBackgroundWorker{Environment.NewLine}{e.Error.Message}"
			}
		}

		private void MainBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			if (sender is BackgroundWorker)
			{
        // message: "Lancement de [...].",
				// Lancement de la tâche principale du service
        throw new NotImplementedException();
			}
		}
	}
}
