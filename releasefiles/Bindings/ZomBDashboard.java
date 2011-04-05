/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2011, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This file can be redistributed under the Lesser GNU General Public License found in REDIST LICENSE.txt
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
package org.thecatattack.System451.Communication.Dashboard;

import com.sun.cldc.jna.*;
import edu.wpi.first.wpilibj.Dashboard;
import edu.wpi.first.wpilibj.DriverStation;
import edu.wpi.first.wpilibj.Timer;
import edu.wpi.first.wpilibj.image.ParticleAnalysisReport;

public class ZomBDashboard
{
    private Dashboard db = null;
    private double lasttime = GetClock();
    private String prints = "@@ZomB:|";
    private static ZomBDashboard instance;
    private static final Function addfnc = NativeLibrary.getDefaultInstance().getFunction("ZomBDashboardAdd");
    private static final Function Hasspacefnc = NativeLibrary.getDefaultInstance().getFunction("ZomBDashboardHasSpace");
    private static final Function IsConnectedfnc = NativeLibrary.getDefaultInstance().getFunction("ZomBDashboardIsConnected");
    private static final Function IsanyConnectedfnc = NativeLibrary.getDefaultInstance().getFunction("ZomBDashboardIsAnyConnected");
    private static final Function cansendfnc = NativeLibrary.getDefaultInstance().getFunction("ZomBDashboardCanSend");
    private static final Function resetfnc = NativeLibrary.getDefaultInstance().getFunction("ZomBDashboardResetCounter");
    private static final Function sendfnc = NativeLibrary.getDefaultInstance().getFunction("ZomBDashboardSend");
    private static final Function getstringfnc = NativeLibrary.getDefaultInstance().getFunction("ZomBDashboardGetStringViaArg");

    private ZomBDashboard(ZomBModes mode, boolean use1180, String ip)
    {
        if (mode != ZomBModes.DBPacket)
            NativeLibrary.getDefaultInstance().getFunction("ZomBDashboardInit").call3(mode.V, Pointer.createStringBuffer(ip), use1180?1:0);
        else
            db = DriverStation.getInstance().getDashboardPackerHigh();
    }

    public static synchronized ZomBDashboard getInstance(ZomBModes mode, boolean use1180, String ip)
    {
        if (instance == null)
        {
            instance = new ZomBDashboard(mode, use1180, ip);
        }
        return instance;
    }

    public static synchronized ZomBDashboard getInstance(ZomBModes mode, String ip)
    {
        return getInstance(mode, false, ip);
    }

    public static synchronized ZomBDashboard getInstance(ZomBModes mode, boolean use1180)
    {
        return getInstance(mode, use1180, "10.0.0.5");
    }

    public static synchronized ZomBDashboard getInstance(ZomBModes mode)
    {
        return getInstance(mode, false, "10.0.0.5");
    }

    public static synchronized ZomBDashboard getInstance()
    {
        return getInstance(ZomBModes.DBPacket, false, "10.0.0.5");
    }

    //main
    public boolean add(String name, String value)
    {
        if (db != null)
        {
            String s= esc(name) + "=" + esc(value) + "|";
            if (s.length() <= getSpace())
            {
                prints+=s;
                return true;
            }
            return false;
        }
        else
            return addfnc.call2(Pointer.createStringBuffer(name), Pointer.createStringBuffer(value))==1?true:false;
    }

    public boolean add(String name, int value)
    {
        return add(name, String.valueOf(value));
    }

    public boolean add(String name, boolean value)
    {
        return add(name, String.valueOf(value ? 1 : 0));
    }

    public boolean add(String name, float value)
    {
        return add(name, String.valueOf(value));
    }

    public boolean add(String name, double value)
    {
        return add(name, String.valueOf(value));
    }

    public boolean add(String name, ParticleAnalysisReport value)
    {
        return add(name, String.valueOf(value.boundingRectWidth / (double) value.imageWidth) + "x"
                + String.valueOf(value.boundingRectHeight / (double) value.imageHeight) + "+"
                + String.valueOf(value.boundingRectLeft / (double) value.imageWidth) + ","
                + String.valueOf(value.boundingRectTop / (double) value.imageHeight));
    }

    private String esc(String name)
    {
        if (name.indexOf("%")!=-1)
        {
            String s = "";
            for (int i = 0; i < name.length(); i++)
            {
                char nxt = name.charAt(i);
                if (nxt == '%')
                    s+="%%";
                else
                    s+=nxt;
            }
            name = s;
        }
        if (name.indexOf("|")!=-1)
        {
            String s = "";
            for (int i = 0; i < name.length(); i++)
            {
                char nxt = name.charAt(i);
                if (nxt == '|')
                    s+="%p";
                else
                    s+=nxt;
            }
            name = s;
        }
        if (name.indexOf("=")!=-1)
        {
            String s = "";
            for (int i = 0; i < name.length(); i++)
            {
                char nxt = name.charAt(i);
                if (nxt == '=')
                    s+="%e";
                else
                    s+=nxt;
            }
            name = s;
        }
        if (name.indexOf(":")!=-1)
        {
            String s = "";
            for (int i = 0; i < name.length(); i++)
            {
                char nxt = name.charAt(i);
                if (nxt == ':')
                    s+="%c";
                else
                    s+=nxt;
            }
            name = s;
        }
        return name;
    }

    //main
    public boolean addDebugVariable(String name, String value)
    {
        return add("dbg-"+name, value);
    }

    public boolean addDebugVariable(String name, int value)
    {
        return addDebugVariable(name, String.valueOf(value));
    }

    public boolean addDebugVariable(String name, boolean value)
    {
        return addDebugVariable(name, String.valueOf(value ? 1 : 0));
    }

    public boolean addDebugVariable(String name, float value)
    {
        return addDebugVariable(name, String.valueOf(value));
    }

    public boolean addDebugVariable(String name, double value)
    {
        return addDebugVariable(name, String.valueOf(value));
    }

    public boolean var(String name, String value)
    {
        return addDebugVariable(name, value);
    }

    public boolean var(String name, int value)
    {
        return addDebugVariable(name, String.valueOf(value));
    }

    public boolean var(String name, boolean value)
    {
        return addDebugVariable(name, String.valueOf(value ? 1 : 0));
    }

    public boolean var(String name, float value)
    {
        return addDebugVariable(name, String.valueOf(value));
    }

    public boolean var(String name, double value)
    {
        return addDebugVariable(name, String.valueOf(value));
    }

    public String getString(String name)
    {
        Pointer ptr = Pointer.createStringBuffer(name);
        Pointer p = new Pointer(50);//50 should be enough for now
        getstringfnc.call2(ptr, p);
        String str = p.getString(0);
        ptr.free();
        return str;
    }

    public int getInt(String name)
    {
        try
        {
            return Integer.parseInt(getString(name));
        }
        catch (Throwable t) { }
        return 0;
    }

    public float getFloat(String name)
    {
        try
        {
            return Float.parseFloat(getString(name));
        }
        catch(Throwable t) { }
        return 0;
    }

    public double getDouble(String name)
    {
        try
        {
            return Double.parseDouble(getString(name));
        }
        catch(Throwable t) { }
        return 0;
    }

    public boolean hasSpace()
    {
        if (db != null)
            return DriverStation.USER_CONTROL_DATA_SIZE - prints.length() > 11;//11 for safety
        return Hasspacefnc.call0()==1?true:false;
    }

    private int getSpace()
    {
        return DriverStation.USER_CONTROL_DATA_SIZE - prints.length() - 11;
    }

    public boolean isConnected()
    {
        if (db != null)
            return true;
        return IsConnectedfnc.call0()==1?true:false;
    }

    public boolean isAnyConnected()
    {
        if (db != null)
            return true;
        return IsanyConnectedfnc.call0()==1?true:false;
    }

    public boolean canSend()
    {
        if (db != null)
            return (lasttime + 0.02 <= GetClock());
        return cansendfnc.call0()==1?true:false;
    }

    public void resetCounter()
    {
        lasttime=GetClock();
        if (db == null)
            resetfnc.call0();
    }

    public boolean send()
    {
        resetCounter();
        if (db != null)
        {
            db.addString(prints + ":ZomB@@");
            db.commit();
            prints = "@@ZomB:|";
            return true;
        }
        else
            return sendfnc.call0()==1?true:false;
    }

    protected double GetClock()
    {
        return Timer.getUsClock() / 1000000f;
    }
}
