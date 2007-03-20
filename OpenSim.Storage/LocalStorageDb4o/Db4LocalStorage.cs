/*
* Copyright (c) OpenSim project, http://sim.opensecondlife.org/
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the <organization> nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY <copyright holder> ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL <copyright holder> BE LIABLE FOR ANY
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
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using libsecondlife;
using OpenSim.Framework.Interfaces;
using OpenSim.Framework.Assets;

namespace OpenSim.Storage.LocalStorageDb4o
{
	/// <summary>
	/// 
	/// </summary>
	public class Db4LocalStorage : ILocalStorage
	{
		private IObjectContainer db;
		
		public Db4LocalStorage()
		{
			try 
			{
				db = Db4oFactory.OpenFile("localworld.yap");
				OpenSim.Framework.Console.MainConsole.Instance.WriteLine("Db4LocalStorage creation");
			}
			catch(Exception e) 
			{
				db.Close();
				OpenSim.Framework.Console.MainConsole.Instance.WriteLine("Db4LocalStorage :Constructor - Exception occured");
				OpenSim.Framework.Console.MainConsole.Instance.WriteLine(e.ToString());
			}
		}
		
		public void StorePrim(PrimData prim)
		{
			IObjectSet result = db.Query(new UUIDQuery(prim.FullID));
			if(result.Count>0)
			{
				//prim already in storage
				//so update it
				PrimData found = (PrimData) result.Next();
				found.PathBegin = prim.PathBegin;
				found.PathCurve= prim.PathCurve;
				found.PathEnd = prim.PathEnd;
				found.PathRadiusOffset = prim.PathRadiusOffset;
				found.PathRevolutions = prim.PathRevolutions;
				found.PathScaleX= prim.PathScaleX;
				found.PathScaleY = prim.PathScaleY;
				found.PathShearX = prim.PathShearX;
				found.PathShearY = prim.PathShearY;
				found.PathSkew = prim.PathSkew;
				found.PathTaperX = prim.PathTaperX;
				found.PathTaperY = prim.PathTaperY;
				found.PathTwist = prim.PathTwist;
				found.PathTwistBegin = prim.PathTwistBegin;
				found.PCode = prim.PCode;
				found.ProfileBegin = prim.ProfileBegin;
				found.ProfileCurve = prim.ProfileCurve;
				found.ProfileEnd = prim.ProfileEnd;
				found.ProfileHollow = prim.ProfileHollow;
				found.Position = prim.Position;
				found.Rotation = prim.Rotation;
                found.Texture = prim.Texture;
				db.Set(found);
				db.Commit();
			}
			else
			{
				//not in storage
				db.Set(prim);
				db.Commit();
			}
		}
		
		public void RemovePrim(LLUUID primID)
		{
			IObjectSet result = db.Query(new UUIDQuery(primID));
			if(result.Count>0)
			{
				PrimData found = (PrimData) result.Next();
				db.Delete(found);
			}
		}
		
		
		public void LoadPrimitives(ILocalStorageReceiver receiver)
		{
			IObjectSet result = db.Get(typeof(PrimData));
			OpenSim.Framework.Console.MainConsole.Instance.WriteLine("Db4LocalStorage.cs: LoadPrimitives() - number of prims in storages is "+result.Count);
			foreach (PrimData prim in result) {
				receiver.PrimFromStorage(prim);
			}
		}
		
		public void ShutDown()
		{
			db.Commit();
			db.Close();
		}
	}
}
