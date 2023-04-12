﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;

namespace AriaLibrary.Helpers
{
    public static class AssimpHelpers
    {
        public static Matrix4x4 CalculateNodeMatrixWS(Node node)
        {
            // get the node transform matrix
            Matrix4x4 val = node.Transform;
            if (node.Parent != null)
            {
                // if the node has a parent run this block of code until there is no parent
                val *= CalculateNodeMatrixWS(node.Parent);
            }
            return val;
        }
    }
}
