using Cozy_House_Generator.Scripts.Core.DataTypes;
using UnityEngine;
using static Cozy_House_Generator.Editor.CustomInspector.Prefab;
using static UnityEditor.EditorGUILayout;

namespace Cozy_House_Generator.Editor.CustomInspector
{
	
	public static class UseOnFloorResolverDrawer 
	{

		public static void Draw(UseOnFloorResolver resolver)
		{
			BeginHorizontal();
			LabelField("Use On", GUILayout.Width(45));
			SetValue(ref resolver.useOn, (UseOnFloorResolver.UseOn) EnumPopup(resolver.useOn, GUILayout.Width(45)));

			if (resolver.useOn == UseOnFloorResolver.UseOn.All)
			{
				EndHorizontal();

				BeginHorizontal();
				LabelField("  ", GUILayout.Width(45));
				SetValue(ref resolver.haveExceptionFloors,
				         ToggleLeft("Except", resolver.haveExceptionFloors, GUILayout.MaxWidth(55)));
			}

			switch (resolver.useOn)
			{
				case UseOnFloorResolver.UseOn.All:

					if (resolver.haveExceptionFloors == false)
					{
						EndHorizontal();
						break;
					}

					var exceptWidth = 50;
					if (resolver.exceptRange == UseOnFloorResolver.FloorsRange.FirstAndLast)
						exceptWidth = 100;
					
					SetValue(ref resolver.exceptRange,
						(UseOnFloorResolver.FloorsRange) EnumPopup("", resolver.exceptRange, GUILayout.MaxWidth(exceptWidth)));
					
					switch (resolver.exceptRange)
					{
						case UseOnFloorResolver.FloorsRange.First:
							SetValue(ref resolver.exceptFirstRange, IntField(resolver.exceptFirstRange, GUILayout.MaxWidth(140)));
							EndHorizontal();
							break;

						case UseOnFloorResolver.FloorsRange.Last:
							SetValue(ref resolver.exceptLastRange, IntField(resolver.exceptLastRange, GUILayout.MaxWidth(75)));
							EndHorizontal();
							break;

						case UseOnFloorResolver.FloorsRange.FirstAndLast:
							EndHorizontal();
							
							BeginHorizontal();
							LabelField("  ", GUILayout.Width(105));
							LabelField("First", GUILayout.Width(45));
							SetValue(ref resolver.exceptFirstRange,
								IntField(resolver.exceptFirstRange, GUILayout.MaxWidth(40)));
							EndHorizontal();
							
							BeginHorizontal();
							LabelField("  ", GUILayout.Width(105));
							LabelField("Last", GUILayout.Width(45));
							SetValue(ref resolver.exceptLastRange, IntField(resolver.exceptLastRange, GUILayout.MaxWidth(40)));
							EndHorizontal();
							
							break;
					}

					break;

				case UseOnFloorResolver.UseOn.Only:
					
					var onlyWidth = 50;
					if (resolver.onlyRange == UseOnFloorResolver.FloorsRange.FirstAndLast)
						onlyWidth = 100;
					
					
					SetValue(ref resolver.onlyRange,
						(UseOnFloorResolver.FloorsRange) EnumPopup(resolver.onlyRange, GUILayout.MaxWidth(onlyWidth)));

					switch (resolver.onlyRange)
					{
						case UseOnFloorResolver.FloorsRange.First:
							SetValue(ref resolver.onlyFirstRange, IntField("", resolver.onlyFirstRange));
							EndHorizontal();
							break;

						case UseOnFloorResolver.FloorsRange.Last:
							SetValue(ref resolver.onlyLastRange, IntField("", resolver.onlyLastRange));
							EndHorizontal();
							break;

						case UseOnFloorResolver.FloorsRange.FirstAndLast:
							EndHorizontal();
							
							BeginHorizontal();
							LabelField("  ",    GUILayout.Width(105));
							LabelField("First", GUILayout.Width(45));
							SetValue(ref resolver.onlyFirstRange, IntField(resolver.onlyFirstRange));
							EndHorizontal();
							
							BeginHorizontal();
							LabelField("  ",   GUILayout.Width(105));
							LabelField("Last", GUILayout.Width(45));
							SetValue(ref resolver.onlyLastRange, IntField(resolver.onlyLastRange));
							EndHorizontal();
							break;
					}

					break;
			
	        }
	        
		}
		
		
	}
}