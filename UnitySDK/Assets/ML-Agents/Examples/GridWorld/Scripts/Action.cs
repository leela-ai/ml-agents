using System;

namespace Blocksworld
{
    public class Action
    {
        public enum Type
        {
            nullaction,
            handl,
            handr,
            handf,
            handb,

            grasp,
            graspl,
            graspr,
            graspf,
            graspb,
            ungrasp,

            hand1_home,
            hand1_rotate_cw,
            hand1_rotate_ccw,

            hand2_home,
            hand2_left,
            hand2_right,
            hand2_forward,
            hand2_back,

            hand2_grasp,
            hand2_graspf,
            hand2_graspb,
            hand2_graspl,
            hand2_graspr,
            hand2_ungrasp,

            eyel,
            eyer,
            eyef,
            eyeb,
            eye_home,

            magic_action,
            reverse_magic_action
        }

        Type type;

        public String TypeName() { return type.ToString(); }

        // Print the enum names and their int values out
        public void printAllValues() {
          foreach (Action.Type a in Enum.GetValues(typeof(Action.Type)))
          {
                int val = (int) a;
                Console.WriteLine($"{a.ToString()}={val}");
          }
        }


        // Constructor
        public Action(Type type)
        {
            this.type = type;
        }
    }

}