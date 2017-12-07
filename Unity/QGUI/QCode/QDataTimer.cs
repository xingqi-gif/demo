﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class QDataTimer
{
    private int hour;
    private int minute;
    private int seconds;
    private int milliseconds;
    private static Regex regex = new Regex("(\\d{2}:){3}\\d+");
    
    public int Hour
    {
        get { return hour; }
        set { hour = value % 24; }
    }

    public int Minute
    {
        get { return minute; }
        set { minute = value % 60; }
    }

    public int Seconds
    {
        get { return seconds; }
        set { seconds = value % 60; }
    }

    public int Milliseconds
    {
        get { return milliseconds; }
        set { milliseconds = value % 10000000; }
    }

    public QDataTimer()
    {
        Hour = Minute = Seconds = Milliseconds = 0;
    }

    public QDataTimer(string timer)
    {
        SetHMS(timer);
    }

    public QDataTimer(int h, int m, int s, int ms=0)
    {
        SetHMS(h, m, s, ms);
    }

    public void SetHMS(int h, int m, int s, int ms = 0)
    {
        Hour = h;
        Minute = m;
        Seconds = s;
        Milliseconds = ms;
    }

    private static string[] tmp;
    public void SetHMS(string timer)
    {
        if (!regex.IsMatch(timer))
        {
            Hour = Minute = Seconds = Milliseconds = 0;
            return;
        }

        tmp = timer.Split(':');
        Hour = int.Parse(tmp[0]);
        Minute = int.Parse(tmp[1]);
        Seconds = int.Parse(tmp[2]);

        if (tmp[3].Length > 8)
            tmp[3] = tmp[3].Remove(8);
        Milliseconds = int.Parse(tmp[3]);
    }

    public new string ToString()
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}:{3}", hour, minute, seconds, milliseconds);
    }
}