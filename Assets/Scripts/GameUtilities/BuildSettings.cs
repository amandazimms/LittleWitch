using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSettings : MonoBehaviour
{
	public enum BuildType { PC, iPhone, iPad, iPadMini };
	public BuildType thisBuild = BuildType.PC;

	PlayerMovement playerMovement;
	//CustomCursor cursor;
	CameraControls cameraControls;

	void Awake()
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		playerMovement = player.GetComponent<PlayerMovement>();
		//cursor = FindObjectOfType<CustomCursor>();
		GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
		cameraControls = cam.GetComponent<CameraControls>();

		DoBuildSettings();
	}

	void DoBuildSettings()
	{
		if (thisBuild == BuildType.PC)
		{
			if (playerMovement)
				playerMovement.isTouchBuild = false;
			//if (cursor)
				//cursor.isTouchBuild = false;
			if (cameraControls)
				cameraControls.isTouchBuild = false;
			//if (cameraControls)
				//if (cameraControls.setOffsetMultiplierInBuildSettings)
				//	cameraControls.zoomClampOffsetMultiplier = new Vector2(1.76f, 1.01f);
		}
		else if (thisBuild == BuildType.iPhone || thisBuild == BuildType.iPad || thisBuild == BuildType.iPadMini)
		{
			if (playerMovement)
				playerMovement.isTouchBuild = true;
			//if (cursor)
				//cursor.isTouchBuild = true;
			if (cameraControls)
				cameraControls.isTouchBuild = true;
		}

		if (thisBuild == BuildType.iPhone)
		{
			//if (cameraControls)
				//if (cameraControls.setOffsetMultiplierInBuildSettings)
				//	cameraControls.zoomClampOffsetMultiplier = new Vector2(1.76f, 1.01f);
		}
		if (thisBuild == BuildType.iPad)
		{
			//if (cameraControls)
				//if (cameraControls.setOffsetMultiplierInBuildSettings)
				//	cameraControls.zoomClampOffsetMultiplier = new Vector2(1.5f, 1.4f);
		}
		if (thisBuild == BuildType.iPadMini)
		{
			//if (cameraControls)
				//if (cameraControls.setOffsetMultiplierInBuildSettings)
				//	cameraControls.zoomClampOffsetMultiplier = new Vector2(1.5f, 1.4f);
		}
	}
}
