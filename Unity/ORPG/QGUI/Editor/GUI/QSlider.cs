﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QGUI
{
    public class QSlider : QWidget, IWidget
    {
        protected float m_Value;
        protected float m_Min;
        protected float m_Max;
        
        public float Value { get { return m_Value; } set { m_Value = value; } }
        public float Min { get { return m_Min; }set { m_Min = value; } }
        public float Max { get { return m_Max; }set { m_Max = value; } }

        public QSlider(EditorWindow window, IObject parent = null) : base(window, parent)
        {
        }

        public QSlider(EditorWindow window, string name, IObject parent = null) : base(window, name, parent)
        {
        }

        protected override void PaintEvent(Event current, Rect rect)
        {
            m_Value = EditorGUI.Slider(rect, m_Name, m_Value, m_Min, m_Max);
        }

        public override IObject Clone()
        {
            QSlider clone = base.Clone() as QSlider;
            clone.m_Value = m_Value;
            clone.m_Min = m_Min;
            clone.m_Max = m_Max;
            return clone;
        }
    }

    public static partial class QWidgetTo
    {
        public static QSlider ToSlider(this IWidget widget)
        {
            if (widget is QSlider)
                return widget as QSlider;
            return null;
        }
    }
}