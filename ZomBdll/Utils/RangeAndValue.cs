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

namespace System451.Communication.Dashboard.Utils
{
    public class RangeAndValue
    {
        private float min;
        private float max;
        private float valuei;
        private bool wrap;
        private bool oob;
        public delegate void Invalidater();
        public event Invalidater Invalidate;

        public RangeAndValue()
        {
            Init(-1, 1, 0, false, false);
        }

        public RangeAndValue(float min, float max)
        {
            Init(min, max, ((min + max) / 2f), false, false);
        }

        public RangeAndValue(float min, float max, float value)
        {
            Init(min, max, value, false, false);
        }

        public RangeAndValue(float min, float max, bool wrap)
        {
            Init(min, max, ((min + max) / 2f), wrap, false);
        }

        public RangeAndValue(float min, float max, float value, bool wrap)
        {
            Init(min, max, value, wrap, false);
        }

        public RangeAndValue(float min, float max, bool wrap, bool allowOOB)
        {
            Init(min, max, ((min + max) / 2f), wrap, allowOOB);
        }

        public RangeAndValue(float min, float max, float value, bool wrap, bool allowOOB)
        {
            Init(min, max, value, wrap, allowOOB);
        }

        public RangeAndValue(float min, float max, float value, bool wrap, bool allowOOB, Invalidater invalidatecall)
        {
            Init(min, max, value, wrap, allowOOB);
            Invalidate += invalidatecall;
        }

        protected void Init(float min, float max, float value, bool wrap, bool allowOOB)
        {
            this.min = min;
            this.max = max;
            this.valuei = value;
            this.wrap = wrap;
            this.oob = allowOOB;
        }

        public float Min
        {
            get
            {
                return min;
            }
            set
            {
                if (min != value)
                {
                    min = value;
                    if (!Validate())//updated value
                        OnInvalidate();
                }
            }
        }
        public float Max
        {
            get
            {
                return max;
            }
            set
            {
                if (max != value)
                {
                    max = value;
                    if (!Validate())//updated value
                        OnInvalidate();
                }
            }
        }
        public float Value
        {
            get
            {
                return valuei;
            }
            set
            {
                if (valuei != value)
                {
                    float valuet = Validate(value);
                    if (valuei != valuet)
                    {
                        valuei = valuet;
                        OnInvalidate();
                    }
                }
            }
        }

        public bool WrapValues
        {
            get
            {
                return wrap;
            }
            set
            {
                wrap = value;
                Validate();
            }
        }
        public bool AllowOutOfBounds
        {
            get
            {
                return oob;
            }
            set
            {
                oob = value;
            }
        }


        private void OnInvalidate()
        {
            if (Invalidate != null)
                Invalidate();
        }
        protected bool Validate()
        {
            float valuet = Validate(valuei);
            if (valuei != valuet)
            {
                valuei = valuet;
                OnInvalidate();
                return true;
            }
            return false;
        }
        protected float Validate(float value)
        {
            if (!oob)
            {
                if (max < min)
                    throw new ArgumentOutOfRangeException("Max should be greater than Min");
                if (max < value)
                    throw new ArgumentOutOfRangeException("Max should be greater than the value");
                if (min > value)
                    throw new ArgumentOutOfRangeException("Min should be less than the value");
            }
            else
            {
                if (max > min)
                {
                    if (value > max)
                    {
                        if (wrap)
                        {
                            while (value > max)
                                value -= (max - min);
                        }
                        else
                            value = max;
                    }
                    if (value < min)
                    {
                        if (wrap)
                        {
                            while (value < max)
                                value += (max - min);
                        }
                        else
                            value = min;
                    }
                }
            }
            return value;
        }

        public float Scale(float min, float max)
        {
            if (this.min == min && this.max == max)
                return this.valuei;//cheap 
            return (((this.valuei - this.min) / (this.max - this.min)) * (max - min)) + min;
        }

    }
}
