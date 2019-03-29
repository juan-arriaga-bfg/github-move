// using System;
// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Xml;
// using CodeStage.AntiCheat.ObscuredTypes;
// using Config;
// using Neskinsoft.Core;
// using Neskinsoft.Core.SimpleJSON;
//
// namespace Backend
// {
//     public class ClientSupportDef
//     {
//         public string Id;
//         public string Data;
//         public string StartMessage;
//         public string EndMessage;
//     }
//
//     public enum ClientSupportFieldAction
//     {
//         Unknown,
//         Add,
//         Set
//     }
//
//     public struct ClientSupportFieldDescription
//     {
//         public string Path;
//         public object Value;
//         public Type Type;
//         public ClientSupportFieldAction Action;
//
//         public ClientSupportFieldDescription(string path, string value, string type, string action)
//         {
//             switch (action)
//             {
//                 case "+":
//                 case "add":
//                     Action = ClientSupportFieldAction.Add;
//                     break;
//                 case "=":
//                 case "set":
//                     Action = ClientSupportFieldAction.Set;
//                     break;
//                 default:
//                     throw new Exception("Wrong action: " + action);
//             }
//
//             Type = Type.GetType(type);
//             if (Type == null)
//             {
//                 throw new Exception("Wrong type: " + type);
//             }
//
//             Path = path;
//
//             Value = Factory.CreateByHandlerType(Type);
//             ((IConfigDeserializableFromTextItem)Value).Deserialize(value);
//         }
//     }
//
//     public class ClientSupport
//     {
//         public delegate void ConsumeAndApplyCallback(string error);
//         public static void ConsumeAndApplyDef(ClientSupportDef def, ConsumeAndApplyCallback callback)
//         {
//             Neskinsoft.Core.Logger.Log("ClientSupport: ConsumeAndApplyDef");
//             Consume(def, isOk =>
//             {
//                 if (!isOk)
//                 {
//                     string error = string.Format(Localizator.Instance.Get("dlg.client.support.consume.error"), def.Id);
//                     callback(error);
//                 }
//                 else
//                 {
//                     AnalyticsController.Instance.IsEnabled = false;
//
//                     string error = null;
//                     var result = ApplyChangesToConfig(def, out error);
//
//                     AnalyticsController.Instance.IsEnabled = true;
//
//                     callback(error);
//                 }
//             });
//         }
//
//         private static bool ApplyChangesForResetMode(JSONNode json, out string error)
//         {
//             error = null;
//             UserManagerInitializer.ResetAll();
//             return true;
//         }
//
//         private static bool ApplyChangesForFullMode(JSONNode json, out string error)
//         {
//             error = null;
//             string configText;
//             try
//             {
//                 configText = json["configUpdate"]["data"];
//                 if (string.IsNullOrEmpty(configText))
//                 {
//                     throw new Exception("string.IsNullOrEmpty(configText) == true");
//                 }
//
//                 XmlDocument doc = new XmlDocument();
//                 doc.LoadXml(configText);
//             }
//             catch (Exception e)
//             {
//                 Neskinsoft.Core.Logger.Log("Error when parse ClientSupportDef json: " + e.Message);
//                 error = string.Format(Localizator.Instance.Get("dlg.client.support.error.wrong.data"), 4);
//                 return false;
//             }
//
//             // Апдейтаем конфиг
//             bool result = UserManagerInitializer.ModifyConfigWithFailSafe(() =>
//             {
//                 UserManagerInitializer.ReplaceCurrentProgressFile(configText, false, false);
//             });
//
//             return result;
//         }
//
//         private static bool ApplyChangesForFieldsMode(JSONNode json, out string error)
//         {
//             error = null;
//             List<ClientSupportFieldDescription> fields = new List<ClientSupportFieldDescription>();
//
//             string level = "0";
//             bool completeAllStages = false;
//             try
//             {
//                 var fieldsJson = json["configUpdate"]["data"]["fields"];
//                 
//                 level = json["configUpdate"]["setLevel"].Value;
//                 
//                 if (json["configUpdate"]["completeAllStages"] != null)
//                 {
//                     completeAllStages = json["configUpdate"]["completeAllStages"];
//                 }
//
//                 foreach (var fieldJson in fieldsJson.Children)
//                 {
//                     var field = new ClientSupportFieldDescription(fieldJson["p"], fieldJson["v"], fieldJson["t"], fieldJson["a"]);
//                     fields.Add(field);
//                 }
//             }
//             catch (Exception e)
//             {
//                 Neskinsoft.Core.Logger.Log("Error when parse ClientSupportDef json: " + e.Message);
//                 error = string.Format(Localizator.Instance.Get("dlg.client.support.error.wrong.data"), 4);
//                 return false;
//             }
//
//             // Апдейтаем конфиг
//             bool result = UserManagerInitializer.ModifyConfigWithFailSafe(() =>
//             {
//                 // Уровни
//                 CheatSheetLevelsController.UnlockLevelsAndProvideReward(level, completeAllStages);
//
//                 foreach (var field in fields)
//                 {
//                     // Убедимся, что искомый путь существует
//                     if (!UserManager.Instance.IsValueExists(field.Path))
//                     {
//                         Container container = UserManager.Instance;
//
//                         if (field.Path.Contains("/"))
//                         {
//                             var steps = field.Path.Split('/').ToList();
//                             //string lastStep = steps[steps.Count - 1];
//                             steps.RemoveAt(steps.Count - 1);
//
//
//                             for (int i = 0; i < steps.Count; i++)
//                             {
//                                 string step = steps[i];
//                                 if (!container.IsValueExists(step))
//                                 {
//                                     container.SetValue(step, new Container());
//                                 }
//                                 container = container.GetValue<Container>(step);
//                             }
//                         }
//
//                         // Создаем несуществующие поля
//                         if (!UserManager.Instance.IsValueExists(field.Path))
//                         {
//                             var fieldType = field.Value.GetType();
//
//                             // todo: it seems like a working code, but
//                             // there is a magic exception
//                             //
//                             //var handledType = Factory.GetHandlerTypeForHandler(fieldType);
//                             //var obj = handledType.IsValueType ? Activator.CreateInstance(handledType) : null;
//                             //UserManager.Instance.SetValue(field.Path, obj);
//
//                             // hack
//                             CreateEmptyRecord(field.Path, fieldType);
//                         }
//                     }
//
//                     // Апдейтаем значения
//                     switch (field.Action)
//                     {
//                         case ClientSupportFieldAction.Add:
//                             if (field.Type == typeof (ObscuredIntValue))
//                             {
//                                 UserManager.Instance.ShiftValue(field.Path, (ObscuredInt) ((IGetValueAsObject) field.Value).GetValueAsObject());
//                             }
//                             else if (field.Type == typeof(int))
//                             {
//                                 UserManager.Instance.ShiftValue(field.Path, (int)((IGetValueAsObject)field.Value).GetValueAsObject());
//                             }
//                             else
//                             {
//                                 throw new ArgumentOutOfRangeException("Add action may be applied only for int and ObscuredInt types");
//                             }
//                             break;
//                         case ClientSupportFieldAction.Set:
//                             UserManager.Instance.SetValue(field.Path, ((IGetValueAsObject)field.Value).GetValueAsObject());
//                             break;
//                         default:
//                             throw new ArgumentOutOfRangeException();
//                     }
//                     Neskinsoft.Core.Logger.Log("Field updated: " + field.Path);
//                 }
//             });
//
//             return result;
//         }
//
//         private static bool ApplyChangesToConfig(ClientSupportDef def, out string error)
//         {
//             error = null;
//             JSONNode json;
//             string method = "unknown";
//
//             try
//             {
//                 // Парсим инфу
//                 json = JSON.Parse(def.Data);
//                 method = json["configUpdate"]["method"];
//
//                 Neskinsoft.Core.Logger.Log("Client support: ApplyChangesToConfig: method: " + method);
//
//                 // Check version
//                 string version;
//                 version = json["configUpdate"]["version"];
//                 if (version != "*")
//                 {
//                     if (version != Versions.GAME)
//                     {
//                         var mask = Localizator.Instance.Get("dlg.client.support.incorrect.version.error");
//                         error = string.Format(mask, Versions.GAME, version);
//                         return false; 
//                     }
//                 }
//             }
//             catch (Exception e)
//             {
//                 Neskinsoft.Core.Logger.Log("Error when parse version: " + e.Message);
//                 error = string.Format(Localizator.Instance.Get("dlg.client.support.error.wrong.data"), 2);
//                 return false;
//             }
//
//             bool result;
//
//             switch (method)
//             {
//                 case "reset":
//                     result = ApplyChangesForResetMode(json, out error);
//                     break;
//                 case "full":
//                     result = ApplyChangesForFullMode(json, out error);
//                     break;
//                 case "fields":
//                     result = ApplyChangesForFieldsMode(json, out error);
//                     break;
//                 default:
//                     Neskinsoft.Core.Logger.Log("Unknown mode for update config: " + method);
//                     result = false;
//                     break;
//             }
//
//             if (!result)
//             {
//                 Neskinsoft.Core.Logger.Log("Error when Apply configUpdate");
//                 if (string.IsNullOrEmpty(error))
//                 {
//                     error = string.Format(Localizator.Instance.Get("dlg.client.support.error.wrong.data"), 3);
//                 }
//             }
//             return result;
//         }
//
//         // govnocode mode on
//         private static void CreateEmptyRecord(string path, Type fieldType)
//         {
//             if      (fieldType == typeof(ObscuredIntValue))     { UserManager.Instance.SetValue<ObscuredInt>(path, 0); }
//             else if (fieldType == typeof(IntValue))             { UserManager.Instance.SetValue(path, 0);}
//             else if (fieldType == typeof(StringValue))          { UserManager.Instance.SetValue(path, string.Empty);}
//             else if (fieldType == typeof(FloatValue))           { UserManager.Instance.SetValue(path, 0f);}
//             else if (fieldType == typeof(PieceTypeValue))       { UserManager.Instance.SetValue(path, PieceType.None);}
//             else if (fieldType == typeof(BoolValue))            { UserManager.Instance.SetValue(path, false);}
//             else if (fieldType == typeof(MissionValue))         { UserManager.Instance.SetValue(path, new Mission("",0,0,MissionTargetOperator.Equal, MissionTargetAction.Unknown, new string[]{}));}
//             else if (fieldType == typeof(CellsPositionsValue))  { UserManager.Instance.SetValue(path,  new CellsPositions(0,0,null,null));}
//             else if (fieldType == typeof(DateTimeValue))        { UserManager.Instance.SetValue(path, new DateTime());}
//             else if (fieldType == typeof(TypeValue))            { UserManager.Instance.SetValue(path, typeof(int));}
//             else if (fieldType == typeof(PriceValue))           { UserManager.Instance.SetValue(path, new Price());}
//             else if (fieldType == typeof(Vector2Value))         { UserManager.Instance.SetValue(path, Vector2.zero);}
//             else if (fieldType == typeof(CellPointValue))       { UserManager.Instance.SetValue(path, new CellPoint());}
//             else if (fieldType == typeof(PieceWeightValue))     { UserManager.Instance.SetValue(path, new PiecesWeight());}
//             else if (fieldType == typeof(ObscuredLongValue))    { UserManager.Instance.SetValue<ObscuredLong>(path, 0); }
//             else if (fieldType == typeof(LongValue))            { UserManager.Instance.SetValue(path, 0); }
//             else { throw new Exception("Unknown type: " + fieldType);}
//         }
//
//         public delegate void ClientSupportGetCallback(ClientSupportDef def);
//
//         public static void Get(string userId, ClientSupportGetCallback callback)
//         {
//             Neskinsoft.Core.Logger.Log("ClientSupport: Get: Request for ID: " + userId ?? "null");
//             Neskinsoft.Core.Logger.Assert(callback != null, "callback == null!");
//             if (callback == null)
//             {
//                 return;
//             }
//
//             if (string.IsNullOrEmpty(userId))
//             {
//                 Neskinsoft.Core.Logger.LogWarning("ClientSupport: Get: IsNullOrEmpty(userId) == true, request SKIPPED");
//                 return;
//             }
//
//             NetworkUtils.Instance.PostToBackend("client-support/get", new Dictionary<string, string>
//             {
//                 {"user_id", userId}
//             }, (result) =>
//             {
//                 if (result.IsOk)
//                 {
//                     try
//                     {
//                         if (result.ResultAsText == "ok empty")
//                         {
//                             callback(null);
//                         }
//                         else
//                         {
//                             ClientSupportDef def = new ClientSupportDef
//                             {
//                                 Id = result.ResultAsJson["id"],
//                                 Data = result.ResultAsJson["data"].ToString(),
//                                 StartMessage = result.ResultAsJson["data"]["configUpdate"]["startMessage"],
//                                 EndMessage = result.ResultAsJson["data"]["configUpdate"]["endMessage"],
//                             };
//                             callback(def);
//                         }
//                     }
//                     catch (Exception e)
//                     {
//                         Neskinsoft.Core.Logger.Log("ClientSupport: Error: " + e.GetType() + " " + e.Message);
//                         callback(null);
//                     }
//                 }
//                 else if (result.IsConnectionError)
//                 {
//                     Neskinsoft.Core.Logger.Log("ClientSupport: Connection error");
//                     callback(null);
//                 }
//                 else
//                 {
//                     Neskinsoft.Core.Logger.Log("ClientSupport: Error: " + result.ErrorAsText);
//                     callback(null);
//                 }
//             });
//         }
//
//         public delegate void ClientSupportConsumeCallback(bool isOk);
//         public static void Consume(ClientSupportDef def, ClientSupportConsumeCallback callback)
//         {
//             Neskinsoft.Core.Logger.Log("ClientSupport: Request...");
//             Neskinsoft.Core.Logger.Assert(callback != null, "callback == null!");
//             if (callback == null)
//             {
//                 return;
//             }
//
//             Neskinsoft.Core.Logger.Assert(!string.IsNullOrEmpty(def.Id), "IsNullOrEmpty(def.Id) == true!");
//             if (string.IsNullOrEmpty(def.Id))
//             {
//                 return;
//             }
//
//             NetworkUtils.Instance.PostToBackend("client-support/consume", new Dictionary<string, string>
//             {
//                 {"client_support_id", def.Id}
//             }, (result) =>
//             {
//                 if (result.IsOk)
//                 {
//                     try
//                     {
//                         if (result.ResultAsText == "ok")
//                         {
//                             callback(true);
//                         }
//                     }
//                     catch (Exception e)
//                     {
//                         Neskinsoft.Core.Logger.Log("ClientSupport: Consume: " + e.GetType() + " " + e.Message);
//                         callback(false);
//                     }
//                 }
//                 else if (result.IsConnectionError)
//                 {
//                     Neskinsoft.Core.Logger.Log("ClientSupport: Consume: Connection error");
//                     callback(false);
//                 }
//                 else
//                 {
//                     Neskinsoft.Core.Logger.Log("ClientSupport: Consume: Error: " + result.ErrorAsText);
//                     callback(false);
//                 }
//             });
//         }
//     }
// }