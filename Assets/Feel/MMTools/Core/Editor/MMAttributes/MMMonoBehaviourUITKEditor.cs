using System;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

namespace MoreMountains.Tools
{
	public class MMInspectorGroupData
	{
		public bool GroupIsOpen;
		public MMInspectorGroupAttribute GroupAttribute;
		public List<SerializedProperty> PropertiesList = new List<SerializedProperty>();
		public HashSet<string> GroupHashSet = new HashSet<string>();
		public Color GroupColor;

		public void ClearGroup()
		{
			GroupAttribute = null;
			GroupHashSet.Clear();
			PropertiesList.Clear();
		}
	}
	
	[CanEditMultipleObjects]
	[CustomEditor(typeof(MMMonoBehaviour), true)] 
	public class MMMonoBehaviourUITKEditor : Editor
	{
		public StyleSheet EditorStyleSheet;
		
		public bool DrawerInitialized;
		public Dictionary<string, MMInspectorGroupData> GroupData ;
		public List<SerializedProperty> PropertiesList;
		private bool _requiresConstantRepaint;
		private bool _requiresConstantRepaintOnlyWhenPlaying;
		private MMMonoBehaviour _targetMonoBehaviourGameObject;
		private bool _targetMonoBehaviourIsNotNull;

		public override bool RequiresConstantRepaint()
		{
			if (_requiresConstantRepaintOnlyWhenPlaying)
			{
				return Application.isPlaying && _targetMonoBehaviourIsNotNull && _targetMonoBehaviourGameObject.enabled;
			}
			else
			{
				return _requiresConstantRepaint;
			}
		}
		
		private string[] _mmHiddenPropertiesToHide;
		private bool _hasMMHiddenProperties = false;

		protected virtual void Initialization()
		{
			if (DrawerInitialized && PropertiesList != null)
			{
				return;
			}

			GroupData = new Dictionary<string, MMInspectorGroupData>();
			PropertiesList = new List<SerializedProperty>();
			
			_targetMonoBehaviourGameObject = (MMMonoBehaviour)target;
			if (_targetMonoBehaviourGameObject != null)
			{
				_targetMonoBehaviourIsNotNull = true;
			}
			
			_requiresConstantRepaint = serializedObject.targetObject.GetType().GetCustomAttribute<MMRequiresConstantRepaintAttribute>() != null;
			_requiresConstantRepaintOnlyWhenPlaying = serializedObject.targetObject.GetType().GetCustomAttribute<MMRequiresConstantRepaintOnlyWhenPlayingAttribute>() != null;
			
			List<FieldInfo> fieldInfoList;
			MMInspectorGroupAttribute previousGroupAttribute = default;
			int fieldInfoLength = MMMonoBehaviourFieldInfo.GetFieldInfo(target, out fieldInfoList);

			for (int i = 0; i < fieldInfoLength; i++)
			{
				MMInspectorGroupAttribute group = Attribute.GetCustomAttribute(fieldInfoList[i], typeof(MMInspectorGroupAttribute)) as MMInspectorGroupAttribute;
				MMInspectorGroupData groupData;
				if (group == null)
				{
					if (previousGroupAttribute != null && previousGroupAttribute.GroupAllFieldsUntilNextGroupAttribute)
					{
						if (!GroupData.TryGetValue(previousGroupAttribute.GroupName, out groupData))
						{
							GroupData.Add(previousGroupAttribute.GroupName, new MMInspectorGroupData
							{
								GroupAttribute = previousGroupAttribute,
								GroupHashSet = new HashSet<string> { fieldInfoList[i].Name },
								GroupColor = MMColors.GetColorAt(previousGroupAttribute.GroupColorIndex)
							});
						}
						else
						{
							groupData.GroupColor = MMColors.GetColorAt(previousGroupAttribute.GroupColorIndex);
							groupData.GroupHashSet.Add(fieldInfoList[i].Name);
						}
					}

					continue;
				}
                
				previousGroupAttribute = group;

				if (!GroupData.TryGetValue(group.GroupName, out groupData))
				{
					bool fallbackOpenState = true;
					if (group.ClosedByDefault) { fallbackOpenState = false; }
					bool groupIsOpen = EditorPrefs.GetBool(string.Format($"{group.GroupName}{fieldInfoList[i].Name}{target.GetInstanceID()}"), fallbackOpenState);
					GroupData.Add(group.GroupName, new MMInspectorGroupData
					{
						GroupAttribute = group,
						GroupColor = MMColors.GetColorAt(previousGroupAttribute.GroupColorIndex),
						GroupHashSet = new HashSet<string> { fieldInfoList[i].Name }, GroupIsOpen = groupIsOpen });
				}
				else
				{
					groupData.GroupHashSet.Add(fieldInfoList[i].Name);
					groupData.GroupColor = MMColors.GetColorAt(previousGroupAttribute.GroupColorIndex);
				}
			}

			SerializedProperty iterator = serializedObject.GetIterator();

			if (iterator.NextVisible(true))
			{
				do
				{
					FillPropertiesList(iterator);
				} while (iterator.NextVisible(false));
			}
			DrawerInitialized = true;
		}
		
		public void FillPropertiesList(SerializedProperty serializedProperty)
		{
			bool shouldClose = false;

			foreach (KeyValuePair<string, MMInspectorGroupData> pair in GroupData)
			{
				if (pair.Value.GroupHashSet.Contains(serializedProperty.name))
				{
					SerializedProperty property = serializedProperty.Copy();
					shouldClose = true;
					pair.Value.PropertiesList.Add(property);
					break;
				}
			}

			if (!shouldClose)
			{
				SerializedProperty property = serializedProperty.Copy();
				PropertiesList.Add(property);
			}
		}
		
		public override VisualElement CreateInspectorGUI()
		{
			Initialization();
			
			VisualElement root = new VisualElement();
			root.styleSheets.Add(EditorStyleSheet);
			
			// Draw the script field
			SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");
			PropertyField scriptField = new PropertyField(scriptProperty);
			scriptField.SetEnabled(false); 
			root.Add(scriptField);

			if (PropertiesList.Count == 0)
			{
				return root;
			}

			foreach (KeyValuePair<string, MMInspectorGroupData> pair in GroupData)
			{
				DrawGroup(pair.Value, root);
			}
			
			serializedObject.ApplyModifiedProperties();

			return root;
		}
		
		protected virtual void DrawGroup(MMInspectorGroupData groupData, VisualElement root)
		{
			Foldout foldout = new Foldout();
			foldout.text = groupData.GroupAttribute.GroupName;
			foldout.value = groupData.GroupIsOpen;
			foldout.AddToClassList("mm-foldout");
			foldout.style.borderLeftColor = groupData.GroupColor;
			foldout.viewDataKey = target.name + groupData.GroupAttribute.GroupName;
			root.Add(foldout);
			
			var toggleField = typeof(Foldout).GetField("m_Toggle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var toggleElement = (Toggle)toggleField.GetValue(foldout);
			toggleElement.AddToClassList("mm-foldout-toggle");
			
			for (int i = 0; i < groupData.PropertiesList.Count; i++)
			{
				DrawChild(i, foldout, root);
			}

			void DrawChild(int i, Foldout foldout, VisualElement root)
			{
				if ((_hasMMHiddenProperties) && (_mmHiddenPropertiesToHide.Contains(groupData.PropertiesList[i].name)))
				{
					return;
				}
				PropertyField field = new PropertyField(groupData.PropertiesList[i]);
				field.label = ObjectNames.NicifyVariableName(groupData.PropertiesList[i].name);
				field.tooltip = groupData.PropertiesList[i].tooltip;
				foldout.Add(field);
			}
		}
	}
}
