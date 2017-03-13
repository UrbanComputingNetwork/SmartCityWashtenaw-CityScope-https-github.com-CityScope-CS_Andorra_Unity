using System.Collections;
using UnityEngine;
using Tango;

public class TangoAreaLearning : MonoBehaviour, ITangoLifecycle
{
	private TangoApplication m_tangoApplication;
	public TangoARPoseController m_tangoPoseController;

	public void Start()
	{
		// m_tangoPoseController = FindObjectOfType<TangoARPoseController>();
		m_tangoApplication = FindObjectOfType<TangoApplication>();
		if (m_tangoApplication != null)
		{
			m_tangoApplication.Register(this);
			m_tangoApplication.RequestPermissions();
		}
	}

	public void OnTangoPermissions(bool permissionsGranted)
	{
		if (permissionsGranted)
		{
			AreaDescription[] list = AreaDescription.GetList();
			if (list != null && list.Length > 0)
			{
				// Find and load the most recent Area Description
				foreach (AreaDescription areaDescription in list)
				{
					AreaDescription.Metadata metadata = areaDescription.GetMetadata();
					if (metadata.m_name.ToLower().Contains("active"))
					{
						m_tangoApplication.Startup(areaDescription);
						Debug.LogFormat("Loading \"{0}\" area.", metadata.m_name);
						return;
					}
				}

			}
			// No Area Descriptions available.
			Debug.Log("No area descriptions is named  ACTIVE, so I'll load most recent area");
			m_tangoApplication.m_autoConnectToService = true;
			m_tangoApplication.m_3drUseAreaDescriptionPose = false;
			m_tangoApplication.m_enableAreaDescriptions = false;
			m_tangoPoseController.m_useAreaDescriptionPose = false;
			m_tangoApplication.Startup(null);
		}
	}

	public void OnTangoServiceConnected()
	{
	}

	public void OnTangoServiceDisconnected()
	{
	}
}