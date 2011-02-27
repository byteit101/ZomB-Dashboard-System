/*
* ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
* Copyright (C) 2011, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
* 
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
* 
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
* 
* You should have received a copy of the GNU General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
#include "ZDashboard.h"
using namespace std;

ZomBDashboard* ZomBDashboard::instance=NULL;

ZomBDashboard& ZomBDashboard::GetInstance(ZomBModes mode, bool use1180, string ip)
{
    if (instance == NULL)
    {
        instance = new ZomBDashboard(mode, use1180, ip);
    }
    return *instance;
}

ZomBDashboard::ZomBDashboard(ZomBModes mode, bool use1180, string ip) :
    lasttime(GetClock()), currentPain(50), remoteIP(ip), sources()
{
    if ((mode & DBPacket) == DBPacket)
        sources.push_back(new System451::Communication::Dashboard::ZomBDashboardPacketSender());
    if ((mode & TCP) == TCP)
        sources.push_back(new System451::Communication::Dashboard::ZomBTCPSender(use1180));
    if ((mode & RemoteData) == RemoteData)
        src = new System451::Communication::Dashboard::ZomBTCPSource(ip);
    else
        src = NULL;
    
    //Update Hz
    for (uint i = 0; i < this->sources.size(); i++)
    {
        float t = this->sources[i]->GetSendingHertz();
        currentPain=(currentPain>t ? currentPain : t);//Max
    }
    debugf("constructed\n");
}
ZomBDashboard::ZomBDashboard(const ZomBDashboard& ref) :
    lasttime(ref.lasttime), currentPain(ref.currentPain), sources(ref.sources), src(ref.src)
{
    debugf("transfering\n");
}
void ZomBDashboard::operator=(const ZomBDashboard& ref)
{
    this->lasttime=ref.lasttime;
    this->currentPain=ref.currentPain;
    this->sources=ref.sources;
    this->src=ref.src;
}
ZomBDashboard::~ZomBDashboard()
{
    debugf("Destroying...\n");
    for (uint i = 0; i < this->sources.size(); i++)
    {
        delete this->sources[i];
    }
}

bool ZomBDashboard::Add(string name, string value)
{
    return this->Add(name.c_str(), value.c_str());
}
bool ZomBDashboard::Add(string name, int value)
{
    stringstream out;
    out << value;
    return this->Add(name.c_str(), out.str().c_str());
}
bool ZomBDashboard::Add(string name, float value)
{
    stringstream out;
    out << value;
    return this->Add(name.c_str(), out.str().c_str());
}
bool ZomBDashboard::Add(string name, double value)
{
    stringstream out;
    out << value;
    return this->Add(name.c_str(), out.str().c_str());
}

bool ZomBDashboard::Add(string name, ParticleAnalysisReport value)
{
    double Height = value.imageHeight;
    double Width = value.imageWidth;
                
    stringstream out;
    out<<(value.boundingRect.width / Width)<<"x"<<(value.boundingRect.height / Height)<<"+"<<(value.boundingRect.left / Width)<<","<<(value.boundingRect.top / Height);
    return this->Add(name.c_str(), out.str().c_str());
}
bool ZomBDashboard::Add(const char* name, int value)
{
    stringstream out;
    out << value;
    return this->Add(name, out.str().c_str());
}
bool ZomBDashboard::Add(const char* name, float value)
{
    stringstream out;
    out << value;
    return this->Add(name, out.str().c_str());
}
bool ZomBDashboard::Add(const char* name, double value)
{
    stringstream out;
    out << value;
    return this->Add(name, out.str().c_str());
}
bool ZomBDashboard::Add(const char* name, ParticleAnalysisReport value)
{
    double Height = value.imageHeight;
    double Width = value.imageWidth;
                
    stringstream out;
    out<<(value.boundingRect.width / Width)<<"x"<<(value.boundingRect.height / Height)<<"+"<<(value.boundingRect.left / Width)<<","<<(value.boundingRect.top / Height);
    return this->Add(name, out.str().c_str());
}

bool ZomBDashboard::AddDebugVariable(string name, string value)
{
    return this->AddDebugVariable(name.c_str(), value.c_str());
}
bool ZomBDashboard::AddDebugVariable(string name, int value)
{
    stringstream out;
    out << value;
    return this->AddDebugVariable(name.c_str(), out.str().c_str());
}
bool ZomBDashboard::AddDebugVariable(string name, float value)
{
    stringstream out;
    out << value;
    return this->AddDebugVariable(name.c_str(), out.str().c_str());
}
bool ZomBDashboard::AddDebugVariable(string name, double value)
{
    stringstream out;
    out << value;
    return this->AddDebugVariable(name.c_str(), out.str().c_str());
}
bool ZomBDashboard::AddDebugVariable(const char* name, int value)
{
    stringstream out;
    out << value;
    return this->AddDebugVariable(name, out.str().c_str());
}
bool ZomBDashboard::AddDebugVariable(const char* name, float value)
{
    stringstream out;
    out << value;
    return this->AddDebugVariable(name, out.str().c_str());
}
bool ZomBDashboard::AddDebugVariable(const char* name, double value)
{
    stringstream out;
    out << value;
    return this->AddDebugVariable(name, out.str().c_str());
}

//AddDebugVariable renamed for convenience
bool ZomBDashboard::var(const char* name, const char* value)
{
    return this->AddDebugVariable(name, value);
}
bool ZomBDashboard::var(string name, string value)
{
    return this->AddDebugVariable(name, value);
}
bool ZomBDashboard::var(string name, int value)
{
    return this->AddDebugVariable(name, value);
}
bool ZomBDashboard::var(string name, float value)
{
    return this->AddDebugVariable(name, value);
}
bool ZomBDashboard::var(string name, double value)
{
    return this->AddDebugVariable(name, value);
}
bool ZomBDashboard::var(const char* name, int value)
{
    return this->AddDebugVariable(name, value);
}
bool ZomBDashboard::var(const char* name, float value)
{
    return this->AddDebugVariable(name, value);
}
bool ZomBDashboard::var(const char* name, double value)
{
    return this->AddDebugVariable(name, value);
}
bool ZomBDashboard::AddDebugVariable(const char* name, const char* value)
{
    string fv="dbg-";
    fv+=name;
    return this->Add(fv.c_str(), value);
}
string ZomBDashboard::GetString(string name)
{
    if (src != NULL)
    {
        return src->Get(name);
    }
    return "";
}
int ZomBDashboard::GetInt(string name)
{
    if (src != NULL)
    {
        stringstream ss(src->Get(name));
        int r;
        ss>>r;
        return r;
    }
    return 0;
}
float ZomBDashboard::GetFloat(string name)
{
    if (src != NULL)
    {
        stringstream ss(src->Get(name));
        float r;
        ss>>r;
        return r;
    }
    return 0.0f;
}
double ZomBDashboard::GetDouble(string name)
{
    if (src != NULL)
    {
        stringstream ss(src->Get(name));
        double r;
        ss>>r;
        return r;
    }
    return 0.0;
}
bool ZomBDashboard::Add(const char* name, const char* value)
{
    bool r=true;
    for (uint i = 0; i < this->sources.size(); i++)
    {
        r=r&&this->sources[i]->Add(name, value);
    }
    return r;
}
bool ZomBDashboard::Send()
{
    bool r=true;
    for (uint i = 0; i < this->sources.size(); i++)
    {
        r=r&&this->sources[i]->Send();
    }
    this->ResetCounter();
    return r;
}
bool ZomBDashboard::HasSpace() const
{
    //the smallest one
    bool r=true;
    for (uint i = 0; i < this->sources.size(); i++)
    {
        int t = this->sources[i]->GetAvalibleSpace();
        r=r&&(t > 1 || t == System451::Communication::Dashboard::InfiniteSpace);
    }
    return r;
}
size_t ZomBDashboard::GetAvalibleSpace() const
{
    //Gets the smallest amount of space available
    int r=System451::Communication::Dashboard::InfiniteSpace;
    for (uint i = 0; i < this->sources.size(); i++)
    {
        int t = this->sources[i]->GetAvalibleSpace();
        if (t!=System451::Communication::Dashboard::InfiniteSpace)
        {
            if (r==System451::Communication::Dashboard::InfiniteSpace||r>t)
                r=t;
        }
    }
    return r;
}
bool ZomBDashboard::IsConnected() const
{
    bool r=true;
    for (uint i = 0; i < this->sources.size(); i++)
    {
        r=r&&this->sources[i]->IsConnected();
    }
    return r;
}
bool ZomBDashboard::IsAnyConnected() const
{
    bool r=false;
    for (uint i = 0; i < this->sources.size(); i++)
    {
        r=r||this->sources[i]->IsConnected();
    }
    return r;
}
bool ZomBDashboard::CanSend() const
{
    return (this->lasttime+(1/this->currentPain)<=GetClock()) && this->IsAnyConnected();
}
void ZomBDashboard::ResetCounter()
{
    this->lasttime=GetClock();
}

///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////


const char* System451::Communication::Dashboard::ZomBDashboardPacketSender::ZomBStart="@@ZomB:|";
const char* System451::Communication::Dashboard::ZomBDashboardPacketSender::ZomBEnd=":ZomB@@";

System451::Communication::Dashboard::ZomBDashboardPacketSender::ZomBDashboardPacketSender() :
    hp(DriverStation::GetInstance()->GetHighPriorityDashboardPacker()), prints("@@ZomB:|")
{
    
}
System451::Communication::Dashboard::ZomBDashboardPacketSender::~ZomBDashboardPacketSender()
{
    
}

bool System451::Communication::Dashboard::ZomBDashboardPacketSender::Add(const char* name, const char* value)
{
    debugf("adding %s=%s\n", name, value);
    string s = esc(name);
    s+="=";
    s+=esc(value);
    s+="|";
    if (s.length()<=GetAvalibleSpace())
    {
        prints+=s;
        return true;
    }
    return false;
}
bool System451::Communication::Dashboard::ZomBDashboardPacketSender::Send()
{
    debugf("sending %s\n", prints.c_str());
    hp.Printf("%s", (prints+ZomBEnd).c_str());
    prints = ZomBStart;
    return true;
}
size_t System451::Communication::Dashboard::ZomBDashboardPacketSender::GetAvalibleSpace() const
{
    return (kMaxDashboardDataSize-prints.length())-10;//for end and 2 bytes
}

const string System451::Communication::Dashboard::ZomBDashboardPacketSender::esc(const char* val) const
{
    //there are 4 escape codes: %p produces a pipe %c produces a colon %e produces an equals sign and %% produces an % sign. 
    string s = val;
    for (uint i = 0; i < s.length(); i++)
    {
        if (s[i]=='%')
        {
            s.replace(i, 1, "%%");
            i++;
        }
    }
    for (uint i = 0; i < s.length(); i++)
    {
        if (s[i]=='=')
        {
            s.replace(i, 1, "%e");
            i++;
        }
    }
    for (uint i = 0; i < s.length(); i++)
    {
        if (s[i]==':')
        {
            s.replace(i, 1, "%c");
            i++;
        }
    }
    for (uint i = 0; i < s.length(); i++)
    {
        if (s[i]=='|')
        {
            s.replace(i, 1, "%p");
            i++;
        }
    }
    return s;
}

///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////


System451::Communication::Dashboard::ZomBTCPSender::ZomBTCPSender(bool use1180) :
    nvpairs(), t("ZBTCPSender", (FUNCPTR)runner), topkil("ZBTCPTKiller", (FUNCPTR)reader)
{
    tcp=0;
    zcp=0;
    usep1180=use1180;
    connected=false;
    running=true;
    t.Start((int)this);
}
System451::Communication::Dashboard::ZomBTCPSender::~ZomBTCPSender()
{
    running=false;
    connected=false;
    t.Stop();
    topkil.Stop();
    
    if (tcp!=0)
        close(tcp);
    if (zcp!=0)
        close(zcp);
}

bool System451::Communication::Dashboard::ZomBTCPSender::Add(const char* name, const char* value)
{
    nvpairs.push(make_pair<string, string>(name, value));
    return true;
}
bool System451::Communication::Dashboard::ZomBTCPSender::Send()
{
    nvpairs.push(make_pair<string, string>("\x03", ""));//special value
    return true;
}
bool System451::Communication::Dashboard::ZomBTCPSender::IsConnected() const
{
    return connected;
}
void System451::Communication::Dashboard::ZomBTCPSender::runner(ZomBTCPSender* ths)
{
    ths->connected=false;
    sockaddr_in sa;
    const int saln= sizeof(sockaddr_in);
    bzero((char *) &sa, saln);
    sa.sin_family = AF_INET;
    sa.sin_len = (u_char) saln;
    sa.sin_port = htons(ths->usep1180?1180:9066);
    sa.sin_addr.s_addr = htonl(INADDR_ANY);
    
    while (ths->running)
    {
        //Create
        while ((ths->tcp = socket(AF_INET, SOCK_STREAM, 0)) == ERROR)
        {
            debugf("ZomB socket err\r\n");
        }
        
        // Set the TCP socket so that it can be reused if it is in the wait state.
        int reuseAddr = 1;
        setsockopt(ths->tcp, SOL_SOCKET, SO_REUSEADDR, reinterpret_cast<char*>(&reuseAddr), sizeof(reuseAddr));
        
        //Set the TCP socket so that it is as fast as possible
        //reuseAddr = IPTOS_LOWDELAY;
        reuseAddr=IPTOS_THROUGHPUT;
        setsockopt(ths->tcp, IPPROTO_IP, IP_TOS, reinterpret_cast<char*>(&reuseAddr), sizeof(reuseAddr));
        
        //Bind
        if (bind(ths->tcp, (sockaddr*)&sa, saln)== ERROR)
        {
            debugf("ZomB socket bind err\r\n");
            goto CloseSocket;
        }
        
        //Listen
        if (listen(ths->tcp, 1)!= OK)
        {
            debugf("ZomB socket listen err\r\n");
            goto CloseSocket;
        }
        
        //Sender loop
        while (ths->running)
        {
            sockaddr_in ca;
            int caln;
            
            //accept
            if ((ths->zcp=accept(ths->tcp, reinterpret_cast<sockaddr*>(&ca), &caln))== ERROR)
            {
                debugf("ZomB socket accept err\r\n");
                goto CloseSocket;
            }
            
            //Explicit flush, go
            static const char header[2] = { 0x45, 0x00 };
            if (write(ths->zcp, const_cast<char*>(header), 2)!= ERROR)
            {
                ths->connected=true;
                
                //We are connected! Start sending data
                ths->topkil.Start((int)ths);
                ths->sendrunner();
                
                //exiting, clear nvpairs or we may get a segfault
                while (!ths->nvpairs.empty())
                    ths->nvpairs.pop();
            }
            
            ths->connected=false;
            close(ths->zcp);
            debugf("ZomB zcp socket deleted\r\n");
        }
        
        CloseSocket:
        close(ths->tcp);
        debugf("ZomB socket deleted\r\n");
        Wait(0.1);//Don't kill
    }
}
void System451::Communication::Dashboard::ZomBTCPSender::sendrunner()
{
    while (running)
    {
        while (running && nvpairs.empty())
        {
            Wait(0.002);//2MS
        }
        while (running && (!nvpairs.empty()))
        {
            pair<string, string> cur = nvpairs.front();
            if (cur.first == "\x03")
            {
                static const char sender=0x00;
                if (write(zcp, const_cast<char*>(&sender), 1)<0)
                {
                    return;
                }
            }
            else
            {
                char* nl = new char[2];
                nl[0]=(char)cur.first.length();
                nl[1]=0;
                char* vl = new char[2];
                vl[0]=(char)cur.second.length();
                vl[1]=0;
                if (write(zcp, nl, 1)<0)
                {
                    return;
                }
                if (write(zcp, vl, 1)<0)
                {
                    return;
                }
                if (write(zcp, const_cast<char*>(cur.first.c_str()), cur.first.length())<0)
                {
                    return;
                }
                if (write(zcp, const_cast<char*>(cur.second.c_str()), cur.second.length())<0)
                {
                    return;
                }
                delete[] nl;
                delete[] vl;
            }
            nvpairs.pop();
        }
    }
}
void System451::Communication::Dashboard::ZomBTCPSender::reader(ZomBTCPSender* ths)
{
    int killproc=0;
    char read[1];
    
    while (ths->running)
    {
        recv(ths->zcp, read, 1, 0);
        if (read[0]==0x00)
            killproc++;
        else
            killproc=0;
        if (killproc>=3)
        {
            ths->running=false;
            return;
        }
    }
}

///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////


System451::Communication::Dashboard::ZomBTCPSource::ZomBTCPSource(string ip) :
    curvalues(), t("ZBTCPSrc", (FUNCPTR)reader), sink(semMCreate(SEM_Q_PRIORITY | SEM_DELETE_SAFE | SEM_INVERSION_SAFE)), rip(ip)
{
    tcp=0;
    connected=false;
    running=true;
    t.Start((int)this);
}
System451::Communication::Dashboard::ZomBTCPSource::~ZomBTCPSource()
{
    running=false;
    connected=false;
    t.Stop();
    
    if (tcp!=0)
        close(tcp);
    semDelete(sink);
}

string System451::Communication::Dashboard::ZomBTCPSource::Get(string name)
{
    Synchronized sync(sink);
    if (curvalues.find(name)!=curvalues.end())
        return curvalues[name];
    return "";
}
void System451::Communication::Dashboard::ZomBTCPSource::reader(ZomBTCPSource* ths)
{
    ths->connected=false;
    sockaddr_in sa;
    const int saln= sizeof(sockaddr_in);
    bzero((char *) &sa, saln);
    sa.sin_family = AF_INET;
    sa.sin_port = htons(9067);//TODO: this is hard coded
    inet_pton(AF_INET, ths->rip.c_str(), &sa.sin_addr);

    while (ths->running)
    {
        //Create
        while ((ths->tcp = socket(AF_INET, SOCK_STREAM, 0)) == ERROR)
        {
            debugf("ZomB socket err\r\n");
        }
        
        // Set the TCP socket so that it can be reused if it is in the wait state.
        int reuseAddr = 1;
        setsockopt(ths->tcp, SOL_SOCKET, SO_REUSEADDR, reinterpret_cast<char*>(&reuseAddr), sizeof(reuseAddr));
        
        //Set the TCP socket so that it is as fast as possible
        //reuseAddr = IPTOS_LOWDELAY;
        reuseAddr=IPTOS_THROUGHPUT;
        setsockopt(ths->tcp, IPPROTO_IP, IP_TOS, reinterpret_cast<char*>(&reuseAddr), sizeof(reuseAddr));
        
        //Bind
        if (connect(ths->tcp, (sockaddr*)&sa, saln)== ERROR)
        {
            debugf("ZomB (s) socket bind err\n");
            goto CloseSocket;
        }
        
        ths->connected=true;
        //We are connected! Start reading
        ths->sendreader();
        
        CloseSocket:
        close(ths->tcp);
        ths->connected=false;
        debugf("ZomBs deleted\n");
        Wait(0.1);//Don't kill
    }
}
void System451::Communication::Dashboard::ZomBTCPSource::sendreader()
{
    bool implicitSend = false;
    bool longNames = false;
    int last;
    do
    {
        last = readByte();
        switch (last)
        {
            
            case 0x45:
                implicitSend = false;
                break;
            case 0x49:
                implicitSend = true;
                break;
            case 0x4C:
                longNames = true;
                break;
            case -1:
                return;
            default:
                break;
        }
    }
    while (last != 0x00);
    //end config
    debugf("Reading TCP!\n");
    int namel, valuel;
    while (running)
    {
        last = readByte();
        
        if (last == -1)
            return;//stream closed
        if (last == 0)
            continue;
        
        //last byte was name lenght
        namel = last;
        
        //Read the value
        if (longNames&&false)//TOOD: remove this
        {
            valuel = (readByte() << 8) + readByte();//TODO: test
        }
        else
        {
            valuel = readByte();
        }
        if (valuel < 0)
            return;//stream closed

        //Make the buffers
        char* buf = readBytes(namel);
        if (buf == 0)//closed!
            return;
        char* vbuf = readBytes(valuel);
        if (vbuf == 0)//closed!
            return;
        
        //Add the value
        {
            Synchronized sync(sink);
            string strbuf = string (buf);
            if (curvalues.find(strbuf) == curvalues.end())//before
                delete [] (curvalues[strbuf]);//delete
            curvalues[strbuf] = const_cast<const char*>(vbuf);
        }
    }
}
int System451::Communication::Dashboard::ZomBTCPSource::readByte()
{
    try
    {
        char* rs = this->readBytes(1);
        char r = rs[0];
        delete[] rs;
        return (int)r;
    }
    catch(std::exception e)
    {
        return -1;
    }
    catch(...)
    {
        return -1;
    }
}
char* System451::Communication::Dashboard::ZomBTCPSource::readBytes(int number)
{
    if (number < 1)
        return 0;
    char* buf = new char[number+1];
    bzero(buf, number+1);
    int num = recv(tcp, buf, number, 0);
    if (num == ERROR || num == 0)
    {
        debugf("AHH! Recieve failed\r\n");
        if (number == 1)
            throw std::exception();
        return 0;
    }
    while (num < number)
    {
        debugf("Receiving... (retry %d of %d)\n", num, number);
        char* buf2 = new char[number-num];
        int num2 = recv(tcp, buf2, number-num, 0);
        if (num2 == ERROR || num2 == 0)
        {
            debugf("AHH! Recieve failed\r\n");
            if (number == 1)
                throw std::exception();
            return 0;
        }
        memcpy((buf)+num, buf2, num2);
        num+=num2;
        delete buf2;
    }
    return buf;
}

///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////

void ZomBDashboardInit(ZomBModes mode, char* ip, bool useport1180)
{
    ZomBDashboard::GetInstance(mode, useport1180, string (ip));
}
int ZomBDashboardAdd(const char* name, const char* value)
{
    return !ZomBDashboard::GetInstance().Add(name, value);
}
int ZomBDashboardAddDebugVariable(const char* name, const char* value)
{
    return !ZomBDashboard::GetInstance().AddDebugVariable(name, value);
}
const char* ZomBDashboardGetString(const char* name)
{
    return ZomBDashboard::GetInstance().GetString(string(name)).c_str();
}
void ZomBDashboardGetStringViaArg(const char* name, char* outValue)
{
    strcpy(outValue, ZomBDashboard::GetInstance().GetString(string(name)).c_str());
}
int ZomBDashboardGetInt(const char* name)
{
    return ZomBDashboard::GetInstance().GetInt(string(name));
}
float ZomBDashboardGetFloat(const char* name)
{
    return ZomBDashboard::GetInstance().GetFloat(string(name));
}
double ZomBDashboardGetDouble(const char* name)
{
    return ZomBDashboard::GetInstance().GetDouble(string(name));
}
int ZomBDashboardHasSpace()
{
    return ZomBDashboard::GetInstance().HasSpace();
}
int ZomBDashboardIsConnected()
{
    return ZomBDashboard::GetInstance().IsConnected();
}
int ZomBDashboardIsAnyConnected()
{
    return ZomBDashboard::GetInstance().IsAnyConnected();
}
int ZomBDashboardCanSend()
{
    return ZomBDashboard::GetInstance().CanSend();
}
int ZomBDashboardSend()
{
    return !ZomBDashboard::GetInstance().Send();
}
void ZomBDashboardResetCounter()
{
    ZomBDashboard::GetInstance().ResetCounter();
}
