using UnityEditor;
using UnityEngine;

namespace Cozy_House_Generator.Editor.CustomInspector.Props
{
    public class ToggleStyle 
    {
        private const string Path = "Assets/Cozy House Generator/Editor/Res/Icons/";

        private const string AnyPath                            = Path + "any.png";
        private const string DoorHorizontalPath                 = Path + "door_horizontal.png";
        private const string DoorVerticalPath                   = Path + "door_vertical.png";
        private const string FloorPath                          = Path + "floor.png";
        private const string ForbiddenPath                      = Path + "outside.png";
        private const string Props1Path                         = Path + "props1.png";
        private const string WallHorizontalPath                 = Path + "wall_horizontal.png";
        private const string WallVerticalPath                   = Path + "wall_vertical.png";
        private const string WindowHorizontalPath               = Path + "window_horizontal.png";
        private const string WindowVerticalPath                 = Path + "window_vertical.png";
        private const string HoverPath                          = Path + "light.png";
        private const string WallOrWindowHorizontalPath         = Path + "wall_or_window_horizontal.png";
        private const string WallOrWindowVerticalPath           = Path + "wall_or_window_vertical.png";
        private const string DoorOrWindowHorizontalPath         = Path + "door_or_window_horizontal.png";
        private const string DoorOrWindowVerticalPath           = Path + "door_or_window_vertical.png";
        private const string DoorOrWallHorizontalPath           = Path + "door_or_wall_horizontal.png";
        private const string DoorOrWallVerticalPath             = Path + "door_or_wall_vertical.png";
        private const string DoorOrWallOrWindowHorizontalPath   = Path + "door_or_wall_or_window_horizontal.png";
        private const string DoorOrWallOrWindowVerticalPath     = Path + "door_or_wall_or_window_vertical.png";
        private const string PropLooksForwardPath               = Path + "prop_looks_forward.png";
        private const string PropLooksBackwardPath              = Path + "prop_looks_backward.png";
        private const string PropLooksRightPath                 = Path + "prop_looks_right.png";
        private const string PropLooksLeftPath                  = Path + "prop_looks_left.png";
        private const string WallAnyHorizontalPath              = Path + "wall_any_horizontal.png";
        private const string WallAnyVerticalPath                = Path + "wall_any_vertical.png";
        
        
        public Texture any;
        public Texture doorHorizontal;
        public Texture doorVertical;
        public Texture floor;
        public Texture forbidden;
        public Texture props1;
        public Texture wallHorizontal;
        public Texture wallVertical;
        public Texture windowHorizontal;
        public Texture windowVertical;
        public Texture wallOrWindowHorizontal;
        public Texture wallOrWindowVertical;
        public Texture doorOrWindowHorizontal;
        public Texture doorOrWindowVertical;
        public Texture doorOrWallHorizontal;
        public Texture doorOrWallVertical;
        public Texture doorOrWallOrWindowHorizontal;
        public Texture doorOrWallOrWindowVertical;
        public Texture propLooksForward;
        public Texture propLooksBackward;
        public Texture propLooksRight;
        public Texture propLooksLeft;
        public Texture wallAnyHorizontal;
        public Texture wallAnyVertical;

        public GUIStyle cellToggle;

        
        public ToggleStyle()
        {
            Initialize();
        }
        
        
        public void Initialize()
        {
            any                          = (Texture2D)AssetDatabase.LoadAssetAtPath(AnyPath,                          typeof(Texture2D));
            doorHorizontal               = (Texture2D)AssetDatabase.LoadAssetAtPath(DoorHorizontalPath,               typeof(Texture2D));
            doorVertical                 = (Texture2D)AssetDatabase.LoadAssetAtPath(DoorVerticalPath,                 typeof(Texture2D));
            floor                        = (Texture2D)AssetDatabase.LoadAssetAtPath(FloorPath,                        typeof(Texture2D));
            forbidden                    = (Texture2D)AssetDatabase.LoadAssetAtPath(ForbiddenPath,                    typeof(Texture2D));
            props1                       = (Texture2D)AssetDatabase.LoadAssetAtPath(Props1Path,                       typeof(Texture2D));
            wallHorizontal               = (Texture2D)AssetDatabase.LoadAssetAtPath(WallHorizontalPath,               typeof(Texture2D));
            wallVertical                 = (Texture2D)AssetDatabase.LoadAssetAtPath(WallVerticalPath,                 typeof(Texture2D));
            windowHorizontal             = (Texture2D)AssetDatabase.LoadAssetAtPath(WindowHorizontalPath,             typeof(Texture2D));
            windowVertical               = (Texture2D)AssetDatabase.LoadAssetAtPath(WindowVerticalPath,               typeof(Texture2D));
            wallOrWindowHorizontal       = (Texture2D)AssetDatabase.LoadAssetAtPath(WallOrWindowHorizontalPath,       typeof(Texture2D));
            wallOrWindowVertical         = (Texture2D)AssetDatabase.LoadAssetAtPath(WallOrWindowVerticalPath,         typeof(Texture2D));
            doorOrWindowHorizontal       = (Texture2D)AssetDatabase.LoadAssetAtPath(DoorOrWindowHorizontalPath,       typeof(Texture2D));
            doorOrWindowVertical         = (Texture2D)AssetDatabase.LoadAssetAtPath(DoorOrWindowVerticalPath,         typeof(Texture2D));
            doorOrWallHorizontal         = (Texture2D)AssetDatabase.LoadAssetAtPath(DoorOrWallHorizontalPath,         typeof(Texture2D));
            doorOrWallVertical           = (Texture2D)AssetDatabase.LoadAssetAtPath(DoorOrWallVerticalPath,           typeof(Texture2D));
            doorOrWallOrWindowHorizontal = (Texture2D)AssetDatabase.LoadAssetAtPath(DoorOrWallOrWindowHorizontalPath, typeof(Texture2D));
            doorOrWallOrWindowVertical   = (Texture2D)AssetDatabase.LoadAssetAtPath(DoorOrWallOrWindowVerticalPath,   typeof(Texture2D));
            propLooksForward             = (Texture2D)AssetDatabase.LoadAssetAtPath(PropLooksForwardPath,             typeof(Texture2D));
            propLooksBackward            = (Texture2D)AssetDatabase.LoadAssetAtPath(PropLooksBackwardPath,            typeof(Texture2D));
            propLooksRight               = (Texture2D)AssetDatabase.LoadAssetAtPath(PropLooksRightPath,               typeof(Texture2D));
            propLooksLeft                = (Texture2D)AssetDatabase.LoadAssetAtPath(PropLooksLeftPath,                typeof(Texture2D));
            wallAnyHorizontal            = (Texture2D)AssetDatabase.LoadAssetAtPath(WallAnyHorizontalPath,            typeof(Texture2D));
            wallAnyVertical              = (Texture2D)AssetDatabase.LoadAssetAtPath(WallAnyVerticalPath,              typeof(Texture2D));
            
            cellToggle = new GUIStyle
            {
                hover = {background = (Texture2D) AssetDatabase.LoadAssetAtPath(HoverPath, typeof(Texture2D))}
            };
        }
        
    }
}