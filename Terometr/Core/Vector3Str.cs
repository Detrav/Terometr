﻿using Detrav.Terometr.UserElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
{
    class Vector3Str
    {
        public string left;
        public double right;
        public PlayerBarElement.clr self;
        public Vector3Str(string left, double right, PlayerBarElement.clr self)
        {
            this.left = left;
            this.right = right;
            this.self = self;
        }
    }
}