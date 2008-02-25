/*
* Copyright (c) Contributors, http://opensimulator.org/
* See CONTRIBUTORS.TXT for a full list of copyright holders.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the OpenSim Project nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
* 
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using OpenSim.Region.Physics.Manager;
using OpenSim.Region.Physics.Meshing;

public class Vertex : PhysicsVector, IComparable<Vertex>
{
    public Vertex(float x, float y, float z)
        : base(x, y, z)
    {
    }

    public Vertex(PhysicsVector v)
        : base(v.X, v.Y, v.Z)
    {
    }

    public Vertex Clone()
    {
        return new Vertex(X, Y, Z);
    }

    public static Vertex FromAngle(double angle)
    {
        return new Vertex((float) Math.Cos(angle), (float) Math.Sin(angle), 0.0f);
    }


    public virtual bool Equals(Vertex v, float tolerance)
    {
        PhysicsVector diff = this - v;
        float d = diff.length();
        if (d < tolerance)
            return true;

        return false;
    }


    public int CompareTo(Vertex other)
    {
        if (X < other.X)
            return -1;

        if (X > other.X)
            return 1;

        if (Y < other.Y)
            return -1;

        if (Y > other.Y)
            return 1;

        if (Z < other.Z)
            return -1;

        if (Z > other.Z)
            return 1;

        return 0;
    }

    public static bool operator >(Vertex me, Vertex other)
    {
        return me.CompareTo(other) > 0;
    }

    public static bool operator <(Vertex me, Vertex other)
    {
        return me.CompareTo(other) < 0;
    }

    public String ToRaw()
    {
        // Why this stuff with the number formatter?
        // Well, the raw format uses the english/US notation of numbers
        // where the "," separates groups of 1000 while the "." marks the border between 1 and 10E-1.
        // The german notation uses these characters exactly vice versa!
        // The Float.ToString() routine is a localized one, giving different results depending on the country
        // settings your machine works with. Unusable for a machine readable file format :-(
        NumberFormatInfo nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        nfi.NumberDecimalDigits = 3;

        String s1 = X.ToString("N2", nfi) + " " + Y.ToString("N2", nfi) + " " + Z.ToString("N2", nfi);

        return s1;
    }
}

public class Triangle
{
    public Vertex v1;
    public Vertex v2;
    public Vertex v3;

    private float radius_square;
    private float cx;
    private float cy;

    public Triangle(Vertex _v1, Vertex _v2, Vertex _v3)
    {
        v1 = _v1;
        v2 = _v2;
        v3 = _v3;

        CalcCircle();
    }

    public bool isInCircle(float x, float y)
    {
        float dx, dy;
        float dd;

        dx = x - cx;
        dy = y - cy;

        dd = dx*dx + dy*dy;
        if (dd < radius_square)
            return true;
        else
            return false;
    }

    public bool isDegraded()
    {
        // This means, the vertices of this triangle are somewhat strange.
        // They either line up or at least two of them are identical
        return (radius_square == 0.0);
    }

    private void CalcCircle()
    {
        // Calculate the center and the radius of a circle given by three points p1, p2, p3
        // It is assumed, that the triangles vertices are already set correctly
        double p1x, p2x, p1y, p2y, p3x, p3y;

        // Deviation of this routine: 
        // A circle has the general equation (M-p)^2=r^2, where M and p are vectors
        // this gives us three equations f(p)=r^2, each for one point p1, p2, p3
        // putting respectively two equations together gives two equations
        // f(p1)=f(p2) and f(p1)=f(p3)
        // bringing all constant terms to one side brings them to the form
        // M*v1=c1 resp.M*v2=c2 where v1=(p1-p2) and v2=(p1-p3) (still vectors)
        // and c1, c2 are scalars (Naming conventions like the variables below)
        // Now using the equations that are formed by the components of the vectors
        // and isolate Mx lets you make one equation that only holds My
        // The rest is straight forward and eaasy :-)
        // 

        /* helping variables for temporary results */
        double c1, c2;
        double v1x, v1y, v2x, v2y;

        double z, n;

        double rx, ry;

        // Readout the three points, the triangle consists of
        p1x = v1.X;
        p1y = v1.Y;

        p2x = v2.X;
        p2y = v2.Y;

        p3x = v3.X;
        p3y = v3.Y;

        /* calc helping values first */
        c1 = (p1x*p1x + p1y*p1y - p2x*p2x - p2y*p2y)/2;
        c2 = (p1x*p1x + p1y*p1y - p3x*p3x - p3y*p3y)/2;

        v1x = p1x - p2x;
        v1y = p1y - p2y;

        v2x = p1x - p3x;
        v2y = p1y - p3y;

        z = (c1*v2x - c2*v1x);
        n = (v1y*v2x - v2y*v1x);

        if (n == 0.0) // This is no triangle, i.e there are (at least) two points at the same location
        {
            radius_square = 0.0f;
            return;
        }

        cy = (float) (z/n);

        if (v2x != 0.0)
        {
            cx = (float) ((c2 - v2y*cy)/v2x);
        }
        else if (v1x != 0.0)
        {
            cx = (float) ((c1 - v1y*cy)/v1x);
        }
        else
        {
            Debug.Assert(false, "Malformed triangle"); /* Both terms zero means nothing good */
        }

        rx = (p1x - cx);
        ry = (p1y - cy);

        radius_square = (float) (rx*rx + ry*ry);
    }

    public List<Simplex> GetSimplices()
    {
        List<Simplex> result = new List<Simplex>();
        Simplex s1 = new Simplex(v1, v2);
        Simplex s2 = new Simplex(v2, v3);
        Simplex s3 = new Simplex(v3, v1);

        result.Add(s1);
        result.Add(s2);
        result.Add(s3);

        return result;
    }

    public override String ToString()
    {
        NumberFormatInfo nfi = new NumberFormatInfo();
        nfi.CurrencyDecimalDigits = 2;
        nfi.CurrencyDecimalSeparator = ".";

        String s1 = "<" + v1.X.ToString(nfi) + "," + v1.Y.ToString(nfi) + "," + v1.Z.ToString(nfi) + ">";
        String s2 = "<" + v2.X.ToString(nfi) + "," + v2.Y.ToString(nfi) + "," + v2.Z.ToString(nfi) + ">";
        String s3 = "<" + v3.X.ToString(nfi) + "," + v3.Y.ToString(nfi) + "," + v3.Z.ToString(nfi) + ">";

        return s1 + ";" + s2 + ";" + s3;
    }

    public PhysicsVector getNormal()
    {
        // Vertices

        // Vectors for edges
        PhysicsVector e1;
        PhysicsVector e2;

        e1 = new PhysicsVector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        e2 = new PhysicsVector(v1.X - v3.X, v1.Y - v3.Y, v1.Z - v3.Z);

        // Cross product for normal
        PhysicsVector n = PhysicsVector.cross(e1, e2);

        // Length
        float l = n.length();

        // Normalized "normal"
        n = n/l;

        return n;
    }

    public void invertNormal()
    {
        Vertex vt;
        vt = v1;
        v1 = v2;
        v2 = vt;
    }

    // Dumps a triangle in the "raw faces" format, blender can import. This is for visualisation and 
    // debugging purposes
    public String ToStringRaw()
    {
        String output = v1.ToRaw() + " " + v2.ToRaw() + " " + v3.ToRaw();
        return output;
    }
}
