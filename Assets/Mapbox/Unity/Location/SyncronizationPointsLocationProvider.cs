namespace Mapbox.Unity.Location
{
	using UnityEngine;
	using System.Collections.Generic;
	using IndoorMappingDemo;

	public class SyncronizationPointsLocationProvider : AbstractLocationProvider
	{

		private object _syncLock = new object();
		Dictionary<int, IFixedLocation> _syncronizationPoints = new Dictionary<int, IFixedLocation>();
		bool _isUISetupComplete = false;

		private void Awake()
		{
			if (_syncronizationPoints != null)
			{
				_syncronizationPoints = new Dictionary<int, IFixedLocation>();
			}
		}

		protected int Count
		{
			get
			{
				lock (_syncLock)
				{
					return _syncronizationPoints.Count;
				}
			}
		}

		protected void Enqueue(IFixedLocation locationProvider)
		{
			lock (_syncLock)
			{
				if (!_syncronizationPoints.ContainsKey(locationProvider.LocationId))
				{
					Debug.Log("Registering id : " + locationProvider.LocationId);
					_syncronizationPoints.Add(locationProvider.LocationId, locationProvider);
				}
			}
		}

		public void Register(IFixedLocation locationProvider)
		{
			Enqueue(locationProvider);            
            

		}

		private void Update()
		{
			//HACK : To add buttons in increasing order. 

			if (Count < 8 || _isUISetupComplete == true)
			{
				return;
			}
			else
			{
				for (int i = 0; i < Count; i++)
				{
					ApplicationUIManager.Instance.AddToSyncPointUI(i, _syncronizationPoints[i].LocationName, OnSyncRequested);
				}
				_isUISetupComplete = true;
			}
		}

		public void OnSyncRequested(int id)
		{
			Debug.Log("setting location to initialization point");
            var location = new FixedLocationData();
            location.SetLocation(0, "initial position", "initialization position", new Utils.Vector2d(41.399384, 2.192985), 42.84f);
            SendLocation(location.CurrentLocation);
            LocationProviderFactory.Instance.DefaultLocationProvider = this;
			//ApplicationUIManager.Instance.OnStateChanged(ApplicationState.SyncPoint_Calibration);
		}
	}
}