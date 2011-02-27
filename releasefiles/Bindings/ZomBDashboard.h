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
#ifndef __ZomB_H__
#define __ZomB_H__

#include "Dashboard.h"
#include "DriverStation.h"
#include "Timer.h"
#include "Synchronized.h"
#include "Vision/AxisCamera.h"
#include <sstream>
#include <queue>
#include <map>
#include <utility>
#include <sockLib.h> 
#include <inetLib.h>
#include <netinet/ip.h>
//#define ZBDEBUG
#ifdef ZBDEBUG
#define debugf(str,args...) printf(str, ##args)
#else
#define debugf(str,args...)
#endif
using std::string;
using std::vector;
using std::queue;
using std::stringstream;
using std::map;
using std::pair;
typedef unsigned int uint;

namespace System451
{
    namespace Communication
    {
        namespace Dashboard
        {
            class IZomBSender
            {
                public:
                    virtual bool Add(const char* name, const char* value)=0;
                    virtual bool Send()=0;
                    virtual bool IsConnected() const=0;
                    virtual size_t GetAvalibleSpace() const=0;
                    virtual float GetSendingHertz() const=0;
                    virtual ~IZomBSender()
                    {
                    }
            };
            const int InfiniteSpace=-1;
            
            /**
             * Pack data into the "user data" field that gets sent to the dashboard laptop
             * via the driver station.
             */
            class ZomBDashboardPacketSender : public IZomBSender
            {
                    //We need to get around the namespace!
                    ::Dashboard &hp;
                    string prints;

                public:
                    ZomBDashboardPacketSender();
                    ~ZomBDashboardPacketSender();

                    virtual bool Add(const char* name, const char* value);

                    virtual bool Send();

                    virtual bool IsConnected() const
                    {
                        return true;
                    }
                    
                    virtual size_t GetAvalibleSpace() const;

                    virtual float GetSendingHertz() const
                    {
                        return 50.0;
                    }
                private:
                    
                    static const INT32 kMaxDashboardDataSize= USER_STATUS_DATA_SIZE - sizeof(UINT32) * 3 - sizeof(UINT8); // 13 bytes needed for 3 size parameters and the sequence number
                    static const char* ZomBStart;
                    static const char* ZomBEnd;
                protected:
                    const string esc(const char* val) const;

            };
            /**
             * Pack data into a tcp stream that gets sent to the dashboard
             */
            class ZomBTCPSender : public IZomBSender
            {
                    queue<pair<string, string> > nvpairs;
                    Task t;
                    Task topkil;
                    int tcp;
                    int zcp;
                    bool connected;
                    bool running;
                    bool usep1180;

                public:
                    ZomBTCPSender(bool use1180=false);
                    virtual ~ZomBTCPSender();

                    virtual bool Add(const char* name, const char* value);

                    virtual bool Send();

                    virtual bool IsConnected() const;

                    virtual size_t GetAvalibleSpace() const
                    {
                        return InfiniteSpace;//TODO: test to make sure -1 passes through
                    }
                    
                    virtual float GetSendingHertz() const
                    {
                        return 750.0;//assume 1.5ms per packet
                    }
                private:
                    static void runner(ZomBTCPSender* ths);
                    void sendrunner();
                    static void reader(ZomBTCPSender* ths);
            };
            
            class IZomBSource
            {
                public:
                    virtual string Get(string name)=0;
                    virtual bool IsConnected() const=0;
                    virtual ~IZomBSource()
                    {
                    }
            };
            
            /*
             * Reads data from a remote ZomB server. Useful for debug values
             */
            class ZomBTCPSource : public IZomBSource
            {
                    map<string, const char*> curvalues;
                    Task t;
                    int tcp;
                    bool connected;
                    bool running;
                    SEM_ID sink;
                    string rip;//remote ip, rest in peace

                public:
                    ZomBTCPSource(string ip);
                    virtual ~ZomBTCPSource();

                    virtual string Get(string name);

                    virtual bool IsConnected() const
                    {
                        return connected;
                    }
                private:
                    int readByte();
                    char* readBytes(int number);
                    void sendreader();
                    static void reader(ZomBTCPSource* ths);
            };
        }
    }
}
enum ZomBModes
{
    DBPacket=0x01,
    TCP=0x02,
    Both=0x03,
    RemoteData=0x10,
    AllTCP=0x12,
    All=0xFF
};
class ZomBDashboard
{
    private:
        double lasttime;
        float currentPain;//AKA CurrentHertz
        string remoteIP;
        vector<System451::Communication::Dashboard::IZomBSender*> sources;

        System451::Communication::Dashboard::IZomBSource* src;

        static ZomBDashboard *instance;

        ZomBDashboard(ZomBModes mode, bool use1180, string ip);

    public:
        ZomBDashboard(const ZomBDashboard& ref);
        void operator=(const ZomBDashboard& ref);

        virtual ~ZomBDashboard();
        static ZomBDashboard &GetInstance(ZomBModes mode=DBPacket, bool use1180=false, string ip="10.4.51.5");

        bool Add(string name, string value);
        bool Add(string name, int value);
        bool Add(string name, float value);
        bool Add(string name, double value);
        bool Add(string name, ParticleAnalysisReport value);
        bool Add(const char* name, int value);
        bool Add(const char* name, float value);
        bool Add(const char* name, double value);
        bool Add(const char* name, const char* value);
        bool Add(const char* name, ParticleAnalysisReport value);

        bool AddDebugVariable(const char* name, const char* value);
        bool AddDebugVariable(string name, string value);
        bool AddDebugVariable(string name, int value);
        bool AddDebugVariable(string name, float value);
        bool AddDebugVariable(string name, double value);
        bool AddDebugVariable(const char* name, int value);
        bool AddDebugVariable(const char* name, float value);
        bool AddDebugVariable(const char* name, double value);

        //AddDebugVariable renamed for convenience
        bool var(const char* name, const char* value);
        bool var(string name, string value);
        bool var(string name, int value);
        bool var(string name, float value);
        bool var(string name, double value);
        bool var(const char* name, int value);
        bool var(const char* name, float value);
        bool var(const char* name, double value);

        string GetString(string name);
        int GetInt(string name);
        float GetFloat(string name);
        double GetDouble(string name);

        bool HasSpace() const;
        size_t GetAvalibleSpace() const;
        bool IsConnected() const;
        bool IsAnyConnected() const;
        bool CanSend() const;
        bool Send();
        void ResetCounter();

};

extern "C"
{
    void ZomBDashboardInit(ZomBModes mode, char* ip, bool useport1180);
    int ZomBDashboardAdd(const char* name, const char* value);
    int ZomBDashboardAddTarget(const char* name, ParticleAnalysisReport value);
    int ZomBDashboardAddDebugVariable(const char* name, const char* value);
    const char* ZomBDashboardGetString(const char* name);
    void ZomBDashboardGetStringViaArg(const char* name, char* outValue);
    int ZomBDashboardGetInt(const char* name);
    float ZomBDashboardGetFloat(const char* name);
    double ZomBDashboardGetDouble(const char* name);
    int ZomBDashboardHasSpace();
    int ZomBDashboardIsConnected();
    int ZomBDashboardIsAnyConnected();
    int ZomBDashboardCanSend();
    int ZomBDashboardSend();
    void ZomBDashboardResetCounter();
}

#endif

