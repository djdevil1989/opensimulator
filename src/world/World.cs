using System;
using libsecondlife;
using libsecondlife.Packets;
using System.Collections.Generic;
using System.Text;
using PhysicsSystem;

namespace OpenSim.world
{
    public class World
    {
    	public Dictionary<libsecondlife.LLUUID, Entity> Entities;
    	public float[] LandMap;
    	public ScriptEngine Scripts;
    	public TerrainDecode terrainengine = new TerrainDecode();
    	public uint _localNumber=0;
    	private PhysicsScene phyScene;
    	private float timeStep= 0.1f;
    	
    	private Random Rand = new Random();

    	public World()
    	{
    		ServerConsole.MainConsole.Instance.WriteLine("World.cs - creating new entitities instance");
    		Entities = new Dictionary<libsecondlife.LLUUID, Entity>();

    		ServerConsole.MainConsole.Instance.WriteLine("World.cs - creating LandMap");
    		terrainengine = new TerrainDecode();
    		LandMap = new float[65536];
    		
    		
    		ServerConsole.MainConsole.Instance.WriteLine("World.cs - Creating script engine instance");
    		// Initialise this only after the world has loaded
    		Scripts = new ScriptEngine(this);
    	}
    	
    	public PhysicsScene PhysScene
    	{
    		set
    		{
    			this.phyScene = value;
    		}
    	}
    	
    	public void Update()
    	{
    		if(this.phyScene.IsThreaded)
    		{
    			this.phyScene.GetResults();
    			
    		}
    		
    		foreach (libsecondlife.LLUUID UUID in Entities.Keys)
    		{
    			Entities[UUID].addFroces();
    		}
    		
    		this.phyScene.Simulate(timeStep);
    		
    		foreach (libsecondlife.LLUUID UUID in Entities.Keys)
    		{
    			Entities[UUID].update();
    		}
    	}

    	public void SendLayerData(OpenSimClient RemoteClient) {
    		for(int x=0; x<16; x=x+4) for(int y=0; y<16; y++){
    			Packet layerpack=this.terrainengine.CreateLayerPacket(LandMap, x,y,x+4,y+1);
    			RemoteClient.OutPacket(layerpack);
    		}
    	}

    	public void AddViewerAgent(OpenSimClient AgentClient) {
    		ServerConsole.MainConsole.Instance.WriteLine("World.cs:AddViewerAgent() - Creating new avatar for remote viewer agent");
    		Avatar NewAvatar = new Avatar(AgentClient);
    		ServerConsole.MainConsole.Instance.WriteLine("World.cs:AddViewerAgent() - Adding new avatar to world");
    		this.Entities.Add(AgentClient.AgentID, NewAvatar);
    		ServerConsole.MainConsole.Instance.WriteLine("World.cs:AddViewerAgent() - Starting RegionHandshake ");
    		NewAvatar.SendRegionHandshake(this);
    		
    		NewAvatar.PhysActor = this.phyScene.AddAvatar(new PhysicsVector(NewAvatar.position.X, NewAvatar.position.Y, NewAvatar.position.Z));
    		//this.Update();		// will work for now, but needs to be optimised so we don't update everything in the sim for each new user
    	}

    	public bool Backup() {
    		/* TODO: Save the current world entities state. */

    		return false;
    	}
    }
}
