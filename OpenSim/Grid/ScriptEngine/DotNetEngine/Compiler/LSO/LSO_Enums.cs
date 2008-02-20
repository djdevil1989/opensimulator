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
/* Original code: Tedd Hansen */
using System;

namespace OpenSim.Grid.ScriptEngine.DotNetEngine.Compiler.LSO
{
    public static class LSO_Enums
    {
        //public System.Collections.Generic.Dictionary<Byte, Type> OpCode_Add_Types;

        //LSO_Enums() {
        //    OpCode_Add_Types.Add(51, typeof(String));
        //    OpCode_Add_Types.Add(17, typeof(UInt32));
        //}

        [Serializable]
        public enum OpCode_Add_TypeDefs
        {
            String = 51,
            UInt32 = 17
        }

        [Serializable]
        public enum OpCode_Cast_TypeDefs
        {
            String = 19
        }

        [Serializable]
        public struct Key
        {
            public string KeyString;
        }

        [Serializable]
        public struct Vector
        {
            public UInt32 Z;
            public UInt32 Y;
            public UInt32 X;
        }

        [Serializable]
        public struct Rotation
        {
            public UInt32 S;
            public UInt32 Z;
            public UInt32 Y;
            public UInt32 X;
        }

        [Serializable]
        public enum Variable_Type_Codes
        {
            Void = 0,
            Integer = 1,
            Float = 2,
            String = 3,
            Key = 4,
            Vector = 5,
            Rotation = 6,
            List = 7,
            Null = 8
        }

        [Serializable]
        public enum Event_Mask_Values
        {
            state_entry = 0,
            state_exit = 1,
            touch_start = 2,
            touch = 3,
            touch_end = 4,
            collision_start = 5,
            collision = 6,
            collision_end = 7,
            land_collision_start = 8,
            land_collision = 9,
            land_collision_end = 10,
            timer = 11,
            listen = 12,
            on_rez = 13,
            sensor = 14,
            no_sensor = 15,
            control = 16,
            money = 17,
            email = 18,
            at_target = 19,
            not_at_target = 20,
            at_rot_target = 21,
            not_at_rot_target = 22,
            run_time_permissions = 23,
            changed = 24,
            attach = 25,
            dataserver = 26,
            link_message = 27,
            moving_start = 28,
            moving_end = 29,
            object_rez = 30,
            remote_data = 31,
            http_response = 32
        }

        [Serializable]
        public enum Operation_Table
        {
            NOOP = 0x0,
            POP = 0x1,
            POPS = 0x2,
            POPL = 0x3,
            POPV = 0x4,
            POPQ = 0x5,
            POPARG = 0x6,
            POPIP = 0x7,
            POPBP = 0x8,
            POPSP = 0x9,
            POPSLR = 0xa,
            DUP = 0x20,
            DUPS = 0x21,
            DUPL = 0x22,
            DUPV = 0x23,
            DUPQ = 0x24,
            STORE = 0x30,
            STORES = 0x31,
            STOREL = 0x32,
            STOREV = 0x33,
            STOREQ = 0x34,
            STOREG = 0x35,
            STOREGS = 0x36,
            STOREGL = 0x37,
            STOREGV = 0x38,
            STOREGQ = 0x39,
            LOADP = 0x3a,
            LOADSP = 0x3b,
            LOADLP = 0x3c,
            LOADVP = 0x3d,
            LOADQP = 0x3e,
            LOADGP = 0x3f,
            LOADGSP = 0x40,
            LOADGLP = 0x41,
            LOADGVP = 0x42,
            LOADGQP = 0x43,
            PUSH = 0x50,
            PUSHS = 0x51,
            PUSHL = 0x52,
            PUSHV = 0x53,
            PUSHQ = 0x54,
            PUSHG = 0x55,
            PUSHGS = 0x56,
            PUSHGL = 0x57,
            PUSHGV = 0x58,
            PUSHGQ = 0x59,
            PUSHIP = 0x5a,
            PUSHBP = 0x5b,
            PUSHSP = 0x5c,
            PUSHARGB = 0x5d,
            PUSHARGI = 0x5e,
            PUSHARGF = 0x5f,
            PUSHARGS = 0x60,
            PUSHARGV = 0x61,
            PUSHARGQ = 0x62,
            PUSHE = 0x63,
            PUSHEV = 0x64,
            PUSHEQ = 0x65,
            PUSHARGE = 0x66,
            ADD = 0x70,
            SUB = 0x71,
            MUL = 0x72,
            DIV = 0x73,
            MOD = 0x74,
            EQ = 0x75,
            NEQ = 0x76,
            LEQ = 0x77,
            GEQ = 0x78,
            LESS = 0x79,
            GREATER = 0x7a,
            BITAND = 0x7b,
            BITOR = 0x7c,
            BITXOR = 0x7d,
            BOOLAND = 0x7e,
            BOOLOR = 0x7f,
            NEG = 0x80,
            BITNOT = 0x81,
            BOOLNOT = 0x82,
            JUMP = 0x90,
            JUMPIF = 0x91,
            JUMPNIF = 0x92,
            STATE = 0x93,
            CALL = 0x94,
            RETURN = 0x95,
            CAST = 0xa0,
            STACKTOS = 0xb0,
            STACKTOL = 0xb1,
            PRINT = 0xc0,
            CALLLIB = 0xd0,
            CALLLIB_TWO_BYTE = 0xd1,
            SHL = 0xe0,
            SHR = 0xe1
        }

        [Serializable]
        public enum BuiltIn_Functions
        {
            llSin = 0,
            llCos = 1,
            llTan = 2,
            llAtan2 = 3,
            llSqrt = 4,
            llPow = 5,
            llAbs = 6,
            llFabs = 7,
            llFrand = 8,
            llFloor = 9,
            llCeil = 10,
            llRound = 11,
            llVecMag = 12,
            llVecNorm = 13,
            llVecDist = 14,
            llRot2Euler = 15,
            llEuler2Rot = 16,
            llAxes2Rot = 17,
            llRot2Fwd = 18,
            llRot2Left = 19,
            llRot2Up = 20,
            llRotBetween = 21,
            llWhisper = 22,
            llSay = 23,
            llShout = 24,
            llListen = 25,
            llListenControl = 26,
            llListenRemove = 27,
            llSensor = 28,
            llSensorRepeat = 29,
            llSensorRemove = 30,
            llDetectedName = 31,
            llDetectedKey = 32,
            llDetectedOwner = 33,
            llDetectedType = 34,
            llDetectedPos = 35,
            llDetectedVel = 36,
            llDetectedGrab = 37,
            llDetectedRot = 38,
            llDetectedGroup = 39,
            llDetectedLinkNumber = 40,
            llDie = 41,
            llGround = 42,
            llCloud = 43,
            llWind = 44,
            llSetStatus = 45,
            llGetStatus = 46,
            llSetScale = 47,
            llGetScale = 48,
            llSetColor = 49,
            llGetAlpha = 50,
            llSetAlpha = 51,
            llGetColor = 52,
            llSetTexture = 53,
            llScaleTexture = 54,
            llOffsetTexture = 55,
            llRotateTexture = 56,
            llGetTexture = 57,
            llSetPos = 58,
            llGetPos = 59,
            llGetLocalPos = 60,
            llSetRot = 61,
            llGetRot = 62,
            llGetLocalRot = 63,
            llSetForce = 64,
            llGetForce = 65,
            llTarget = 66,
            llTargetRemove = 67,
            llRotTarget = 68,
            llRotTargetRemove = 69,
            llMoveToTarget = 70,
            llStopMoveToTarget = 71,
            llApplyImpulse = 72,
            llApplyRotationalImpulse = 73,
            llSetTorque = 74,
            llGetTorque = 75,
            llSetForceAndTorque = 76,
            llGetVel = 77,
            llGetAccel = 78,
            llGetOmega = 79,
            llGetTimeOfDay = 80,
            llGetWallclock = 81,
            llGetTime = 82,
            llResetTime = 83,
            llGetAndResetTime = 84,
            llSound = 85,
            llPlaySound = 86,
            llLoopSound = 87,
            llLoopSoundMaster = 88,
            llLoopSoundSlave = 89,
            llPlaySoundSlave = 90,
            llTriggerSound = 91,
            llStopSound = 92,
            llPreloadSound = 93,
            llGetSubString = 94,
            llDeleteSubString = 95,
            llInsertString = 96,
            llToUpper = 97,
            llToLower = 98,
            llGiveMoney = 99,
            llMakeExplosion = 100,
            llMakeFountain = 101,
            llMakeSmoke = 102,
            llMakeFire = 103,
            llRezObject = 104,
            llLookAt = 105,
            llStopLookAt = 106,
            llSetTimerEvent = 107,
            llSleep = 108,
            llGetMass = 109,
            llCollisionFilter = 110,
            llTakeControls = 111,
            llReleaseControls = 112,
            llAttachToAvatar = 113,
            llDetachFromAvatar = 114,
            llTakeCamera = 115,
            llReleaseCamera = 116,
            llGetOwner = 117,
            llInstantMessage = 118,
            llEmail = 119,
            llGetNextEmail = 120,
            llGetKey = 121,
            llSetBuoyancy = 122,
            llSetHoverHeight = 123,
            llStopHover = 124,
            llMinEventDelay = 125,
            llSoundPreload = 126,
            llRotLookAt = 127,
            llStringLength = 128,
            llStartAnimation = 129,
            llStopAnimation = 130,
            llPointAt = 131,
            llStopPointAt = 132,
            llTargetOmega = 133,
            llGetStartParameter = 134,
            llGodLikeRezObject = 135,
            llRequestPermissions = 136,
            llGetPermissionsKey = 137,
            llGetPermissions = 138,
            llGetLinkNumber = 139,
            llSetLinkColor = 140,
            llCreateLink = 141,
            llBreakLink = 142,
            llBreakAllLinks = 143,
            llGetLinkKey = 144,
            llGetLinkName = 145,
            llGetInventoryNumber = 146,
            llGetInventoryName = 147,
            llSetScriptState = 148,
            llGetEnergy = 149,
            llGiveInventory = 150,
            llRemoveInventory = 151,
            llSetText = 152,
            llWater = 153,
            llPassTouches = 154,
            llRequestAgentData = 155,
            llRequestInventoryData = 156,
            llSetDamage = 157,
            llTeleportAgentHome = 158,
            llModifyLand = 159,
            llCollisionSound = 160,
            llCollisionSprite = 161,
            llGetAnimation = 162,
            llResetScript = 163,
            llMessageLinked = 164,
            llPushObject = 165,
            llPassCollisions = 166,
            llGetScriptName = 167,
            llGetNumberOfSides = 168,
            llAxisAngle2Rot = 169,
            llRot2Axis = 170,
            llRot2Angle = 171,
            llAcos = 172,
            llAsin = 173,
            llAngleBetween = 174,
            llGetInventoryKey = 175,
            llAllowInventoryDrop = 176,
            llGetSunDirection = 177,
            llGetTextureOffset = 178,
            llGetTextureScale = 179,
            llGetTextureRot = 180,
            llSubStringIndex = 181,
            llGetOwnerKey = 182,
            llGetCenterOfMass = 183,
            llListSort = 184,
            llGetListLength = 185,
            llList2Integer = 186,
            llList2Float = 187,
            llList2String = 188,
            llList2Key = 189,
            llList2Vector = 190,
            llList2Rot = 191,
            llList2List = 192,
            llDeleteSubList = 193,
            llGetListEntryType = 194,
            llList2CSV = 195,
            llCSV2List = 196,
            llListRandomize = 197,
            llList2ListStrided = 198,
            llGetRegionCorner = 199,
            llListInsertList = 200,
            llListFindList = 201,
            llGetObjectName = 202,
            llSetObjectName = 203,
            llGetDate = 204,
            llEdgeOfWorld = 205,
            llGetAgentInfo = 206,
            llAdjustSoundVolume = 207,
            llSetSoundQueueing = 208,
            llSetSoundRadius = 209,
            llKey2Name = 210,
            llSetTextureAnim = 211,
            llTriggerSoundLimited = 212,
            llEjectFromLand = 213,
            llParseString2List = 214,
            llOverMyLand = 215,
            llGetLandOwnerAt = 216,
            llGetNotecardLine = 217,
            llGetAgentSize = 218,
            llSameGroup = 219,
            llUnSit = 220,
            llGroundSlope = 221,
            llGroundNormal = 222,
            llGroundContour = 223,
            llGetAttached = 224,
            llGetFreeMemory = 225,
            llGetRegionName = 226,
            llGetRegionTimeDilation = 227,
            llGetRegionFPS = 228,
            llParticleSystem = 229,
            llGroundRepel = 230,
            llGiveInventoryList = 231,
            llSetVehicleType = 232,
            llSetVehicleFloatParam = 233,
            llSetVehicleVectorParam = 234,
            llSetVehicleRotationParam = 235,
            llSetVehicleFlags = 236,
            llRemoveVehicleFlags = 237,
            llSitTarget = 238,
            llAvatarOnSitTarget = 239,
            llAddToLandPassList = 240,
            llSetTouchText = 241,
            llSetSitText = 242,
            llSetCameraEyeOffset = 243,
            llSetCameraAtOffset = 244,
            llDumpList2String = 245,
            llScriptDanger = 246,
            llDialog = 247,
            llVolumeDetect = 248,
            llResetOtherScript = 249,
            llGetScriptState = 250,
            llRemoteLoadScript = 251,
            llSetRemoteScriptAccessPin = 252,
            llRemoteLoadScriptPin = 253,
            llOpenRemoteDataChannel = 254,
            llSendRemoteData = 255,
            llRemoteDataReply = 256,
            llCloseRemoteDataChannel = 257,
            llMD5String = 258,
            llSetPrimitiveParams = 259,
            llStringToBase64 = 260,
            llBase64ToString = 261,
            llXorBase64Strings = 262,
            llRemoteDataSetRegion = 263,
            llLog10 = 264,
            llLog = 265,
            llGetAnimationList = 266,
            llSetParcelMusicURL = 267,
            llGetRootPosition = 268,
            llGetRootRotation = 269,
            llGetObjectDesc = 270,
            llSetObjectDesc = 271,
            llGetCreator = 272,
            llGetTimestamp = 273,
            llSetLinkAlpha = 274,
            llGetNumberOfPrims = 275,
            llGetNumberOfNotecardLines = 276,
            llGetBoundingBox = 277,
            llGetGeometricCenter = 278,
            llGetPrimitiveParams = 279,
            llIntegerToBase64 = 280,
            llBase64ToInteger = 281,
            llGetGMTclock = 282,
            llGetSimulatorHostname = 283,
            llSetLocalRot = 284,
            llParseStringKeepNulls = 285,
            llRezAtRoot = 286,
            llGetObjectPermMask = 287,
            llSetObjectPermMask = 288,
            llGetInventoryPermMask = 289,
            llSetInventoryPermMask = 290,
            llGetInventoryCreator = 291,
            llOwnerSay = 292,
            llRequestSimulatorData = 293,
            llForceMouselook = 294,
            llGetObjectMass = 295,
            llListReplaceList = 296,
            llLoadURL = 297,
            llParcelMediaCommandList = 298,
            llParcelMediaQuery = 299,
            llModPow = 300,
            llGetInventoryType = 301,
            llSetPayPrice = 302,
            llGetCameraPos = 303,
            llGetCameraRot = 304,
            llSetPrimURL = 305,
            llRefreshPrimURL = 306,
            llEscapeURL = 307,
            llUnescapeURL = 308,
            llMapDestination = 309,
            llAddToLandBanList = 310,
            llRemoveFromLandPassList = 311,
            llRemoveFromLandBanList = 312,
            llSetCameraParams = 313,
            llClearCameraParams = 314,
            llListStatistics = 315,
            llGetUnixTime = 316,
            llGetParcelFlags = 317,
            llGetRegionFlags = 318,
            llXorBase64StringsCorrect = 319,
            llHTTPRequest = 320,
            llResetLandBanList = 321,
            llResetLandPassList = 322,
            llGetParcelPrimCount = 323,
            llGetParcelPrimOwners = 324,
            llGetObjectPrimCount = 325,
            llGetParcelMaxPrims = 326,
            llGetParcelDetails = 327
        }
    }
}
