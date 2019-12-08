using System;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;
using Cozy_House_Generator.Scripts.House_Builders;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;

namespace Cozy_House_Generator.Scripts
{
	///////////////////////////////////////////
	/// <summary>  A house wall  </summary>
	//////////////////////////////////////////
	[Serializable]
	public class Wall : MonoBehaviour
	{
		public WallInfo   	wallInfo;
		public GameObject 	internalWall;
		public GameObject 	externalWall;
		public GameObject 	internalDoorway;
		public GameObject 	internalDoor;
		public GameObject 	externalDoorway;
		public GameObject 	externalDoor;
		public GameObject 	window;
		public GameObject 	windowFrame;
		
		public int 			extFacadeMaterialId 		= 1;
		public int 			extInsideMaterialId 		= 0;
		public int 			windowFacadeMaterialId 		= 1;
		public int 			windowInsideMaterialId 		= 0;
		public int 			extDoorFacadeMaterialId 	= 0;
		public int 			extDoorInsideMaterialId 	= 1;
		public int 			internalWallMatId 			= 1;
		public int 			internalDoorMatId 			= 1;



		/////////////////////////////////////////////////////////////////////////////////////
		/// <summary>  Instantiates a wall  </summary>
		/// 
		/// <param name="wallInfo">  		 Info for instantiating wall 		</param>
		///////////////////////////////////////////////////////////////////////////////////
		public void Build (WallInfo wallInfo, BuilderData buildData, Interior interior, int floor)
		{
			this.wallInfo = wallInfo;
			Renderer rend;
			Material[] mats;

			switch (wallInfo.wallType)
			{
				case WallType.Void:
					break;
				
				case WallType.ExternalWall:
					
					if (externalWall == null)
						break;
					
					rend 						= externalWall.GetComponent<Renderer>();
					mats 						= rend.sharedMaterials;
					mats[extFacadeMaterialId] 	= buildData.facadeMaterial;
					mats[extInsideMaterialId] 	= interior.walls;
					rend.sharedMaterials 		= mats;
					externalWall.SetActive (true);
					BuildDecor(externalWall, buildData, wallInfo, floor);
					break;
			
				case WallType.InternalWall:
					
					if (internalWall == null)
						break;
					
					rend 					= internalWall.GetComponent<Renderer>();
					mats 					= rend.sharedMaterials;
					mats[internalWallMatId] = interior.walls;
					rend.sharedMaterials 	= mats;
					internalWall.SetActive (true);
					BuildDecor(internalWall, buildData, wallInfo, floor);
					break;
			
				case WallType.InternalDoor:
					
					if (internalDoor == null)
						break;
					
					rend 					= internalDoorway.GetComponent<Renderer>();
					mats 					= rend.sharedMaterials;
					mats[internalDoorMatId] = interior.walls;
					rend.sharedMaterials 	= mats;
					internalDoorway.SetActive (true);
					if (this.wallInfo.placeInternalDoor && internalDoor != null)
						internalDoor.SetActive(true);
					BuildDecor(internalDoorway, buildData, wallInfo, floor);
					break;
			
				case WallType.ExternalDoor:

					if (externalDoorway == null)
						break;
					
					rend 						  = externalDoorway.GetComponent<Renderer>();
					mats 						  = rend.sharedMaterials;
					mats[extDoorFacadeMaterialId] = buildData.facadeMaterial;
					mats[extDoorInsideMaterialId] = interior.walls;
					rend.sharedMaterials 		  = mats;
					externalDoorway.SetActive (true);
					externalDoor.SetActive(true);
					BuildDecor(externalDoorway, buildData, wallInfo, floor);
					break;
			
				case WallType.Window:

					if (window == null)
						break;
					
					rend 							= window.GetComponent<Renderer>();
					mats 							= rend.sharedMaterials;
					mats[windowFacadeMaterialId] 	= buildData.facadeMaterial;
					mats[windowInsideMaterialId] 	= interior.walls;
					rend.sharedMaterials 			= mats;
					window.SetActive (true);
					windowFrame.SetActive(true);
					BuildDecor(window, buildData, wallInfo, floor);
					break;
			}
		}

		
		private void BuildDecor(GameObject decoratorRoot, BuilderData buildData, WallInfo wallInfo, int floor)
		{
			var decorators = decoratorRoot.GetComponentsInChildren<Decorator>(true);
			if (decorators == null || decorators.Length == 0)
				return;

			foreach (var decorator in decorators)
			{
				if (decorator.IsCanBeBuilded(buildData.rnd, floor, wallInfo.ownCell.blueprint.floorsCount))
					decorator.Build(buildData);
			}
		}

		
		////////////////////////////////////////////////////////
		/// <summary>  Use it before build a wall  </summary>
		/////////////////////////////////////////////////////// 
		public void ResetBeforeUsing()
		{
			if (internalWall 	!= null)  internalWall.	  	SetActive(false);
			if (externalWall 	!= null)  externalWall.	  	SetActive(false);
			if (internalDoorway != null)  internalDoorway.  SetActive(false);
			if (internalDoor 	!= null)  internalDoor.	  	SetActive(false);
			if (externalDoorway != null)  externalDoorway.  SetActive(false);
			if (externalDoor 	!= null)  externalDoor.	  	SetActive(false);
			if (window 			!= null)  window.			SetActive(false);
			if (windowFrame 	!= null)  windowFrame.	  	SetActive(false);

			for (int i = 0; i < transform.childCount; i++)
			{
				var decorators = transform.GetChild(i).GetComponentsInChildren<Decorator>();
				if (decorators == null || decorators.Length == 0)
					continue;

				foreach (var decorator in decorators)
				{
					decorator.gameObject.SetActive(false);
				}
			}
		}
	}
}
