using System;
using System.Linq;
using Cozy_House_Generator.Scripts.Core;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;
using Cozy_House_Generator.Scripts.House_Builders;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cozy_House_Generator.Scripts
{
	//////////////////////////////////////////////////////////////////////////////////////
	/// <summary>  A game representation of BlueprintCell in unity 3D space  </summary>
	/////////////////////////////////////////////////////////////////////////////////////
	[Serializable]
	public class HouseCell : MonoBehaviour
	{
		public float 		 		cellSize;
		public float 		 		cellHeight;
		public GameObject 	 		floor;
		public GameObject 	 		ceiling;
		public GameObject 	 		propsContainer;
		public Wall       	 		rightWall;
		public Wall       	 		leftWall;
		public Wall       	 		forwardWall;
		public Wall       	 		backwardWall;
		public int		  	 		roomId;
		public int 		  	 		interiorId;
		public Interior 	 		interior; 
		public int 		  	 		x;
		public int 		  	 		y;
		public Props 		 		props;
		public GameObject 	 		forwardRightColumnInside;
		public GameObject 	 		forwardLeftColumnInside;
		public GameObject 	 		backwardRightColumnInside;
		public GameObject 	 		backwardLeftColumnInside;
		
		public GameObject 			forwardRightColumnFacade;
		public GameObject 			forwardLeftColumnFacade;
		public GameObject 			backwardRightColumnFacade;
		public GameObject 			backwardLeftColumnFacade;
		
		public int 			 		floorMaterialId = 5;
		public int 			 		ceilingMaterialsId;
		public bool 		 		isIntersectedByProps;
		public PropsType 	 		intersectProps;
		
		/////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>  Builds walls, floor, ceiling, and props on the cell  </summary>
		/// 
		/// <param name="blueprintCell">  	Info about a house cell.  			</param>
		/// <param name="data">  			Necessary data for house building.  </param>
		/////////////////////////////////////////////////////////////////////////////////////////////////
		public void Build(BlueprintCell blueprintCell, BuilderData data, int floorId)
		{
			ResetBeforeUsing();
			
			// Writing data for debugging from an inspector ui
			roomId 				 = blueprintCell.RoomId;
			interiorId 			 = blueprintCell.interiorId;
			x 		 			 = blueprintCell.x;
			y 		 			 = blueprintCell.y;
			props 				 = blueprintCell.propsToInstantiate;
			isIntersectedByProps = blueprintCell.isPropsZone;
			intersectProps 		 = blueprintCell.typeOfPropsOnPropsZone;
		
			
			
			TryPlaceColumn(blueprintCell.columnsData.forwardRight,  forwardRightColumnInside,  forwardRightColumnFacade);
			TryPlaceColumn(blueprintCell.columnsData.forwardLeft,   forwardLeftColumnInside,   forwardLeftColumnFacade);
			TryPlaceColumn(blueprintCell.columnsData.backwardRight, backwardRightColumnInside, backwardRightColumnFacade);
			TryPlaceColumn(blueprintCell.columnsData.backwardLeft,  backwardLeftColumnInside,  backwardLeftColumnFacade);

			
			
			
			
			if (roomId < 0) return;
			if (data.interiors.Length < 1)
			{
				Debug.LogWarning("House Cell: INTERIORS ON HOUSE GENERATOR IS EMPTY");
				return;
			}

			interior = data.interiors[interiorId];

			
			SetMaterial(floor.GetComponent<MeshRenderer>(), interior.floor, floorMaterialId);
			
			floor.gameObject. SetActive (true);
			

			if (data.hideCeiling == false)
			{
				SetMaterial(ceiling.GetComponent<MeshRenderer>(), interior.ceiling, ceilingMaterialsId);
				ceiling.gameObject. SetActive (true);
			}
		
			rightWall.		Build (blueprintCell.RightWall, 	data, interior, floorId);
			leftWall.		Build (blueprintCell.LeftWall, 		data, interior, floorId);
			forwardWall.	Build (blueprintCell.ForwardWall, 	data, interior, floorId);
			backwardWall.	Build (blueprintCell.BackwardWall, 	data, interior, floorId);

			if (props == null || props.prefab == null) return;

			GameObject newProps;
			
#if UNITY_EDITOR
	        newProps = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(props.prefab);
	        newProps.transform.parent = propsContainer.transform;
#else
			newProps 					    	= Instantiate(props.prefab, propsContainer.transform);
#endif
			newProps.transform.position 		= transform.position;
			newProps.transform.localPosition 	= blueprintCell.lookAtForwardLocalPos;
			newProps.transform.localRotation 	= blueprintCell.lookAtForwardLocalRot;

			switch (blueprintCell.propsDirection)
			{
				case SimpleDir.Forward:
					propsContainer.transform.localRotation = Quaternion.Euler(0, 180, 0);
					break;
				
				case SimpleDir.Right:
					propsContainer.transform.localRotation = Quaternion.Euler(0, -90, 0);
					break;
				
				case SimpleDir.Backward:
					propsContainer.transform.localRotation = Quaternion.Euler(0, 0, 0);
					break;
				
				case SimpleDir.Left:
					propsContainer.transform.localRotation = Quaternion.Euler(0, 90, 0);
					break;
			}

			var propsRandomCastomizer = newProps.GetComponent<PropsRandomizer>();
			
			if (propsRandomCastomizer != null)
				propsRandomCastomizer.Castomize(data.rnd);
		}


		////////////////////////////////////////////////////////
		/// <summary>  Use it before using a cell  </summary>
		///////////////////////////////////////////////////////
		private void ResetBeforeUsing()
		{
			floor.						SetActive(false);
			ceiling.					SetActive(false);
			forwardRightColumnInside.	SetActive(false);
			forwardRightColumnFacade.	SetActive(false);
			forwardLeftColumnInside.	SetActive(false);
			forwardLeftColumnFacade.	SetActive(false);
			backwardRightColumnInside.	SetActive(false);
			backwardRightColumnFacade.	SetActive(false);
			backwardLeftColumnInside.	SetActive(false);
			backwardLeftColumnFacade.	SetActive(false);
			rightWall.					ResetBeforeUsing();
			leftWall.					ResetBeforeUsing();
			forwardWall.				ResetBeforeUsing();
			backwardWall.				ResetBeforeUsing();
		}


		private static void TryPlaceColumn(ColumnInfo columnInfo, GameObject inside, GameObject facade)
		{
			switch (columnInfo.type)
			{
				case ColumnType.Inside:
					inside.SetActive(true);
					SetMaterial(inside.GetComponent<Renderer>(), columnInfo.material, 0);
					break;
				
				case ColumnType.Facade:
					facade.SetActive(true);
					SetMaterial(facade.GetComponent<Renderer>(), columnInfo.material, 0);
					break;
			}			
		}
		
		
		/////////////////////////////////////////////////////////////////////
		/// <summary>  Set material on surface  </summary>
		/// 
		/// <param name="rend">    Renderer component of surface  </param>
		/// <param name="newMat">  Replacement material  		  </param>
		/// <param name="matId">   ID of material for replacing   </param>
		////////////////////////////////////////////////////////////////////
		private static void SetMaterial(Renderer rend, Material newMat, int matId)
		{
			if (Application.isEditor)
			{
				var mats = rend.sharedMaterials;
				mats[matId] = newMat;
				rend.sharedMaterials = mats;
			}
			else
			{
				var mats = rend.materials;
				mats[matId] = newMat;
				rend.materials = mats;
			}
		}
		
		

		
		
#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			if (IsCellsSelected(4) == false)
				return;

			var tar = propsContainer == null ? transform : propsContainer.transform;
			DrawCellBorders(tar);
			
			if (UnityEditor.Selection.gameObjects.Length > 1 || UnityEditor.Selection.activeGameObject != gameObject)
				return;
			
			DrawLabels(tar, cellSize, cellHeight);
		}
		
		
		private void DrawCellBorders(Transform tar)
		{
			var offset = new Vector3(0, cellHeight /2, 0);
			
			Gizmos.color  = Color.cyan;
			Gizmos.matrix = tar.localToWorldMatrix;
			
			Gizmos.DrawSphere(offset, 0.05f);
			Gizmos.DrawWireCube(offset + Vector3.forward * cellSize /2f, new Vector3(cellSize, cellHeight, 0));
			Gizmos.DrawWireCube(offset + Vector3.back    * cellSize /2f, new Vector3(cellSize, cellHeight, 0));
			Gizmos.DrawWireCube(offset + Vector3.left    * cellSize /2f, new Vector3(0,        cellHeight, cellSize));
			Gizmos.DrawWireCube(offset + Vector3.right   * cellSize /2f, new Vector3(0,        cellHeight, cellSize));
			
			DrawArrow.ForGizmo(offset, Vector3.back * cellSize / 3);
		}

		
		private void DrawLabels(Transform tar, float cellSize, float cellHeight)
		{
			var pos      = tar.position;
			var right    = tar.right   * cellSize;
			var left     = right       * -1;
			var forward  = tar.forward * cellSize;
			var backward = forward     * -1;
			var height   = cellHeight  / 2f;
			
			
			UnityEditor.Handles.Label(pos + forward  / 2 + Vector3.up * height, "forward", "BoldLabel");
			UnityEditor.Handles.Label(pos + backward / 2 + Vector3.up * height, "back",    "BoldLabel");
			UnityEditor.Handles.Label(pos + left     / 2 + Vector3.up * height, "left",    "BoldLabel");
			UnityEditor.Handles.Label(pos + right    / 2 + Vector3.up * height, "right",   "BoldLabel");

		}
		
		
		private bool IsCellsSelected(int parentCount)
		{
			if (UnityEditor.Selection.gameObjects == null)
				return false;

			return UnityEditor.Selection.gameObjects.Any(obj => IsCellSelected(obj.transform, parentCount));
		}
		
		
		private bool IsCellSelected(Transform selectedCell, int parentCount)
		{
			if (selectedCell == null)
				return false;
			
			var currentParent = selectedCell;
			while (parentCount > 0)
			{
				parentCount--;
				if (currentParent == null)
					return false;

				if (currentParent == transform)
					return true;

				currentParent = currentParent.parent;
			}
			return false;
		}
#endif
	}
}

