/* Copyright 2017 Cimpress

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License. */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows.Threading;
using Caliburn.Micro;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.WpfInfrastructure.Extensions;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels
{
	public enum ChangeType
	{
		ClampDetected,
		ClampRemoved,
		ShirtChanged
	}

	public class ShirtListBoxViewModel : Screen
	{
		private readonly ILogger _logger;

		private BindableCollection<Clamp> _clamps;
		private Clamp _currentItem;
		private Shirt _currentShirt;

		private int _currentId;
		private bool _isEnabled;
		private int _rows;
		private int _columns;

		/// <summary>
		/// Initializes a new instance of the <see cref="ShirtListBoxViewModel"/> class.
		/// </summary>
		public ShirtListBoxViewModel()
		{
			_logger = new Log4NetLogger();
			_logger.Init( GetType() );
			_clamps = new BindableCollection<Clamp>();
		}

		public int Rows
		{
			get { return _rows; }
			set
			{
				if (value != _rows)
				{
					_rows = value;
					NotifyOfPropertyChange();
				}
			}
		}

		public int Columns
		{
			get { return _columns; }
			set
			{
				if (value != _columns)
				{
					_columns = value;
					NotifyOfPropertyChange();
				}
			}
		}

		public int SortOrder { get { return 10; } }

		public bool IsEnabled
		{
			get { return _isEnabled; }

			set
			{
				if (value != _isEnabled)
				{
					_isEnabled = value;
					NotifyOfPropertyChange( () => IsEnabled );
				}
			}
		}

		public Shirt CurrentShirt
		{
			get { return _currentShirt; }
			set
			{
				if (value != _currentShirt)
				{
					_currentShirt = value;
					NotifyOfPropertyChange( () => CurrentShirt );

                    if (CurrentShirt != null)
                        CurrentShirt.NotifyAll();
				}
			}
		}

		public BindableCollection<Clamp> Clamps
		{
			get { return _clamps; }
			set
			{
				if (value != _clamps)
				{
					_clamps = value;
					NotifyOfPropertyChange( () => Clamps );
				}
			}
		}

		public Clamp CurrentItem
		{
			get { return _currentItem; }
			set
			{
				if (value != _currentItem)
				{
					_currentItem = value;
					CurrentShirt = _currentItem != null ? _currentItem.CurrentShirt : null;
                    NotifyOfPropertyChange(() => CurrentItem);
				}
			}
		}

		public void CreateSampleData()
		{
			for (var i = 0; i < 20; i++)
			{
				Clamps.Add( GetRandomClamp() );
			}
		}

		/// <summary>
		/// Call this regularly to fill the ShirtListBox with shirts. Will automatically already existing shirts (in case of cyclic buffer).
		/// </summary>
		public void ClampChanged(Clamp clamp, ChangeType changeType)
		{
			var existsAlready = Clamps.Any( c => c.ID == clamp.ID );

			if (changeType == ChangeType.ClampDetected)
			{
				// remove ALL
				if (existsAlready)
				{
					var clamps = Clamps.Where( c => c.ID == clamp.ID );
					foreach (var clampToRemove in clamps)
						ExecuteDeleteSpecificClamp( clampToRemove );
				}

				Clamps.Add( new Clamp
							  {
								  ID = clamp.ID,
								  TimeStamp = clamp.CurrentShirt != null ? clamp.CurrentShirt.LastRfidRead : DateTime.Now,
								  CurrentShirt = clamp.CurrentShirt
							  } );
			}
			else if (changeType == ChangeType.ClampRemoved)
			{
				// remove ALL
				if (existsAlready)
				{
					var clamps = Clamps.Where( c => c.ID == clamp.ID );
					foreach (var clampToRemove in clamps)
						ExecuteDeleteSpecificClamp( clampToRemove );
				}
			}
			else if (changeType == ChangeType.ShirtChanged)
			{
				if (existsAlready)
				{
					var existingClamp = Clamps.First( c => c.ID == clamp.ID );

					existingClamp.CurrentShirt.RfidReadCounter = clamp.CurrentShirt.RfidReadCounter;
					existingClamp.CurrentShirt.LastRfidRead = clamp.CurrentShirt.LastRfidRead;
					existingClamp.CurrentShirt.FlaggedComment = clamp.CurrentShirt.FlaggedComment;
					existingClamp.CurrentShirt.Route = clamp.CurrentShirt.Route;
					existingClamp.CurrentShirt.SelectedRouteIndex = clamp.CurrentShirt.SelectedRouteIndex;
				}
			}
		}

		public void ResetClamps(IList<Clamp> clamps)
		{
			Clamps = new BindableCollection<Clamp>();

			// TODO: Centigrade! The ShirtListBox cannot handle large amount of new clamps. Reproducible in simulation mode.
			//Clamps.AddRange( clamps );

			foreach (var clamp in clamps)
			{
				Clamps.Add( clamp );
			}

			// [HV] Testcodezeilen (Funktionieren alle nicht, da ein "Ressourcen Problem" vorliegt !)
			//var random = new Random( DateTime.Now.Millisecond );
			//for (var i = 0; i < 50; i++)
			//{
			//	clamps.Add( new Clamp { ID = random.Next( int.MaxValue ), TimeStamp = DateTime.Now, CurrentShirt = new Shirt() } );
			//}
			//Clamps.Add( new Clamp { ID = random.Next( int.MaxValue ), TimeStamp = DateTime.Now, CurrentShirt = new Shirt() } );
		}

		private void ExecuteDeleteSpecificClamp(Clamp existingClamp)
		{
			_logger.Debug( string.Format( "Execute delete of specific clamp (clamp='{0}'", existingClamp ) );
			const double deletionDelay = 700; // Must be greater than the animation duration in the DataTemplate !!!

			var deletionTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds( deletionDelay ) };
			deletionTimer.Tick += (o, args) =>
			{
				_logger.Debug( string.Format( "Remove clamp (clamp='{0}'", existingClamp ) );
				Clamps.Remove( existingClamp );
				deletionTimer.Stop();
				EventHelper.RemoveAllEventHandler( deletionTimer, "Tick" );
			};

			deletionTimer.Start();
			existingClamp.IsRemoving = true;
		}

		/// <summary>
		/// Used to fadeout the view before resetting the current item that collapses the view.
		/// </summary>
		public void StartClose()
		{
			var timer = new Timer { Interval = 300, AutoReset = false };
			timer.Elapsed += (sender, evt) =>
			{
				Close();
				timer.Dispose();
				timer = null;
			};
			timer.Start();
		}

		public void Close()
		{
			CurrentItem = null;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
		}

		protected override void OnDeactivate(bool close)
		{
			base.OnDeactivate( close );
		}

		#region REMOVE METHODS FOR DUMMY DATA

		private Clamp GetRandomClamp()
		{
			var c = new Clamp { ID = ++_currentId, TimeStamp = DateTime.Now, CurrentShirt = Shirt.GetRandomShirt(), IsEmpty = GetRandomBool() };
			return c;
		}

		private static readonly Random Random = new Random();

		private bool GetRandomBool()
		{
			bool empty;
			switch (Random.Next( 10 ))
			{
				case 0:
					empty = true;
					break;
				default:
					empty = false;
					break;
			}

			return empty;
		}

		#endregion REMOVE METHODS FOR DUMMY DATA
	}
}
