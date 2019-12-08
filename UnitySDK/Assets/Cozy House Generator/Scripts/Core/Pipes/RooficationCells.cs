using System.Collections.Generic;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{
    public class RooficationCells : IGeneratorPipe
    {
        private int                useCount;
        private UseOnFloorResolver floors;
        
        
        public void Run(Blueprint blueprint, int floor, Interior[] interiors, Material facadeMaterial,
                        Material  facadeColumnMaterial,
                        Random rnd)
        {
            if (floor < 1)
                return;

            var borders = FindBorders(blueprint, floor);
            if (borders.Length > 0)
            {
                foreach (var border in borders)
                {
                    border.RoomId = -1;
                }
            }
        }

        
        public string GetPipeName()
        {
            return "Roofication Cells";
        }
        
        
        public int UseCount()
        {
            return useCount;
        }

       
        public UseOnFloorResolver FloorsRangeData()
        {
            return floors;
        }
        
        
        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new RooficationCells {useCount = count, floors = floors};
        }

        
        private BlueprintCell[] FindBorders(Blueprint blueprint, int floor)
        {
            List<BlueprintCell> borders = new List<BlueprintCell>();
            for (int x = 0; x < blueprint.size; x++)
            {
                for (int y = 0; y < blueprint.size; y++)
                {
                    var cell = blueprint.GetCell(floor, x, y);

                    if (cell.neighbours.Forward(floor)                 == null  || cell.neighbours.Backward(floor)              == null  || 
                        cell.neighbours.Right(floor)                   == null  || cell.neighbours.Left(floor)                  == null  ||
                        cell.neighbours.Forward(floor)       .IsRoom() == false || cell.neighbours.Backward(floor)    .IsRoom() == false || 
                        cell.neighbours.Right(floor)         .IsRoom() == false || cell.neighbours.Left(floor)        .IsRoom() == false ||
                        cell.neighbours.ForwardRight(floor)            == null  || cell.neighbours.ForwardLeft(floor)           == null  ||
                        cell.neighbours.BackwardRight(floor)           == null  || cell.neighbours.BackwardLeft(floor)          == null  ||
                        cell.neighbours.ForwardRight(floor)  .IsRoom() == false || cell.neighbours.ForwardLeft(floor) .IsRoom() == false ||
                        cell.neighbours.BackwardRight(floor) .IsRoom() == false || cell.neighbours.BackwardLeft(floor).IsRoom() == false
                        )
                        borders.Add(cell);
                }
            }

            return borders.ToArray();
        }
    }
}