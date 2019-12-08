using System;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;

namespace Cozy_House_Generator.Scripts.Core
{   
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  A part of the house grid in the blueprint that contains walls, floor, some props, etc...  </summary>
   /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class BlueprintCell
    {
        public              int                          RoomId        { get; set; }
        public              WallInfo                     LeftWall      { get; private set; }
        public              WallInfo                     RightWall     { get; private set; }
        public              WallInfo                     ForwardWall   { get; private set; }
        public              WallInfo                     BackwardWall  { get; private set; }
                         
        public readonly     int                          x;
        public readonly     int                          y;
        public readonly     int                          floor;
        public              int                          interiorId;
        public              Props                        propsToInstantiate;
        public              bool                         isPropsZone;
        public              PropsType                    typeOfPropsOnPropsZone;
        public              Vector3                      lookAtForwardLocalPos;
        public              Quaternion                   lookAtForwardLocalRot;
        public              SimpleDir                    propsDirection;
        public              int                          windowsRate;
        public              int                          tempId;  //Can be used in pipe
                 
        public              ColumnsData                  columnsData = new ColumnsData();
        public readonly     Blueprint                    blueprint;
        public              BlueprintCellNeighbours      neighbours;
        

        public BlueprintCell (Blueprint blueprint, int floor, int x, int y, int roomId = -1)
        {
            RoomId             = roomId;
            this.blueprint     = blueprint;
            this.floor         = floor; 
            this.x             = x;
            this.y             = y;
            LeftWall           = new WallInfo(SimpleDir.Left);
            RightWall          = new WallInfo(SimpleDir.Right);
            ForwardWall        = new WallInfo(SimpleDir.Forward);
            BackwardWall       = new WallInfo(SimpleDir.Backward);
            neighbours         = new BlueprintCellNeighbours(this);
        }

        
        public BlueprintCell(BlueprintCell copySource)
        {
            RoomId                     =     copySource.RoomId;
            LeftWall                   = new WallInfo(copySource.LeftWall);    
            RightWall                  = new WallInfo(copySource.RightWall);
            ForwardWall                = new WallInfo(copySource.ForwardWall);
            BackwardWall               = new WallInfo(copySource.BackwardWall);
            x                          =     copySource.x;
            y                          =     copySource.y;
            floor                      =     copySource.floor;
            interiorId                 =     copySource.interiorId;
            propsToInstantiate         =     copySource.propsToInstantiate;
            isPropsZone                =     copySource.isPropsZone;
            typeOfPropsOnPropsZone     =     copySource.typeOfPropsOnPropsZone;
            lookAtForwardLocalPos      =     copySource.lookAtForwardLocalPos;
            lookAtForwardLocalRot      =     copySource.lookAtForwardLocalRot;
            propsDirection             =     copySource.propsDirection;
            windowsRate                =     copySource.windowsRate;
            tempId                     =     copySource.tempId;
            columnsData                = new ColumnsData(copySource.columnsData);
            blueprint                  =     copySource.blueprint;
            neighbours                 = new BlueprintCellNeighbours(copySource.neighbours);
        }

        public bool IsRoom()
        {
            return RoomId >= 0;
        }
    }
}







