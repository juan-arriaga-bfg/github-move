// using System;
// using System.Collections.Generic;
// using Config;
//
// #if UNITY_EDITOR
//     using UnityEditor;
// #endif
//
// namespace Backend
// {
//     public class ClientUpdateDef
//     {
//         public string Id;
//         public string Url;
//     }
//
//     public class ClientUpdate
//     {
//         public delegate void ClientUpdateGetUpdatesListCallback(List<ClientUpdateDef> updatesList);
//         public static void GetUpdatesList(string group, ClientUpdateGetUpdatesListCallback callback)
//         {
//             Neskinsoft.Core.Logger.Log("ClientUpdate: Request updates list...");
//             
// #if DEBUG
//         #if UNITY_EDITOR
//             if (EditorPrefs.GetBool(UMFields.DISABLE_CLIENT_UPDATES, false))
//         #else
//             if (UserManager.Instance.GetValue<bool>(UMFields.DISABLE_CLIENT_UPDATES, false))
//         #endif
//             {
//                 Neskinsoft.Core.Logger.LogWarning("ClientUpdate: Skip request by DISABLE_CLIENT_UPDATES flag");
//                 
//                 List<ClientUpdateDef> emptyList = new List<ClientUpdateDef>(); 
//                 callback(emptyList);
//                 return;
//             }
// #endif
//             
//             Neskinsoft.Core.Logger.Assert(callback != null, "callback == null!");
//             if (callback == null)
//             {
//                 return;
//             }
//
// //            if (group != "A" && group != "B")
// //            {
// //                throw new ArgumentException("ClientUpdate: Unknown client group detected: " + group);
// //            }
//
//             var groupAsString = group;
//
//             //string groupAsString = "ALL";
//             //switch (group)
//             //{
//             //    case 0: break;
//             //    case 1: groupAsString = "A"; break;
//             //    case 2: groupAsString = "B"; break;
//             //    default:
//             //        Neskinsoft.Core.Logger.LogError("Unknown client group detected: " + group);
//             //        break;
//             //}
//
//             var prms = new Dictionary<string, string>
//             {
//                 {"client_group", groupAsString},
//             };
//
//             string installedIds = UserManager.Instance.GetValue(UMFields.APPLIED_UPDATE_IDS, "");
//             if (!string.IsNullOrEmpty(installedIds))
//             {
//                 prms.Add("installed_ids", installedIds);
//             }
//
//             NetworkUtils.Instance.PostToBackend("client-update/get-updates-list",
//                 prms,                                                                          
//                 (result) =>
//                 {
//                     Neskinsoft.Core.Logger.Log("ClientUpdate: get-updates-list request ended with result: " + result.IsOk);
//
//                     if (result.IsOk)
//                     {
//                         try
//                         {
//                             var updates = result.ResultAsJson["updates"];
//                             List<ClientUpdateDef> updatesList = new List<ClientUpdateDef>();
//                             foreach (var item in updates.Children)
//                             {
//                                 updatesList.Add(new ClientUpdateDef
//                                 {
//                                     Id = item["id"],
//                                     Url = item["url"]
//                                 });
//                             }
//                             callback(updatesList);
//                         }
//                         catch (Exception e)
//                         {
//                             Neskinsoft.Core.Logger.Log("ClientUpdate: GetList Error: " + e.GetType() + " " + e.Message);
//                             callback(null);
//                         }
//                     }
//                     else
//                     {
//                         var mess = string.IsNullOrEmpty(result.ErrorAsText) ? "Connection error" : result.ErrorAsText;
//                         Neskinsoft.Core.Logger.Log("ClientUpdate: GetList Error: " + mess);
//                         callback(null);
//                     }
//                 }
//             );
//         }
//     }
// }