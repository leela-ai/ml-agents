using System;
using System.Collections.Generic;
using System.Text;


namespace Blocksworld
{
    public class BlocksWorldSensoriMotorSystem
    {

        public long _clock = 0; // SimpleSensoriMotorSystem has its own clock variable

        public long stepClock()
        {
            return _clock++;
        }
        public long clock() { return _clock; }
        public void setClock(long c) { _clock = c; }

        public bool multithreaded = false;


        /** config params */
        public bool enableHand2;
        public bool enableBlock1;
        public bool enableBlock2;
        public bool enableBlock3;
        public bool enableBlock4;
        public bool enableHandRotation;
        public bool enableEyeMotion;
        public bool enableTestActions;
        public bool enableGrasping;
        public bool testEnableExplainedResults;
        public bool enableVisualSensors;
        public bool enableFovealSensors;
        public bool enableShapeSensors = true;

        // Set holding current sensor states
        SensorState sensors = new SensorState();
        HashSet<String> reflexActions = new HashSet<String>();

        public int randomBlockMotionPeriod = 1000;
        public int randomBlockMotionAfter = 50000;

        public SensorState getSensorState() { return sensors; }

        public Random rand;
        int seed = -1;

        long graspHabituationPeriod;
        long graspRefractoryPeriod;
        long graspReflexEnabledBefore;
        double graspReflexProbability = 0;

        void readGridLimits() {
            
            handMinX = 1;
            handMinY = 1;
            handMaxX = 5;
            handMaxY = 5;
            
            // The coarse visual field system ranges from 1 to 5 in x and y
            visualFieldMinX = 1;
            visualFieldMinY = 1;
            visualFieldMaxX = 5;
            visualFieldMaxY = 5;
            
            // The glance field ranges from 0 to 4 in x and y
            glanceFieldMinX = 0;
            glanceFieldMinY = 0;
            glanceFieldMaxX = 4;
            glanceFieldMaxY = 4;
            
            handPropField = new SchemaField(handMinX, handMinY, handMaxX, handMaxY);
            visualField = new SchemaField(visualFieldMinX, visualFieldMinY, visualFieldMaxX, visualFieldMaxY);
            glanceField = new SchemaField(glanceFieldMinX, glanceFieldMinY, glanceFieldMaxX, glanceFieldMaxY);
        }
        


        public void readConfigParams()
        {
            enableHand2 = false;
            enableBlock1 = true;
            enableBlock2 = true;
            enableBlock3 = false;
            enableBlock4 = false;

            graspHabituationPeriod = 30;
            graspRefractoryPeriod = 20;
            graspReflexEnabledBefore = 4000;
            graspReflexProbability = 0.7;

            multithreaded = false;

            // Feature switches
            enableGrasping = true;
            enableVisualSensors = true;
            enableFovealSensors = true;
            enableEyeMotion = true;
            enableHandRotation = false;
            enableHand2 = false;
            //enableSandBox = config.getbool("feature.enable-sand-box");

            seed = -1;
            if (seed == -1)
            {
                Console.Write("BlocksWorld Physics Engine starting with no random seed");
                rand = new Random();
            }
            else
            {
                Console.Write("BlocksWorld Physics Engine starting with config value random seed of " + seed);
                rand = new Random(seed);
            }

            randomBlockMotionPeriod = 500;
            randomBlockMotionAfter = 500;
        }

        // Fields which define the time and space associated with the SensoriMotorSystem

        // The hand proprioceptive system ranges from 1 to 5 in x and y
        public static int handMinX = 1;
        public static int handMinY = 1;
        public static int handMaxX = 5;
        public static int handMaxY = 5;

        // The coarse visual field system ranges from 1 to 5 in x and y
        public static int visualFieldMinX = 1;
        public static int visualFieldMinY = 1;
        public static int visualFieldMaxX = 5;
        public static int visualFieldMaxY = 5;

        // The global field ranges from 1 to 7 in x and y
        public static int globalFieldMinX = 1;
        public static int globalFieldMinY = 1;
        public static int globalFieldMaxX = 7;
        public static int globalFieldMaxY = 7;

        // The glance field ranges from 0 to 4 in x and y
        public static int glanceFieldMinX = 0;
        public static int glanceFieldMinY = 0;
        public static int glanceFieldMaxX = 4;
        public static int glanceFieldMaxY = 4;


        public SchemaField handPropField;
        public SchemaField visualField;
        public SchemaField glanceField;

        // flags to control whether eye motion is allowed and hand rotation is allowed

        // Objects placed into the world for the simple sensorimotor system to detect
        // Everything is placed at 0,0

        public List<SimpleObject> objects = new List<SimpleObject>();
        public SimpleObject block1;
        public SimpleObject block2;
        public SimpleObject block3;
        public SimpleObject block4;
        public HandObject hand1;
        public HandObject hand2;
        public SimpleObject eye;

        // Fields associated with colors

        public static String TEXTURE_0 = "00";
        public static String TEXTURE_1 = "01";
        public static String TEXTURE_2 = "02";
        public static String TEXTURE_3 = "03";

        /* var objcolors = [
        "BLACK",
        "BLUE",
        "CYAN",
        "DARK_GRAY",
        "GRAY",
        "GREEN",
        "LIGHT_GRAY",
        "MAGENTA",
        "ORANGE",
        "PINK",
        "RED",
        "WHITE",
        "YELLOW",
        "rgb(100,120,180)",
        "rgb(20,180,180)",
        "rgb(255,100,80"];
        */

        static int PINK = 9;
        static int YELLOW = 12;
        static int BLUE = 1;


        static public int[] colors = {
        0x000000, // black
        0x0000FF, // blue
        0x00EEEE, // cyan
        0x696969, //     "DARK_GRAY"
        0xbebebe, //     "GRAY"
        0x00ff00, //     "GREEN"
        0xd3d3d3,         //     "LIGHT_GRAY"
        0xff00ff,         //     "MAGENTA"
        0xffa500,         //     "ORANGE"
        0xff1493,         //     "PINK"
        0xee0000,         //     "RED"
        0xffffff,         //     "WHITE"
        0xffff00,         //     "YELLOW"
        0x008080,
        0xA0522D,
        0x6A5ACD
    };

        // Helper functions
        public int indexToColor(int index)
        {
            return colors[index];
        }

        public int colorToIndex(int c)
        {
            for (int i = 1; i < colors.Length; i++)
            {
                int thisC = colors[i];
                if (c == thisC) return (i);
            }
            throw new Exception("Color not found in colors list: " + c);
        }




        // Constructor sets up objects and adds them to the objects list

        public BlocksWorldSensoriMotorSystem()
        {

            readConfigParams();
            readGridLimits();

            block1 = new SimpleObject(this, "b1", colors[PINK], TEXTURE_0, "circle", 0, 0); // 
            block2 = new SimpleObject(this, "b2", colors[YELLOW], TEXTURE_1, "triangle", 0, 0);
            block3 = new SimpleObject(this, "b3", colors[YELLOW], TEXTURE_1, "circle", 0, 0);
            block4 = new SimpleObject(this, "b4", colors[PINK], TEXTURE_1, "triangle", 0, 0);
            hand1 = new HandObject(this, "h", colors[BLUE], TEXTURE_2, 0, 0);
            hand2 = new HandObject(this, "j", colors[8], TEXTURE_3, 0, 0);
            eye = new SimpleObject(this, "v", colors[11], TEXTURE_0, "circle", 0, 0);

            eye.setLimit(glanceField.minX, glanceField.minY, glanceField.maxX, glanceField.maxY);
            eye.setPosition(2, 2);

            hand1.setLimit(handPropField.minX, handPropField.minY, handPropField.maxX, handPropField.maxY);
            hand2.setLimit(handPropField.minX, handPropField.minY, handPropField.maxX, handPropField.maxY);

            // Blocks are in the global coordinate system which spans from 0 to 6
            block1.setLimit(0, 0, 6, 6);
            block2.setLimit(0, 0, 6, 6);
            block3.setLimit(0, 0, 6, 6);
            block4.setLimit(0, 0, 6, 6);

	    initializeObjectPositions();

        }


        /**
           Actions which we want to advertise to the learning system as executable
         */
        public List<String> usableActionNames()
        {
            String[] capabilities = {
        "nullaction",
        "handl",
        "handr",
        "handf",
        "handb",
        "grasp",
        //       "graspl",
        //       "graspr",
        //       "graspb",
        //       "graspf",
        "ungrasp",
        "eyel",
        "eyer",
        "eyef",
        "eyeb",
        //"eye_home",
        //"hand1_home"
        //"hand1_rotate_cw",
        //"hand1_rotate_ccw",
        //"hand2_home",
        //"hand2_left",
        //"hand2_right",
        //"hand2_forward",
        //"hand2_back",
        //"hand2_grasp",
        //"hand2_ungrasp",
        };

            List<String> actions = new List<String>(capabilities);
            return actions;
        }

        /**
           Return map containing {sensors: {items: { }, actions: [a1,a2,...  ]}}
         */
        public Dictionary<String,Object> getCapabilities()
        {
            stepPhysicalWorld("0", new List<String>());
            Dictionary<String, Object> c = new Dictionary<String,Object>();
            Dictionary<String,Object> v = new Dictionary<String, Object>();
            c.Add("sensors", v);
            v.Add("actions", usableActionNames());

            Dictionary<String, Object> items = new Dictionary<String, Object>();

            foreach (KeyValuePair<String, SensorInput> entry in sensors.Items())
            {
               items.Add(entry.Key, true);
            }

            v.Add("items", items);

            return c;
        }


        public void initializeObjectPositions()
        {
            placeBlocks();
            placeHands();
        }

        void placeHands()
        {
            objects.Add(hand1);
            moveTo(hand1, 3, 3);
            if (enableHand2)
	    {
                objects.Add(hand2);
                moveTo(hand2, 5, 5);
            }
            else
	    {
		remove(hand2);
	    }
        }

        public void placeBlocks()
        {
            if (enableBlock1)
            {
                objects.Add(block1);
                moveTo(block1, 1, 5);
            }

            if (enableBlock2)
            {
                objects.Add(block2);
                moveTo(block2, 5, 5);
            }

            if (enableBlock3)
            {
                objects.Add(block3);
                moveTo(block3, 5, 1);
            }

            // even if enabled, we put block4 out of the playpen by default, for experiments where we later present novel item
            if (enableBlock4)
            {
                objects.Add(block4);
                // to get block out of sight, use forceMoveTo, it can move out of max bounds of the block
                forceMoveTo(block4, 100, 100);
            }




        }


        void randomlyMoveBlocks()
        {
            if (enableBlock1)
            {
                int x = rand.Next(5) + 1;
                int y = rand.Next(5) + 1;
                moveTo(block1, x, y);
            }

            if (enableBlock2)
            {
                int x = rand.Next(5) + 1;
                int y = rand.Next(5) + 1;
                moveTo(block2, x, y);
            }

            if (enableBlock3)
            {
                int x = rand.Next(5) + 1;
                int y = rand.Next(5) + 1;
                moveTo(block3, x, y);
            }

            if (enableBlock4)
            {
                int x = rand.Next(5) + 1;
                int y = rand.Next(5) + 1;
                moveTo(block4, x, y);
            }


        }


        /*
               public bool moveTo(SimpleObject obj, int xpos, int ypos) {

               Supported Commands:

               Move To
               cmd_name: "move_to"
               args: {"obj_name": "b1", "x": 2, "y": 3}

         */


        public Dictionary<String, Object> getInfoFromSMS(String name)
        {
            List<String> objs = new List<String> (){ "h", "j", "b1", "b2", "b3", "b4", "v" };
            Dictionary<string, object> vals = new Dictionary<string, object>();
            if (objs.Contains(name))
            {
                SimpleObject obj = findObjectByName(name);
                int x = obj.pos.x;
                int y = obj.pos.y;
                vals.Add(name + "x", x);
                vals.Add(name + "y", y);
            }
            return vals;
        }

        public Dictionary<String, Object> doSomethingInSMS(String cmdName, Object value)
        {
            Dictionary<String, Object> args = ((Dictionary<String, Object>)value);
            Dictionary<String, Object> vals = new Dictionary<string, object>();
            if ("move_to".Equals(cmdName))
            {

                String objname = (String)args["name_of_thing"];
                int x = (int)args["x"];
                int y = (int)args["y"];

                SimpleObject obj = findObjectByName(objname);

                obj.pos.x = x;
                obj.pos.y = y;

            }
            else if ("get_obj_pos".Equals(cmdName))
            {
                String objname = (String)args["name_of_thing"];
                List<String> objs = new List<String>() { "h", "j", "b1", "b2", "b3", "b4", "v" };
                if (objs.Contains(objname))
                {
                    SimpleObject obj = findObjectByName(objname);
                    int x = obj.pos.x;
                    int y = obj.pos.y;
                    vals.Add(objname + "x", x);
                    vals.Add(objname + "y", y);
                }

            }
            return vals;
        }


        /**
           import java.util.stream.*;

           List<Student> students = persons.stream()
            .filter(p -> p.getAge() > 18)
            .map(Student::new)
            .collect(Collectors.toList());
        */

        /**
         * This is the main method which causes the sensors in the world/sensor state to be updated
         *
         * @param actions list of primitive actions to perform
         * 
         * @return encoded string of sensor values  "NAME1=[0|1];NAME2=[0|1];..."
         * e.g., "hp22=1;hp23=0;hp24=0;"
         *          */

        public String stepPhysicalWorld(String seqid, List<String> actions)
        {
            doActions(actions);

            if (clock() % randomBlockMotionPeriod == 0 && clock() > randomBlockMotionAfter && hand1.graspedObject == null)
            {
                ungrasp(hand1);
                randomlyMoveBlocks();
            }

            setTouchSensors();
            setProprioSensors(sensors);
            setVisualSensors(sensors);

            // Someday we may have primitive actions that take more than one clock step to complete, so any which
            // actually complete this step will be returned as the value from this method.
            //
            // For now however every primitive action takes one clock step, so we can just return the list we were given.

            stepClock();

            // All actions complete in one clock step at the moment, so
            // just return the list of primitive actions we were just
            // given, plus any reflex actions we executed

            // actions.AddRange(reflexActions);    

            // sensors.setActions(actions);
            // sensors.setClock(clock());
            // List<Dictionary<String,Object>> objlocs = getDebugState();
            // sensors.setDebugInfo(objlocs);

            // Dictionary<String, Object> stateMap = sensors.toMap();
            
            reflexActions.Clear();

            return sensors.ToObservationString();

        }

        public void doActions(List<String> actions)
        {
            //HashMap<String,Action> outputList
            foreach (String aname in actions)
            {
                // Action.Type action = Action.Type.valueOf(aname);
                Enum.TryParse(aname, out Action.Type action);
                // CODE HERE To execute actions
                switch (action)
                {
                    case Action.Type.hand1_rotate_cw:
                        if (enableHandRotation)
                        {
                            hand1.rotateClockwise();
                        }
                        else
                        {
                            //logger.warn("HAND1_ROTATE_CW disabled");
                        }
                        break;
                    case Action.Type.hand1_rotate_ccw:
                        if (enableHandRotation)
                        {
                            hand1.rotateCounterClockwise();
                        }
                        else
                        {
                            //                    logger.warn("HAND1_ROTATE_CCW disabled");
                        }
                        break;
                    case Action.Type.handf:
                        moveForward(hand1);
                        break;
                    case Action.Type.handb:
                        moveBack(hand1);
                        break;
                    case Action.Type.handl:
                        moveLeft(hand1);
                        break;
                    case Action.Type.handr:
                        moveRight(hand1);
                        break;
                    case Action.Type.hand1_home:
                        if (enableTestActions)
                        {
                            moveTo(hand1, 1, 1);
                        }
                        break;
                    case Action.Type.hand2_forward:
                        moveForward(hand2);
                        break;
                    case Action.Type.hand2_back:
                        moveBack(hand2);
                        break;
                    case Action.Type.hand2_left:
                        moveLeft(hand2);
                        break;
                    case Action.Type.hand2_right:
                        moveRight(hand2);
                        break;
                    case Action.Type.hand2_home:
                        if (enableTestActions && enableHand2)
                        {
                            moveTo(hand2, 1, 1);
                        }
                        break;

                    case Action.Type.eyeb:
                        if (enableEyeMotion)
                        {
                            limitBack(eye);
                        }
                        else
                        {
                            //                    logger.warn("eyeb disabled");
                        }
                        break;

                    case Action.Type.eyef:
                        if (enableEyeMotion)
                        {
                            limitForward(eye);
                        }
                        else
                        {
                            //                    logger.warn("eyef disabled");
                        }
                        break;

                    case Action.Type.eyel:
                        if (enableEyeMotion)
                        {
                            limitLeft(eye);
                        }
                        else
                        {
                            //logger.warn("eyel disabled");
                        }
                        break;

                    case Action.Type.eyer:
                        if (enableEyeMotion)
                        {
                            limitRight(eye);
                        }
                        else
                        {
                            //logger.warn("eyer disabled");
                        }
                        break;

                    case Action.Type.eye_home:
                        if (enableTestActions)
                        {
                            eye.pos.x = eye.pos.y = 2;
                        }
                        break;


                    case Action.Type.grasp:  // grasp any object touching the hand, object touching hand front is preferred
                        if (enableGrasping)
                        {
                            grasp(hand1);
                        }
                        break;
                    case Action.Type.graspl:   // grasp specifically object on left of hand
                        if (enableGrasping)
                        {
                            graspl(hand1);
                        }
                        break;
                    case Action.Type.graspr: // grasp specifically object on right of hand
                        if (enableGrasping)
                        {
                            graspr(hand1);
                        }
                        break;
                    case Action.Type.graspf: // grasp specifically object on front of hand
                        if (enableGrasping)
                        {
                            graspf(hand1);
                        }
                        break;
                    case Action.Type.graspb: // grasp specifically object on back of hand
                        if (enableGrasping)
                        {
                            graspb(hand1);
                        }
                        break;



                    case Action.Type.ungrasp:
                        if (enableGrasping)
                        {
                            ungrasp(hand1);
                        }
                        break;
                    case Action.Type.hand2_graspl:
                        if (enableGrasping)
                        {
                            graspl(hand1);
                        }
                        break;
                    case Action.Type.hand2_graspr:
                        if (enableGrasping)
                        {
                            graspr(hand1);
                        }
                        break;
                    case Action.Type.hand2_graspf:
                        if (enableGrasping)
                        {
                            graspf(hand1);
                        }
                        break;
                    case Action.Type.hand2_graspb:
                        if (enableGrasping)
                        {
                            graspb(hand1);
                        }
                        break;
                    case Action.Type.hand2_ungrasp:
                        if (enableGrasping)
                        {
                            ungrasp(hand1);
                        }
                        break;
                    case Action.Type.magic_action:
                        if (testEnableExplainedResults)
                        {
                            double r = rand.NextDouble();
                            if (r < 0.01)
                            {
                                // use MAGIC_ACTION to put block2 at 32
                                moveTo(block2, 3, 2);
                            }
                        }
                        break;
                    case Action.Type.reverse_magic_action:
                        if (testEnableExplainedResults)
                        {
                            remove(block2);
                        }
                        break;
                    case Action.Type.nullaction:
                        break;


                    default:
                        Console.Write("unknown Action type " + action);
                        break;
                }
            }
        }

        public void setTouchSensors()
        {
            setTouchSensors(hand1);

            sensors.setSensorValue(hand1.name + ".hcl", hand1.grasping, clock());
            sensors.setSensorValue(hand1.name + ".hgr", hand1.graspedObject != null, clock());

            sensors.setSensorValue(hand2.name + ".hcl", hand2.grasping, clock());
            sensors.setSensorValue(hand2.name + ".hgr", hand2.graspedObject != null, clock());

        }

        public void setProprioSensors(SensorState w)
        {
            // Set Proprioceptive sensors for each of two hands and the eye to be on
            setObjProprioSensors(hand1);
            setObjProprioSensors(hand2);
            setObjProprioSensors(eye);
        }

        public void setVisualSensors(SensorState w)
        {
            if (enableVisualSensors)
            {
                setCoarseVisualFieldSensors(w);
                if (enableFovealSensors)
                {
                    setFovealSensors(w);
                }
            }
        }

        public void setTouchSensors(HandObject h)
        {
            SimpleObject _front = objectTouchingFront(h);
            SimpleObject _back = objectTouchingBack(h);
            SimpleObject _left = objectTouchingLeft(h);
            SimpleObject _right = objectTouchingRight(h);

            // Set coarse tactile sensors
            //        sensors.setSensorValue(h.name + ".tactf", front != null && hand1.graspedObject == null , clock);
            sensors.setSensorValue(h.name + ".tactf", _front != null, clock());
            sensors.setSensorValue(h.name + ".tactb", _back != null, clock());
            sensors.setSensorValue(h.name + ".tactl", _left != null, clock());
            sensors.setSensorValue(h.name + ".tactr", _right != null, clock());

            // texture sensor
            SimpleObject[] objs = { _front, _back, _left, _right };
            bool text0 = false;
            bool text1 = false;
            bool text2 = false;
            bool text3 = false;

            foreach (SimpleObject obj in objs)
            {
                text0 = text0 || (obj != null && obj.texture.Equals(TEXTURE_0));
                text1 = text1 || (obj != null && obj.texture.Equals(TEXTURE_1));
                text2 = text2 || (obj != null && obj.texture.Equals(TEXTURE_2));
                text3 = text3 || (obj != null && obj.texture.Equals(TEXTURE_3));
            }

            sensors.setSensorValue(h.name + ".text0", text0, clock());
            sensors.setSensorValue(h.name + ".text1", text1, clock());
            sensors.setSensorValue(h.name + ".text2", text2, clock());
            sensors.setSensorValue(h.name + ".text3", text3, clock());
        }

        public void setCoarseVisualFieldSensors(SensorState w)
        {
            int minx = visualField.minX;
            int miny = visualField.minY;
            int glancex = eye.pos.x;
            int glancey = eye.pos.y;
            int maxx = visualField.maxX;
            int maxy = visualField.maxY;

            int glanceMidX = (glanceField.maxX - glanceField.minX)/2 + glanceField.minX;
            int glanceMidY = (glanceField.maxY - glanceField.minY)/2 + glanceField.minY;


            // Loop through every position in the visual sensor grid and set Sensor Value to true if there is an object at x and y 
            for (int i = minx; i <= maxx; i++)
            {
                for (int j = miny; j <= maxy; j++)
                {

		    String ipad = i.ToString("D2"); // zero padded two digit int
		    String jpad = j.ToString("D2"); // zero padded two digit int

                    String nameForVisualSensor = $"vf{ipad}{jpad}";
                    // mapping from coarse visual field (i,j) to global field in which objects are positioned
                    // add eye x and y offset but correct for starting position coordinates for the visual Field
                    SimpleObject isThereAnObjAtPos = objectAtPosition((i + glancex - glanceMidX), (j + glancey - glanceMidY));
                    sensors.setSensorValue(nameForVisualSensor, isThereAnObjAtPos != null, clock());
                }
            }
        }

        static int FOVEAL_OFFSET = 1;

        public void setFovealSensors(SensorState w)
        {
            int glancex = eye.pos.x;
            int glancey = eye.pos.y;
            // 3 x 3 square foveal field
            for (int x = 1; x < 4; x++)
            {
                for (int y = 1; y < 4; y++)
                {
                    String fovColorSensorName = $"fov{x}{y}";
                    String fovShapeSensorName = $"fvs{x}{y}";
                    // iterate over texture detail names
                    String nameForFovealCircleSensor = $"{fovShapeSensorName}.circle";
                    String nameForFovealTriangleSensor = $"{fovShapeSensorName}.triangle";
                    String nameForFovealSquareSensor = $"{fovShapeSensorName}.square";

                    int thisXPos = ((x + glancex) - FOVEAL_OFFSET);
                    int thisYPos = ((y + glancey) - FOVEAL_OFFSET);
                    // Check to see if there is an object at a specific foveal sensor location
                    SimpleObject obj = objectAtPosition(thisXPos, thisYPos);
                    if (obj == null)
                    {
                        // we have 16 possible colors, hence 16 color sensors at each foveal x,y location
                        for (int i = 0; i < colors.Length; i++)
                        {
                            String ipad = i.ToString("D2"); // zero padded two digit int
                            String nameForFovealColorSensor =$"{fovColorSensorName}.{ipad}";
                            sensors.setSensorValue(nameForFovealColorSensor, false, clock());
                        }
                        if (enableShapeSensors)
                        {
                            sensors.setSensorValue(nameForFovealCircleSensor, false, clock());
                            sensors.setSensorValue(nameForFovealTriangleSensor, false, clock());
                            sensors.setSensorValue(nameForFovealSquareSensor, false, clock());
                        }
                    }
                    else
                    {
                        for (int i = 0; i < colors.Length; i++)
                        {
                            bool isThisSensorOn = colors[i] == obj.color;
                            // If there is and the color of the object
                            // matches the specific color we are at, turn
                            // this sensor on
                            String ipad = i.ToString("D2"); // zero padded two digit int

                            String nameForFovealColorSensor = $"{fovColorSensorName}.{ipad}";

                            sensors.setSensorValue(nameForFovealColorSensor, isThisSensorOn, clock());
                        }
                        if (enableShapeSensors)
                        {
                            sensors.setSensorValue(nameForFovealCircleSensor, "circle".Equals(obj.shape), clock());
                            sensors.setSensorValue(nameForFovealTriangleSensor, "triangle".Equals(obj.shape), clock());
                            sensors.setSensorValue(nameForFovealSquareSensor, "square".Equals(obj.shape), clock());
                        }
                    }
                }
            }
        }

        /*
        public void setFovealSensors(SensorState w) {
            int glancex = eye.pos.x;
            int glancey = eye.pos.y;
            // 3 x 3 square foveal field
            for(int x = 1; x < 4; x++) {
                for(int y = 1; y < 4; y++) {
                    String fovSensorName = String.format("fov%d%d",x,y);
                    // iterate over texture detail names
                    for (int i = 0; i < colors.length; i++) {
                        String nameForFovealDetailSensor = String.format("%s.%02d",fovSensorName,i);
                        int thisXPos = (((x-2)+glancex)-this.visualField.minX)+FOVEAL_OFFSET;
                        int thisYPos = (((y-2)+glancey)-this.visualField.minY)+FOVEAL_OFFSET;
                        // Check to see if there is an object at a specific foveal sensor location
                        SimpleObject obj = objectAtPosition(thisXPos, thisYPos);
                        if (obj == null) {
                            sensors.setSensorValue(nameForFovealDetailSensor, false, clock());
                        } else {
                            bool isThisSensorOn = colors[i].Equals(obj.color);
                            // If there is and the color of the object matches the specific color we are at, turn this sensor on
                            sensors.setSensorValue(nameForFovealDetailSensor, isThisSensorOn, clock());
                        }
                    }
                }
            }
        }
        */


        public void setObjProprioSensors(SimpleObject obj)
        {
            int minx = obj.minx;
            int miny = obj.miny;
            int maxx = obj.maxx;
            int maxy = obj.maxy;
            // Loop through every position in sensor grid and set Sensor Value
            // to true if x and y position of object matches x and y position
            // of that Sensor Value Note: Could have done this by directly
            // setting sensor at x,y of object position to be true. However
            // this would require that the sensor grid be created with all
            // default values of false and that whenever an object moves the
            // code would be required to maintain the invariant that the
            // sensor be set to false at the previous location of the object

            for (int i = minx; i <= maxx; i++)
            {
                for (int j = miny; j <= maxy; j++)
                {
                    String objPos = nameForObjPos(obj, i, j);
                    sensors.setSensorValue(objPos, ((obj.getPosition().x) == i) && ((obj.getPosition().y) == j), clock());
                }
            }
        }


        // Helper functions

        // Handling objects, finding their positions, getting their names

        public String nameForObjPos(SimpleObject obj, int x, int y)
        {
	    String xpad = x.ToString("D2"); // zero padded two digit int
	    String ypad = y.ToString("D2"); // zero padded two digit int
            return $"{obj.name}p{xpad}{ypad}";
        }

        SimpleObject objectAtPosition(int x, int y)
        {
            foreach (SimpleObject o in objects)
            {
                Vec2 pos = o.getPosition();
                if (pos.x == x && pos.y == y)
                {
                    return o;
                }
            }
            return null;
        }


        // Methods associated with moving

        // Help check if within range
        int range(int v, int min, int max)
        {
            if (v < min)
            {
                return min;
            }
            else if (v > max)
            {
                return max;
            }
            else
            {
                return v;
            }
        }

        public bool moveTo(SimpleObject obj, Vec2 dest)
        {
            return moveTo(obj, dest.x, dest.y);
        }

        public void forceMoveTo(SimpleObject obj, int xpos, int ypos)
        {
            obj.pos.x = xpos;
            obj.pos.y = ypos;
        }


        SimpleObject findObjectByName(String name)
        {
            foreach (SimpleObject o in objects)
            {
                if (o.name.Equals(name))
                {
                    return o;
                }
            }

            if (eye.name.Equals(name))
            {
                return eye;
            }

            return null;
        }

        public void moveobj(String name, int xpos, int ypos)
        {
            SimpleObject obj = findObjectByName(name);
            moveTo(obj, xpos, ypos);
        }

        public bool moveTo(SimpleObject obj, int xpos, int ypos)
        {
            if (obj.pos.x == xpos && obj.pos.y == ypos)
            {
                // we're already at destination
                return true;
            }

            if (canMoveTo(obj, xpos, ypos))
            {
                obj.pos.x = xpos;
                obj.pos.y = ypos;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool canMoveTo(SimpleObject obj, Vec2 dest)
        {
            return canMoveTo(obj, dest.x, dest.y);
        }

        public bool canMoveTo(SimpleObject obj, int xpos, int ypos)
        {
            int tx = range(xpos, obj.minx, obj.maxx);
            int ty = range(ypos, obj.miny, obj.maxy);

            if (tx != xpos)
            {
                return false;
            }

            if (ty != ypos)
            {
                return false;
            }

            SimpleObject obj2 = objectAtPosition(tx, ty);
            if (obj2 == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        // Simple move just check range limits
        public void limitMoveTo(SimpleObject obj, Vec2 dest)
        {
            obj.pos.x = range(dest.x, obj.minx, obj.maxx);
            obj.pos.y = range(dest.y, obj.miny, obj.maxy);
        }


        public void limitForward(SimpleObject obj)
        {
            Vec2 pos = obj.getPosition();
            Vec2 dest = pos.Add(FORWARD);
            limitMoveTo(obj, dest);
        }

        public void limitBack(SimpleObject obj)
        {
            Vec2 pos = obj.getPosition();
            Vec2 dest = pos.Add(BACK);
            limitMoveTo(obj, dest);
        }

        public void limitRight(SimpleObject obj)
        {
            Vec2 pos = obj.getPosition();
            Vec2 dest = pos.Add(RIGHT);
            limitMoveTo(obj, dest);

        }

        public void limitLeft(SimpleObject obj)
        {
            Vec2 pos = obj.getPosition();
            Vec2 dest = pos.Add(LEFT);
            limitMoveTo(obj, dest);
        }


        public SimpleObject objectTouchingFront(SimpleObject obj)
        {
            Vec2 pos = obj.getPosition();
            return objectAtPosition(pos.x, pos.y + 1);
        }

        public SimpleObject objectTouchingBack(SimpleObject obj)
        {
            Vec2 pos = obj.getPosition();
            return objectAtPosition(pos.x, pos.y - 1);
        }

        public SimpleObject objectTouchingLeft(SimpleObject obj)
        {
            Vec2 pos = obj.getPosition();
            return objectAtPosition(pos.x - 1, pos.y);
        }

        public SimpleObject objectTouchingRight(SimpleObject obj)
        {
            Vec2 pos = obj.getPosition();
            return objectAtPosition(pos.x + 1, pos.y);
        }

        public void grasp(HandObject h)
        {
            SimpleObject front = objectTouchingFront(h);
            SimpleObject back = objectTouchingBack(h);
            SimpleObject left = objectTouchingLeft(h);
            SimpleObject right = objectTouchingRight(h);
            h.grasp(clock());
            List<SimpleObject> touched = new List<SimpleObject>();

            if (front != null) touched.Add(front);
            if (back != null) touched.Add(back);
            if (left != null) touched.Add(left);
            if (right != null) touched.Add(right);

            if (touched.Count > 0)
            {
                h.graspedObject = touched[rand.Next(touched.Count)];
            }

        }

        public void graspf(HandObject h)
        {
            SimpleObject front = objectTouchingFront(h);
            SimpleObject obj = null;
            if (front != null)
            {
                obj = front;
            }
            h.graspedObject = obj;
            h.grasp(clock());
        }

        public void graspb(HandObject h)
        {
            SimpleObject back = objectTouchingBack(h);
            SimpleObject obj = null;

            if (back != null)
            {
                obj = back;
            }
            h.graspedObject = obj;
            h.grasp(clock());
        }

        public void graspl(HandObject h)
        {
            SimpleObject left = objectTouchingLeft(h);
            SimpleObject obj = null;

            if (left != null)
            {
                obj = left;
            }
            h.graspedObject = obj;
            h.grasp(clock());
        }

        public void graspr(HandObject h)
        {
            SimpleObject right = objectTouchingRight(h);
            SimpleObject obj = null;

            if (right != null)
            {
                obj = right;
            }
            h.graspedObject = obj;
            h.grasp(clock());
        }

        public void ungrasp(HandObject h)
        {
            h.graspedObject = null;
            h.ungrasp(clock());
        }

        void remove(SimpleObject o)
        {
            o.setPosition(new Vec2(-100, -100));
        }

        Vec2 FORWARD = new Vec2(0, 1);
        Vec2 BACK = new Vec2(0, -1);
        Vec2 LEFT = new Vec2(-1, 0);
        Vec2 RIGHT = new Vec2(1, 0);


        /**
         * If hand is grasping something, make sure grasped object can move forward.
         * Hand will occupy prior position of grasped object.
         */
        public void moveForward(HandObject h)
        {
            moveHand(h, FORWARD);
        }

        public void moveBack(HandObject h)
        {
            moveHand(h, BACK);
        }

        public void moveLeft(HandObject h)
        {
            moveHand(h, LEFT);
        }

        public void moveRight(HandObject h)
        {
            moveHand(h, RIGHT);
        }

        void moveHand(HandObject h, Vec2 delta)
        {
            Vec2 hpos = h.getPosition().copy();
            Vec2 hdest = hpos.Add(delta);

            // Is hand grasping a block? See if we can move the hand and block
            // to the desired destination.
            if (h.graspedObject != null)
            {
                SimpleObject g = h.graspedObject;

                // save the grasped object start position
                Vec2 gpos = g.getPosition().copy();
                // grasped object target positino
                Vec2 gdest = gpos.Add(delta);

                // logger.info(clock()+": "+h+" grasping g at "+gpos +" moving to "+gdest);


                // Get g out of the way of the hand
                remove(g);
                // Get hand out of way
                remove(h);

                if (canMoveTo(h, hdest) &&
                        canMoveTo(g, gdest))
                {
                    if (gdest.x == 6)
                    {
                        // logger.info("how can something be put at x=6?");
                    }
                    // move hand forward
                    h.setPosition(hdest);
                    g.setPosition(gdest);
                    // logger.info(clock() + ": dragged obj "+g+" to "+gdest);
                }
                else
                {
                    // couldn't move, leave things where they were
                    g.setPosition(gpos);
                    h.setPosition(hpos);
                }
            }
            else
            {
                if (canMoveTo(h, hdest))
                {
                    h.setPosition(hdest);
                }
            }

          
            invokeGraspReflex(h);

        }



        void invokeGraspReflex(HandObject h)
        {
            SimpleObject front = objectTouchingFront(h);
            SimpleObject back = objectTouchingBack(h);
            SimpleObject left = objectTouchingLeft(h);
            SimpleObject right = objectTouchingRight(h);

            // If the hand is in a grasp due to reflex, and it's been longer than habituation time, it gets
            // 'tired' and opens the hand
            if (clock() - h.graspStartTime > graspHabituationPeriod)
            {
                ungrasp(h);
            }
            else if (clock() - h.ungraspStartTime > graspRefractoryPeriod)
            {
                if (clock() < graspReflexEnabledBefore)
                {// grasp reflex is enabled
                    if (rand.NextDouble() < graspReflexProbability)
                    { // we have randomly decided to grasp, if anything is touching the hand
                      // TODO we need to send this action back to the Brain, to let it know we did a grasp 
                      // Reflex grasp
                        if (front != null)
                        {
                            graspf(h);
                            reflexActions.Add("grasp");
                            h.reflexGrasp = true;
                        }
                        else if (left != null)
                        {
                            graspl(h);
                            reflexActions.Add("grasp");
                            h.reflexGrasp = true;
                        }
                        else if (right != null)
                        {
                            graspr(h);
                            reflexActions.Add("grasp");
                            h.reflexGrasp = true;
                        }
                        else if (back != null)
                        {
                            graspb(h);
                            reflexActions.Add("grasp");
                            h.reflexGrasp = true;
                        }
                    }
                }
            }
        }

        
        /**
         * move a SimpleObject foward one grid position
         */
        public void moveForward(SimpleObject obj)
        {
            Vec2 pos = obj.getPosition();
            Vec2 dest = pos.Add(FORWARD);
            moveTo(obj, dest);
        }

        public void moveBack(SimpleObject obj)
        {
            Vec2 pos = obj.getPosition();
            Vec2 dest = pos.Add(BACK);
            moveTo(obj, dest);
        }

        public void moveRight(SimpleObject obj)
        {
            Vec2 pos = obj.getPosition();
            Vec2 dest = pos.Add(RIGHT);
            moveTo(obj, dest);

        }

        public void moveLeft(SimpleObject obj)
        {
            Vec2 pos = obj.getPosition();
            Vec2 dest = pos.Add(LEFT);
            moveTo(obj, dest);
        }


        // Rendering functions

        /**
         * print map of where things are
         */
        public String showWorld()
        {
            StringBuilder b = new StringBuilder();
            b.Append("\n  -------------------\n");
            for (int y = 6; y >= 0; y--)
            {
                b.Append($"{y} |");
                for (int x = 0; x <= 6; x++)
                {
                    SimpleObject obj = objectAtPosition(x, y);
                    if (obj == null)
                    {
                        b.Append("  |");
                    }
                    else
                    {
                        b.Append($"{obj.name}|");
                    }
                }
                b.Append("\n  -------------------\n");
            }
            b.Append("    ");
            for (int x = 0; x <= 6; x++)
            {
                b.Append($" {x} ");
            }
            return b.ToString();
        }


        static int N_GRIDS = 7;


        Dictionary<String,Object> gridcell(SimpleObject obj)
        {
            int r = (obj.color >> 16) & 0xFF;
            int g = (obj.color >> 8) & 0xFF;
            int b = obj.color & 0xFF;

            Dictionary<String,Object> cell = new Dictionary<String,Object>();
            cell.Add("name", obj.name);
            cell.Add("x", obj.getPosition().x);
            cell.Add("y", obj.getPosition().y);
            String color = $"rgb({r},{g},{b})";
            cell.Add("color", color);
            cell.Add("shape", obj.shape);
            return cell;
        }


        // Return a json object of sensorstate
        /*
         * public String jsonifySensorState(SensorState ss)
        {
            return JSONValue.toJSONString(ss.items);
        }
        */

        public List<Dictionary<String,Object>> getDebugState()
        {
            List<SimpleObject> objs = new List<SimpleObject>() { hand1, hand2, eye };
            if (enableBlock1) { objs.Add(block1); }
            if (enableBlock2) { objs.Add(block2); }
            if (enableBlock3) { objs.Add(block3); }
            if (enableBlock4) { objs.Add(block4); }
            List<Dictionary<String,Object>> val = new List<Dictionary<String,Object>>();
            foreach (SimpleObject obj in objs)
            {
                val.Add(obj.toMap());
            }

            // Add in eyepos, hand positions
            return val;
        }

        // Convert debug state to LinkedList<Map<String, Object>>
        public List<Dictionary<String,Object>> getDebugObjsList() { throw new Exception("getDebugObjsList not defined"); }

        // Returns a list of maps representing the physical object locations
        public List<Dictionary<String,Object>> objlocList()
        {
            List<Dictionary<String,Object>> physobjs = new List<Dictionary<String,Object>>();
            for (int y = 0; y <= N_GRIDS; y++)
            {
                for (int x = 0; x <= N_GRIDS; x++)
                {
                    SimpleObject obj = objectAtPosition(x, y);
                    if (obj == null)
                    {
                        // empty grid
                    }
                    else
                    {
                        physobjs.Add(gridcell(obj));
                    }
                }
            }
            return physobjs;
        }




        public void initPrimitiveActions()
        {
            // nothing to actually do here, the Action class defines all
            // known action types, and they don't have any state, they are
            // just predefined enum types, so do not need any
            // initialization
        }

    }

}
