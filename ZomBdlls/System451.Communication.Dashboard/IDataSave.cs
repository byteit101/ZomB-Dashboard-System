/*
 * Copyright (c) 2009-2010, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
 * 
 * Permission to use, copy, modify, and distribute this software, its source, and its documentation
 * for any purpose, without fee, and without a written agreement is hereby granted, 
 * provided this paragraph and the following paragraph appear in all copies, and all
 * software that uses this code is released under this license. All projects that use
 * this code MUST release their source without fee.
 * 
 * THIS SOFTWARE IS PROVIDED "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
 * AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
 * Patrick Plenefisch OR FIRST Robotics Team 451 "The Cat Attack" BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.ComponentModel;

namespace System451.Communication.Dashboard
{
    //TODO: Savable has no benifits from inheritence YET
    public interface ISavableZomBData : IDashboardControl
    {
        /// <summary>
        /// Gets the TypeConverter for this data type
        /// </summary>
        /// <returns>Appropriate TypeConverter</returns>
        TypeConverter GetTypeConverter();
        /// <summary>
        /// Gets the Value of the data as a string
        /// </summary>
        string DataValue { get; }
        /// <summary>
        /// Notifies the Saver when new data is present
        /// This should call a fast enqueue function to the string
        /// </summary>
        event EventHandler DataUpdated;
    }

    public interface IZomBDataSaver
    {
        bool PrefixBindings { get; set; }
        void Add(ISavableZomBData DataSource);
        void StartSave(string file);
        void StartSave();
        void EndSave();
    }
}
