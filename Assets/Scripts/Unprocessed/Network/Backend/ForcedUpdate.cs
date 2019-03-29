// using System;
// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using Config;
// using Neskinsoft.Core;
//
// namespace Backend
// {
//     public class ForcedUpdateDef
//     {
//         public string MinVersion;
//         public string LastVersion;
//         public string SoftMessage;
//         public string ForcedMessage;
//         public ForcedUpdateAction Action;
//
//         public ForcedUpdateDef(string minVersion, string lastVersion, string softMessage, string forcedMessage)
//         {
//             MinVersion = minVersion;
//             LastVersion = lastVersion;
//             SoftMessage = softMessage;
//             ForcedMessage = forcedMessage;
//
//             var version = Versions.GAME;
//             if (String.Compare(version, this.MinVersion, StringComparison.OrdinalIgnoreCase) < 0)
//             {
//                 Action = ForcedUpdateAction.Forced;
//             }
//             else if (String.Compare(version, this.LastVersion, StringComparison.OrdinalIgnoreCase) < 0)
//             {
//                 Action = ForcedUpdateAction.Soft;
//             }
//             else
//             {
//                 Action = ForcedUpdateAction.Skip;
//             }
//         }
//     }
//
//     public enum ForcedUpdateAction
//     {
//         Unknown,
//         Error,
//         Skip,
//         Soft,
//         Forced
//     }
//
//     public class ForcedUpdate
//     {
//         public delegate void ForcedUpdateGetCallback(ForcedUpdateDef def);
//         public static void Get(ForcedUpdateGetCallback callback)
//         {
//             Neskinsoft.Core.Logger.Log("ForcedUpdate: Request...");
//             Neskinsoft.Core.Logger.Assert(callback != null, "callback == null!");
//             if (callback == null)
//             {
//                 return;
//             }
//
//             NetworkUtils.Instance.PostToBackend("forced-update/get",
//                 null,
//                 (result) =>
//                 {
//                     if (result.IsOk)
//                     {
//                         try
//                         {
//                             ForcedUpdateDef def = new ForcedUpdateDef
//                             (
//                                 result.ResultAsJson["min_version"],
//                                 result.ResultAsJson["last_version"],
//                                 result.ResultAsJson["soft_message"],
//                                 result.ResultAsJson["forced_message"]
//                             );
//
//                             if (def.LastVersion == "0" || def.MinVersion == "0")
//                             {
//                                 Neskinsoft.Core.Logger.Log("ForcedUpdate: empty record on server");
//                                 def.Action = ForcedUpdateAction.Skip;
//                                 callback(def);
//                             }
//                             else
//                             {
//                                 callback(def);
//                             }
//                         }
//                         catch (Exception e)
//                         {
//                             Neskinsoft.Core.Logger.Log("ForcedUpdate: Error: " + e.GetType() + " " + e.Message);
//                             callback(null);
//                         }
//                     }
//                     else if (result.IsConnectionError)
//                     {
//                         Neskinsoft.Core.Logger.Log("ForcedUpdate: Connection error");
//                         callback(null);
//                     }
//                     else
//                     {
//                         Neskinsoft.Core.Logger.Log("SyncController: Error: " + result.ErrorAsText);
//                         callback(null);
//                     }
//                 }
//             );
//         }
//     }
// }