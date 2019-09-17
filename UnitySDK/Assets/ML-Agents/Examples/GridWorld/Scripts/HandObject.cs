using System;
using System.Collections.Generic;

namespace Blocksworld
{
    public class HandObject : SimpleObject
    {

        public long graspStartTime = 0;
        public long ungraspStartTime = 0;
        public bool reflexGrasp = false;

        // degrees
        public int rotation = 0;

        public bool grasping = false;

        public SimpleObject graspedObject = null;

        public HandObject(BlocksWorldSensoriMotorSystem s, String name, int color, String texture, int x, int y)
            :  base(s, name, color, texture, x, y)
        {
           
        }

        public void grasp()
        {
            grasp(0);
        }

        public void grasp(long clock)
        {
            graspStartTime = clock;
            grasping = true;
        }

        public void ungrasp()
        {
            ungrasp(0);
        }

        public void ungrasp(long clock)
        {
            reflexGrasp = false;
            ungraspStartTime = clock;
            grasping = false;
        }

        public void home()
        {
            getSMS().moveTo(this, 1, 1);
        }

        public void rotateClockwise()
        {
            rotation = ((rotation + 90) + 360) % 360;
        }

        public void rotateCounterClockwise()
        {
            rotation = ((rotation - 90) + 360) % 360;
        }

        public override Dictionary<String, Object> toMap()
        {
            Dictionary<String, Object> obj = base.toMap();
            obj.Add("rotation", rotation);
            obj.Add("grasping", grasping);
            obj.Add("graspedObject", graspedObject == null ? null : graspedObject.name);
            return obj;
        }

        /*    public String toJSONString() {
            return JSONValue.toJSONString(this.toMap());
        }
        */

    }



}
