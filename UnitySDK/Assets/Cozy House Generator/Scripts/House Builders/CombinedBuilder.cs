using System;
using System.Collections.Generic;
using Cozy_House_Generator.Scripts.Core;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
//using UnityEditor;
using Object = UnityEngine.Object;

namespace Cozy_House_Generator.Scripts.House_Builders
{
    public class CombinedBuilder : IHouseBuilder
    {
        private ObjectToCombineData[,]  cellGrid;
        private MeshCombiner 			meshCombiner = new MeshCombiner();
        
        
        public void Build(BuilderData data)
        {
		        var combinedMesh     = CreateHousePartContainer("Combined",      data.root, true);
		        var meshCombineData  = new MeshCombineData();
		        var propsRoot        = CreateHousePartContainer("Props",         data.root);
		        var doorsRoot        = CreateHousePartContainer("Doors",         data.root);
		        var windowFramesRoot = CreateHousePartContainer("Window Frames", data.root);
		        
	        for (int floor = 0; floor < data.blueprint.floorsCount; floor++)
	        {
		        var cellPositions = GetCellPositions(data.blueprint.Size(), data.root.transform,
		                                             data.cellPrefab.GetComponent<HouseCell>().cellSize,
		                                             data.cellPrefab.GetComponent<HouseCell>().cellHeight, floor);
		        
		        for (int x = 0; x < data.blueprint.Size(); x++)
		        {
			        for (int y = 0; y < data.blueprint.Size(); y++)
			        {
				        BuildCell(data.blueprint.GetCell(floor, x, y), data, meshCombineData, cellPositions[x, y],
				                  propsRoot.transform,
				                  doorsRoot.transform, windowFramesRoot.transform, meshCombiner, floor);
			        }
		        }
	        }
	        
	        	Mesh mesh = meshCombiner.Combine(meshCombineData.all, data.root.transform);
	
	        	combinedMesh.GetComponent<MeshFilter>()  .mesh       = mesh;
	        	combinedMesh.GetComponent<MeshCollider>().sharedMesh = mesh; 
	        	combinedMesh.GetComponent<MeshRenderer>().materials  = meshCombineData.all.Materials.ToArray();

        }

		
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Calculates and returns a 2D array with world coordinates of cells in the cell grid  </summary>
        /// 
        /// <param name="size">  The size of the house  			  </param>
        /// <param name="root">  A root (parent) GameObject of house  </param>
        /// 
        /// <returns>  2D array with world coordinates of cells in the cell grid  </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static Vector3[,] GetCellPositions(int size, Transform root, float cellSize, float cellHeight, int floor)
        {
	        var localPositions = new Vector3[size, size];
	        var rootPos        = root.position;

	        for (int x = 0; x < size; x++)
	        {
		        for (int y = 0; y < size; y++)
		        {
			        localPositions[x, y] = new Vector3
			                               {
				                               x = rootPos.x + x 		* cellSize,
				                               y = rootPos.y + floor 	* cellHeight,
				                               z = rootPos.z + y 		* cellSize
			                               };
		        }
	        }

	        return localPositions;
        }
        
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Creates a container for some parts of the house  </summary>
        /// 
        /// <param name="containerName">    The name of the container  					   </param>
        /// <param name="house">  			The house will be a parent of the container    </param>
        /// <param name="haveMesh">  		Is this container must have mesh components?   </param>
        /// 
        /// <returns>  Container for some part of house  </returns>
        ///////////////////////////////////////////////////////////////////////////////////////////
        private static GameObject CreateHousePartContainer(string containerName, Transform house, bool haveMesh = false)
        {
	        var container = haveMesh
		                    ? new GameObject(containerName, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))
		                    : new GameObject(containerName);
		    
	        container.transform.position = house.position;
	        container.transform.rotation = house.rotation;
	        container.transform.parent   = house;
	        return container;
        }
        
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Builds a house cell. Actually, it just gets info from prefabs and merges them.  </summary>
        /// 
        /// <param name="blueprintCell">  	 Info about a house cell.  				   </param>
        /// <param name="data">  			 Necessary data for house building.  	   </param>
        /// <param name="combineData">  	 Here a data for combine will be written.  </param>
        /// <param name="cellPosition">  	 A position of building cell.  			   </param>
        /// <param name="propsRoot">  		 Props container.  						   </param>
        /// <param name="doorsRoot">  		 Doors container.  						   </param>
        /// <param name="windowFramesRoot">  Window frames container. 				   </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void BuildCell(BlueprintCell blueprintCell, BuilderData data, MeshCombineData combineData, 
	        Vector3 cellPosition, Transform propsRoot, Transform doorsRoot, Transform windowFramesRoot, MeshCombiner meshCombiner, int floor)
        {
	        if (data.interiors.Length < 1)
	        {
		        Debug.LogWarning("House Cell: INTERIORS ON HOUSE GENERATOR IS EMPTY");
		        return;
	        }
			BuildColumns	(blueprintCell, cellPosition, data, combineData);
			
	        if (blueprintCell.RoomId < 0) return;
	        var interior = data.interiors[blueprintCell.interiorId];

			BuildCeiling	(cellPosition, data, interior, combineData);
			BuildFloor		(cellPosition, data, interior, combineData);
			BuildWalls		(blueprintCell, cellPosition, data, interior, combineData, doorsRoot, windowFramesRoot, floor);
			InstantiateProps(blueprintCell, cellPosition, data, propsRoot, meshCombiner);
        }
        
        
		/////////////////////////////////////////////////////////////////////////////////////
		/// <summary>  Builds a house cell ceiling.  </summary>
		/// 
		/// <param name="cellPos">  	A position of building cell. 				</param>
		/// <param name="data">  		Necessary data for house building.			</param>
		/// <param name="interior">  	An interior of the current room.  			</param>
		/// <param name="combineData">  Here a data for combine will be written.	</param>
		////////////////////////////////////////////////////////////////////////////////////
        private static void BuildCeiling(Vector3 cellPos, BuilderData data, Interior interior, MeshCombineData combineData)
        {
	        if (data.hideCeiling) return;
	        var ceilingData = GetCombineDataFromGameObject(data.cellPrefab.GetComponent<HouseCell>().ceiling, cellPos);
	        
	        ceilingData.materials[data.cellPrefab.GetComponent<HouseCell>().ceilingMaterialsId] = interior.ceiling;
	        
	        combineData.ceiling.Add(ceilingData);
	        combineData.all    .Add(ceilingData);
        }


		////////////////////////////////////////////////////////////////////////////////////
		/// <summary>  Builds a house cell floor.  </summary>
		/// 
		/// <param name="cellPos">  	A position of building cell.  				</param>
		/// <param name="data"> 		Necessary data for house building.  		</param>
		/// <param name="interior">  	An interior of the current room.			</param>
		/// <param name="combineData">  Here a data for combine will be written. 	</param>
		////////////////////////////////////////////////////////////////////////////////////
        private static void BuildFloor(Vector3 cellPos, BuilderData data, Interior interior, MeshCombineData combineData)
        {
	        var floorData = GetCombineDataFromGameObject(data.cellPrefab.GetComponent<HouseCell>().floor, cellPos);
	        
	        floorData.materials[data.cellPrefab.GetComponent<HouseCell>().floorMaterialId] = interior.floor;
	        
	        combineData.floors.Add(floorData);
	        combineData.all   .Add(floorData);
        }


		////////////////////////////////////////////////////////////////////////////////////////////		
		/// <summary>  Builds a house cell walls.  </summary>
		/// 
		/// <param name="cell">  				Info about a house cell.  					</param>
		/// <param name="cellPos">  			A position of building cell.				</param>
		/// <param name="data">  				Necessary data for house building.  		</param>
		/// <param name="interior">  			An interior of the current room.  			</param>
		/// <param name="combineData">  		Here a data for combine will be written.  	</param>
		/// <param name="doorsRoot">  			Doors container.  							</param>
		/// <param name="windowFramesRoot">  	Window frames container.  					</param>
		////////////////////////////////////////////////////////////////////////////////////////////
        private static void BuildWalls(BlueprintCell   cell,        Vector3   cellPos,   BuilderData data, Interior interior,
                                  MeshCombineData combineData, Transform doorsRoot, Transform   windowFramesRoot, int floor)
        {
	        var rightWall = 	data.cellPrefab.GetComponent<HouseCell>().rightWall;
	        var leftWall = 		data.cellPrefab.GetComponent<HouseCell>().leftWall;
	        var forwardWall = 	data.cellPrefab.GetComponent<HouseCell>().forwardWall;
	        var backwardWall = 	data.cellPrefab.GetComponent<HouseCell>().backwardWall;
	        
	        BuildWall(rightWall, 	cellPos, interior, combineData, cell.RightWall, 	data, doorsRoot, windowFramesRoot, floor);
	        BuildWall(leftWall, 	cellPos, interior, combineData, cell.LeftWall, 		data, doorsRoot, windowFramesRoot, floor);
	        BuildWall(forwardWall,  cellPos, interior, combineData, cell.ForwardWall, 	data, doorsRoot, windowFramesRoot, floor);
	        BuildWall(backwardWall, cellPos, interior, combineData, cell.BackwardWall, 	data, doorsRoot, windowFramesRoot, floor);
		       
        }


		////////////////////////////////////////////////////////////////////////////////////
		/// <summary>  Builds a house cell columns.  </summary>
		/// 
		/// <param name="cell">			Info about a house cell.					</param>  
		/// <param name="cellPos">		A position of building cell.				</param>  
		/// <param name="data">			Necessary data for house building.			</param>  
		/// <param name="combineData">	Here a data for combine will be written.	</param>
		//////////////////////////////////////////////////////////////////////////////////// 
        private static void BuildColumns(BlueprintCell cell, Vector3 cellPos, BuilderData data, MeshCombineData combineData)
		{
			var houseCell = data.cellPrefab.GetComponent<HouseCell>();
			int currentFloor = cell.floor;
			int floorsCount = cell.blueprint.floorsCount;
			
	        TryBuildColumn(currentFloor, floorsCount, cell.columnsData.forwardRight,      houseCell.forwardRightColumnInside,
	                       houseCell.forwardRightColumnFacade, cellPos, combineData, data);
	        
	        TryBuildColumn(currentFloor, floorsCount, cell.columnsData.forwardLeft,      houseCell.forwardLeftColumnInside,
	                       houseCell.forwardLeftColumnFacade, cellPos, combineData, data);
	        
	        TryBuildColumn(currentFloor, floorsCount, cell.columnsData.backwardRight,      houseCell.backwardRightColumnInside,
	                       houseCell.backwardRightColumnFacade, cellPos, combineData, data);
	        
	        TryBuildColumn(currentFloor, floorsCount, cell.columnsData.backwardLeft,      houseCell.backwardLeftColumnInside,
	                       houseCell.backwardLeftColumnFacade, cellPos, combineData, data);
        }

		private static void TryBuildColumn(int         currentFloor, int floorsCount, ColumnInfo columnInfo,
		                                   GameObject  inside,
		                                   GameObject  facade, Vector3 cellPos, MeshCombineData combineData,
		                                   BuilderData builderData)
		{
			ObjectToCombineData column;
			
			switch (columnInfo.type)
			{
				case ColumnType.Inside:
					column = GetCombineDataFromGameObject(inside, cellPos);
					column.materials[0] = columnInfo.material;
					combineData.all.Add(column);
					combineData.all.AddRange(GetDecoratorsCombineData(currentFloor, floorsCount, inside, cellPos, builderData));
					break;
				
				case ColumnType.Facade:
					column = GetCombineDataFromGameObject(facade, cellPos);
					column.materials[0] = columnInfo.material;
					combineData.all.Add(column);
					combineData.all.AddRange(GetDecoratorsCombineData(currentFloor, floorsCount, facade, cellPos, builderData));
					break;
			}
		}

 
		/////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>  Returns info from house cell prefab part. </summary>
		/// 
		/// <param name="tar">  			Target to analyze (part of cell prefab).  	</param>
		/// <param name="cellPosition">  	A position of building cell.     			</param>
		/// 
		/// <returns>  Necessary info for part of house cell combining.  </returns>
		////////////////////////////////////////////////////////////////////////////////////////
        private static ObjectToCombineData GetCombineDataFromGameObject(GameObject tar, Vector3 cellPosition)
        {
	        var data = new ObjectToCombineData
	                   {
		                   mesh = tar.GetComponent<MeshFilter>().sharedMesh,
		                   worldPosition = Matrix4x4.TRS(cellPosition + tar.transform.position,
		                                                 tar.transform.rotation,
		                                                 tar.transform.localScale),
		                   materials = tar.GetComponent<MeshRenderer>().sharedMaterials
	                   };
	        return data;
        }
        
		
        ////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>  Builds a house cell wall.  </summary>
		/// 
		/// <param name="wall">  				A wall inside of cell prefab  				</param>
		/// <param name="cellPos">  			A position of building cell. 				</param>
		/// <param name="interior">  			An interior of the current room.  			</param>
		/// <param name="combineData">  		Here a data for combine will be written.  	</param>
		/// <param name="wallInfo">  			Info about a wall from a blueprint.			</param>
		/// <param name="data">  				Necessary data for house building.  		</param>
		/// <param name="doorsRoot"> 			Doors container. 							</param>            		
		/// <param name="windowFramesRoot"> 	Window frames container. 					</param>
		////////////////////////////////////////////////////////////////////////////////////////////
        private static void BuildWall (Wall wall, Vector3 cellPos, Interior interior, MeshCombineData combineData, 
                                 WallInfo wallInfo, BuilderData data, Transform doorsRoot, Transform windowFramesRoot, int floor)
        {
	        ObjectToCombineData wallData;

	        if (wallInfo.ownCell == null)
		        return;
	        
	        int floorsCount = wallInfo.ownCell.blueprint.floorsCount;
	        
			switch (wallInfo.wallType)
			{
				case WallType.Void:
					break;
				
				case WallType.ExternalWall:
					
					if (wall.externalWall == null)
						return;
					
					wallData = GetCombineDataFromGameObject(wall.externalWall, cellPos);
					wallData.materials[wall.extFacadeMaterialId] = data.facadeMaterial;
					wallData.materials[wall.extInsideMaterialId] = interior.walls;
					combineData.walls.Add(wallData);
					combineData.all  .Add(wallData);
					combineData.all  .AddRange(GetDecoratorsCombineData(floor, floorsCount, wall.externalWall, cellPos, data));
					break;
			
				case WallType.InternalWall:
					
					if (wall.internalWall == null)
						return;
					
					wallData = GetCombineDataFromGameObject(wall.internalWall, cellPos);
					wallData.materials[wall.internalWallMatId] = interior.walls;
					combineData.walls.Add(wallData);
					combineData.all  .Add(wallData);
					combineData.all  .AddRange(GetDecoratorsCombineData(floor, floorsCount, wall.internalWall, cellPos, data));
					break;
			
				case WallType.InternalDoor:
					
					if (wall.internalDoorway == null)
						return;
					
					wallData = GetCombineDataFromGameObject(wall.internalDoorway, cellPos);
					wallData.materials[wall.internalDoorMatId] = interior.walls;
					combineData.walls.Add(wallData);
					combineData.all  .Add(wallData);

					if (wallInfo.placeInternalDoor)
					Object.Instantiate(data.internalDoorPrefab, wall.internalDoor.transform.position + cellPos,
					                   wall.internalDoor.transform.rotation, doorsRoot);
					combineData.all.AddRange(GetDecoratorsCombineData(floor, floorsCount, wall.internalDoorway, cellPos, data));
					break;
			
				case WallType.ExternalDoor:

					if (wall.externalDoorway == null)
						return;
					
					wallData = GetCombineDataFromGameObject(wall.externalDoorway, cellPos);
					wallData.materials[wall.extDoorFacadeMaterialId] = data.facadeMaterial;
					wallData.materials[wall.extDoorInsideMaterialId] = interior.walls;
					combineData.walls.Add(wallData);
					combineData.all  .Add(wallData);
					
					Object.Instantiate(data.externalDoorPrefab, wall.externalDoor.transform.position + cellPos,
					                   wall.externalDoor.transform.rotation, doorsRoot);
					combineData.all.AddRange(GetDecoratorsCombineData(floor, floorsCount, wall.externalDoorway, cellPos, data));
					break;
					
				case WallType.Window:

					if (wall.window == null)
						return;
					
					wallData = GetCombineDataFromGameObject(wall.window, cellPos);
					wallData.materials[wall.windowFacadeMaterialId] = data.facadeMaterial;
					wallData.materials[wall.windowInsideMaterialId] = interior.walls;
					combineData.walls.Add(wallData);
					combineData.all  .Add(wallData);
					
					Object.Instantiate(data.windowFramePrefab, wall.windowFrame.transform.position + cellPos,
					                   wall.windowFrame.transform.rotation, windowFramesRoot);
					combineData.all.AddRange(GetDecoratorsCombineData(floor, floorsCount, wall.window, cellPos, data));
					break;
			}
		}
		

		private static IEnumerable<ObjectToCombineData> GetDecoratorsCombineData(
			int currentFloor, int floorsCount, GameObject decoratorRoot, Vector3 cellPosition, BuilderData data)
		{
			var decorators = decoratorRoot.GetComponentsInChildren<Decorator>(true);
			if (decorators == null || decorators.Length == 0)
				return null;
			
			var combineDataArray = new List<ObjectToCombineData>();

			foreach (var decorator in decorators)
			{
				if (decorator.IsCanBeBuilded(data.rnd, currentFloor, floorsCount) == false)
					continue;
				
				var combineData = GetCombineDataFromGameObject(decorator.gameObject, cellPosition);
				switch (decorator.materialSource)
				{
					case DecoratorMaterialType.Sample1:
						combineData.materials[0] = data.decoratorSample1; 
						break;
					case DecoratorMaterialType.Sample2:
						combineData.materials[0] = data.decoratorSample2;
						break;
					
					case DecoratorMaterialType.Sample3:
						combineData.materials[0] = data.decoratorSample3;
						break;
				}
				combineDataArray.Add(combineData);
			}

			return combineDataArray;
		}


		////////////////////////////////////////////////////////////////////////
		/// <summary>  Instantiates props.  </summary>
		/// 
		/// <param name="cell">  	Info about a house cell.  			</param>
		/// <param name="cellPos">  A position of building cell.  		</param>
		/// <param name="data">  	Necessary data for house building.  </param>
		/// <param name="root">  	Props container						</param>
		////////////////////////////////////////////////////////////////////////
        private static void InstantiateProps(BlueprintCell cell, Vector3 cellPos, BuilderData data, Transform root, MeshCombiner meshCombiner)
        {
	        if (cell.propsToInstantiate == null || cell.propsToInstantiate.prefab == null) return;

	        var propsContainer = new GameObject("Props Container", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
	        propsContainer.transform.parent = root;
	        propsContainer.transform.position =
		        cellPos + data.cellPrefab.GetComponent<HouseCell>().propsContainer.transform.localPosition;

	        var newProps = Object.Instantiate(cell.propsToInstantiate.prefab, propsContainer.transform);
	        
	        newProps.transform.localPosition = cell.lookAtForwardLocalPos;
	        newProps.transform.localRotation = cell.lookAtForwardLocalRot;

	        switch (cell.propsDirection)
	        {
		        case SimpleDir.Forward:
			        propsContainer.transform.localRotation *= Quaternion.Euler(0, 180, 0);
			        break;

		        case SimpleDir.Right:
			        propsContainer.transform.localRotation *= Quaternion.Euler(0, -90, 0);
			        break;

		        case SimpleDir.Backward:
			        propsContainer.transform.localRotation *= Quaternion.Euler(0, 0, 0);
			        break;

		        case SimpleDir.Left:
			        propsContainer.transform.localRotation *= Quaternion.Euler(0, 90, 0);
			        break;
	        }

	        var propsRandomCastomizer = newProps.GetComponent<PropsRandomizer>();

	        if (propsRandomCastomizer != null)
		        propsRandomCastomizer.Castomize(data.rnd);

	        if (cell.propsToInstantiate.combineAfterInstantiate)
	        {
		        var lodsCombiner = newProps.GetComponent<LodsCombiner>();
		        if (lodsCombiner)
		        {
			        lodsCombiner.Combine(data.lodCullingPercent, cell.propsToInstantiate.shadowCastingMode);
		        }
		        else
		        {
					meshCombiner.Combine(propsContainer);
					propsContainer.GetComponent<MeshRenderer>().shadowCastingMode = cell.propsToInstantiate.shadowCastingMode;
		        }
	        }
        }

    }
    
    
    
}