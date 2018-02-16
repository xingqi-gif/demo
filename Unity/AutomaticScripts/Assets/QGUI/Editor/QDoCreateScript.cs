﻿using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using System.Collections.Generic;


namespace QGUI
{
    public class QDoCreateScript : EndNameEditAction
    {
        private GameObject select;

        private readonly string fieldStr = "\t{0} {1} m_{2};\n";
        private readonly string awakeFind = "\t\tm_{0} = transform.Find(\"{1}\").GetComponent<{2}>();\n";
        private readonly string awakeEvent = "\t\tm_{0}.onValueChanged.AddListener(On{0}ValueChanged);\n";
        private readonly string eventFunction = "\n\tprivate void On{0}ValueChanged({1} value)\n\t{{\n\n\t}}\n";
        private readonly string buttonEvent = "\t\tm_{0}.onClick.AddListener(On{0}Click);\n";
        private readonly string buttonFunction = "\n\tprivate void On{0}Click()\n\t{{\n\n\t}}\n";

        private readonly static string[] types = new string[] {
            "InputField", "ScrollRect", "Dropdown", "Scrollbar", "Slider", "Toggle"
        };
        private readonly static string[] parameters = new string[] {
            "string", "Vector2", "int", "float", "float", "bool"
        };
        
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            if (QCreateScript.selectGameObject == null) return;

            select = QCreateScript.selectGameObject;

            var text = File.ReadAllText(resourceFile);
            text = Create(text, select.GetComponentsInChildren<QMarkupField>());

            var encoding = new UTF8Encoding(true, false);
            Debug.Log(pathName);
            pathName = pathName.Replace("_","/");
            Debug.Log(pathName+"-----");
            File.WriteAllText(pathName, text, encoding);

            AssetDatabase.ImportAsset(pathName);
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }


        private string Create(string text, QMarkupField[] objs)
        {
            var field = new StringBuilder();
            var awake = new StringBuilder();
            var awakeComponent = new StringBuilder();
            var function = new StringBuilder();

            foreach (var obj in objs)
            {
                var name = GetName(obj.name);

                field.AppendFormat(fieldStr, GetLower(obj.jurisdiction.ToString()), obj.component, name);

                if (obj.jurisdiction != QMarkupField.Jurisdiction.Public)
                {
                    awakeComponent.AppendFormat(awakeFind, name, RemoveZreo(GetFind(obj.transform, select.transform)), obj.component);
                }

                for (int i = 0; i < types.Length; i++)
                {
                    if (obj.component == types[i])
                    {

                        awake.AppendFormat(awakeEvent, name);
                        function.AppendFormat(eventFunction, name, parameters[i]);
                        break;
                    }
                }
                if (obj.component == "Button")
                {
                    awake.AppendFormat(buttonEvent, name);
                    function.AppendFormat(buttonFunction, name);
                }
            }

            if (select.layer == 5)
                return string.Format(text, GetUpper(select.name + "UI"), field, awakeComponent.Append(awake), function);
            else
                return string.Format(text, GetUpper(select.name), field, awakeComponent.Append(awake), function);
        }

        private string GetLower(string value)
        {
            return value.Substring(0, 1).ToLower() + value.Substring(1);
        }

        private string GetUpper(string value)
        {
            return value.Substring(0, 1).ToUpper() + value.Substring(1); ;
        }

        private List<string> names = new List<string>();
        private string GetName(string value)
        {
            var name = GetUpper(value);

            foreach (var n in names)
            {
                if (n == name)
                {
                    name += "_" + GetIndex(1, name);
                }
            }
            names.Add(name);

            return name;
        }

        private int GetIndex(int index, string value)
        {
            var tmp = value + "_" + index;
            foreach (var n in names)
            {
                if (n == tmp)
                {
                    index = GetIndex(++index, value);
                }
            }
            return index;
        }

        private string RemoveZreo(string value)
        {
            return value.Remove(0, 1);
        }

        private string GetFind(Transform target, Transform parent)
        {
            if (target == parent)
                return string.Empty;
            return GetFind(target.parent, parent) + "/" + target.name;
        }
    }
}