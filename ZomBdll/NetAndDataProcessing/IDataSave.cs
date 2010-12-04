/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2009-2010, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
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
using System;
using System.ComponentModel;

namespace System451.Communication.Dashboard
{
    //TODO: Savable has no benifits from inheritence YET
    public interface ISavableZomBData
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
        void Add(ISavableZomBData DataSource);
        void StartSave(string file);
        void EndSave();
    }
}
