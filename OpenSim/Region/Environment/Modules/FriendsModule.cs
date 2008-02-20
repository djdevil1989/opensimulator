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
* THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS AS IS AND ANY
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

using Nini.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using OpenSim.Framework;
using OpenSim.Framework.Console;
using OpenSim.Region.Environment.Interfaces;
using OpenSim.Region.Environment.Scenes;
using libsecondlife;

namespace OpenSim.Region.Environment.Modules
{
    public class FriendsModule : IRegionModule
    {

        private LogBase m_log;

        private Scene m_scene;

        Dictionary<LLUUID, LLUUID> m_pendingFriendRequests = new Dictionary<LLUUID, LLUUID>();

        public void Initialise(Scene scene, IConfigSource config)
        {
            m_log = MainLog.Instance;
            m_scene = scene;
            scene.EventManager.OnNewClient += OnNewClient;
            scene.EventManager.OnGridInstantMessageToFriendsModule += OnGridInstantMessage;
        }

        private void OnNewClient(IClientAPI client)
        {
            // All friends establishment protocol goes over instant message
            // There's no way to send a message from the sim 
            // to a user to 'add a friend' without causing dialog box spam
            // 
            // The base set of friends are added when the user signs on in their XMLRPC response
            // Generated by LoginService.  The friends are retreived from the database by the UserManager

            // Subscribe to instant messages
            client.OnInstantMessage += OnInstantMessage;
            client.OnApproveFriendRequest += OnApprovedFriendRequest;
            client.OnDenyFriendRequest += OnDenyFriendRequest;
            client.OnTerminateFriendship += OnTerminateFriendship;

            
        }
        private void OnInstantMessage(IClientAPI client,LLUUID fromAgentID,
                                      LLUUID fromAgentSession, LLUUID toAgentID,
                                      LLUUID imSessionID, uint timestamp, string fromAgentName,
                                      string message, byte dialog, bool fromGroup, byte offline, 
                                      uint ParentEstateID, LLVector3 Position, LLUUID RegionID, 
                                      byte[] binaryBucket)
        {
            // Friend Requests go by Instant Message..    using the dialog param
            // https://wiki.secondlife.com/wiki/ImprovedInstantMessage

            // 38 == Offer friendship
            if (dialog == (byte)38)
            {
                LLUUID friendTransactionID = LLUUID.Random();

                m_pendingFriendRequests.Add(friendTransactionID, fromAgentID);

                m_log.Verbose("FRIEND", "38 - From:" + fromAgentID.ToString() + " To: " + toAgentID.ToString() + " Session:" + imSessionID.ToString() + " Message:" + message);
                GridInstantMessage msg = new GridInstantMessage();
                msg.fromAgentID = fromAgentID.UUID;
                msg.fromAgentSession = fromAgentSession.UUID;
                msg.toAgentID = toAgentID.UUID;
                msg.imSessionID = friendTransactionID.UUID; // This is the item we're mucking with here
                m_log.Verbose("FRIEND","Filling Session: " + msg.imSessionID.ToString());
                msg.timestamp = timestamp;
                if (client != null)
                {
                    msg.fromAgentName = client.FirstName + " " + client.LastName;// fromAgentName;
                }
                else
                {
                    msg.fromAgentName = "(hippos)";// Added for posterity.  This means that we can't figure out who sent it
                }
                msg.message = message;
                msg.dialog = dialog;
                msg.fromGroup = fromGroup;
                msg.offline = offline;
                msg.ParentEstateID = ParentEstateID;
                msg.Position = new sLLVector3(Position);
                msg.RegionID = RegionID.UUID;
                msg.binaryBucket = binaryBucket;
                m_scene.TriggerGridInstantMessage(msg, InstantMessageReceiver.IMModule);
            }
            if (dialog == (byte)39)
            {
                m_log.Verbose("FRIEND", "38 - From:" + fromAgentID.ToString() + " To: " + toAgentID.ToString() + " Session:" + imSessionID.ToString() + " Message:" + message);

            }
            if (dialog == (byte)40)
            {
                m_log.Verbose("FRIEND", "38 - From:" + fromAgentID.ToString() + " To: " + toAgentID.ToString() + " Session:" + imSessionID.ToString() + " Message:" + message);
            }

            // 39 == Accept Friendship

            // 40 == Decline Friendship
               
        }

        private void OnApprovedFriendRequest(IClientAPI client, LLUUID agentID, LLUUID transactionID, List<LLUUID> callingCardFolders)
        {
            if (m_pendingFriendRequests.ContainsKey(transactionID))
            {
                // Found Pending Friend Request with that Transaction..    

                // Compose response to other agent.
                GridInstantMessage msg = new GridInstantMessage();
                msg.toAgentID = m_pendingFriendRequests[transactionID].UUID;
                msg.fromAgentID = agentID.UUID;
                msg.fromAgentName = client.FirstName + " " + client.LastName;
                msg.fromAgentSession = client.SessionId.UUID;
                msg.fromGroup = false;
                msg.imSessionID = transactionID.UUID;
                msg.message = agentID.UUID.ToString();
                msg.ParentEstateID = 0;
                msg.timestamp = (uint)Util.UnixTimeSinceEpoch();
                msg.RegionID = m_scene.RegionInfo.RegionID.UUID;
                msg.dialog = (byte)39;// Approved friend request
                msg.Position = new sLLVector3();
                msg.offline = (byte)0;
                msg.binaryBucket = new byte[0];
                m_scene.TriggerGridInstantMessage(msg, InstantMessageReceiver.IMModule);
                m_scene.StoreAddFriendship(m_pendingFriendRequests[transactionID], agentID, (uint)1);
                m_pendingFriendRequests.Remove(transactionID);

                // TODO: Inform agent that the friend is online
            }
        }
        private void OnDenyFriendRequest(IClientAPI client, LLUUID agentID, LLUUID transactionID, List<LLUUID> callingCardFolders)
        {
            if (m_pendingFriendRequests.ContainsKey(transactionID))
            {
                // Found Pending Friend Request with that Transaction..    

                // Compose response to other agent.
                GridInstantMessage msg = new GridInstantMessage();
                msg.toAgentID = m_pendingFriendRequests[transactionID].UUID;
                msg.fromAgentID = agentID.UUID;
                msg.fromAgentName = client.FirstName + " " + client.LastName;
                msg.fromAgentSession = client.SessionId.UUID;
                msg.fromGroup = false;
                msg.imSessionID = transactionID.UUID;
                msg.message = agentID.UUID.ToString();
                msg.ParentEstateID = 0;
                msg.timestamp = (uint)Util.UnixTimeSinceEpoch();
                msg.RegionID = m_scene.RegionInfo.RegionID.UUID;
                msg.dialog = (byte)40;// Deny friend request
                msg.Position = new sLLVector3();
                msg.offline = (byte)0;
                msg.binaryBucket = new byte[0];
                m_scene.TriggerGridInstantMessage(msg, InstantMessageReceiver.IMModule);
                m_pendingFriendRequests.Remove(transactionID);

            }


        }

        private void OnTerminateFriendship(IClientAPI client, LLUUID agent, LLUUID exfriendID)
        {
            m_scene.StoreRemoveFriendship(agent, exfriendID);
            // TODO: Inform the client that the ExFriend is offline

        }


        private void OnGridInstantMessage(GridInstantMessage msg)
        {
            // Trigger the above event handler
            OnInstantMessage(null,new LLUUID(msg.fromAgentID), new LLUUID(msg.fromAgentSession),
                new LLUUID(msg.toAgentID), new LLUUID(msg.imSessionID), msg.timestamp, msg.fromAgentName,
                msg.message, msg.dialog, msg.fromGroup, msg.offline, msg.ParentEstateID,
                new LLVector3(msg.Position.x, msg.Position.y, msg.Position.z), new LLUUID(msg.RegionID),
                msg.binaryBucket);

        }


        public void PostInitialise()
        {
        }

        

        public void Close()
        {
        }

        public string Name
        {
            get { return "FriendsModule"; }
        }

        public bool IsSharedModule
        {
            get { return false; }
        }
    }
}