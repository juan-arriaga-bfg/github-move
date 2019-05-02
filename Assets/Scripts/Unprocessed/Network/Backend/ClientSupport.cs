using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using BfgAnalytics;
using CodeStage.AntiCheat.ObscuredTypes;
using IW.SimpleJSON;

namespace Backend
{
    public class ClientSupportDef
    {
        public string Id;
        public string Data;
        public string StartMessage;
        public string EndMessage;
    }

    public enum ClientSupportFieldAction
    {
        Unknown,
        Add,
        Set
    }

    public struct ClientSupportFieldDescription
    {
        public string Path;
        public object Value;
        public Type Type;
        public ClientSupportFieldAction Action;

        // todo: FIX
        public ClientSupportFieldDescription(string path, string value, string type, string action)
        {
            switch (action)
            {
                case "+":
                case "add":
                    Action = ClientSupportFieldAction.Add;
                    break;
                case "=":
                case "set":
                    Action = ClientSupportFieldAction.Set;
                    break;
                default:
                    throw new Exception("Wrong action: " + action);
            }

            Type = Type.GetType(type);
            if (Type == null)
            {
                throw new Exception("Wrong type: " + type);
            }

            Path = path;

            // todo: FIX
            // Value = Factory.CreateByHandlerType(Type);
            // ((IConfigDeserializableFromTextItem)Value).Deserialize(value);

            Value = null;
        }
    }

    public static class ClientSupport
    {
        public delegate void ConsumeAndApplyCallback(string error);
        public static void ConsumeAndApplyDef(ClientSupportDef def, ConsumeAndApplyCallback callback)
        {
            IW.Logger.Log("ClientSupport: ConsumeAndApplyDef");
            Consume(def, isOk =>
            {
                if (!isOk)
                {
                    string error = string.Format(LocalizationService.Get("dlg.client.support.consume.error"), def.Id);
                    callback(error);
                }
                else
                {
                    AnalyticsService.Current.IsEnabled = false;

                    ApplyChangesToConfig(def, error =>
                    {
                        AnalyticsService.Current.IsEnabled = true;
                        callback(error);
                    });
                }
            });
        }

        // todo: FIX
        private static bool ApplyChangesForResetMode(JSONNode json, out string error)
        {
            error = null;
            // UserManagerInitializer.ResetAll();
            return true;
        }

        private static void ApplyChangesForFullMode(JSONNode json, Action<string> onComplete)
        {
            string configText = null;
            try
            {
                configText = json["configUpdate"]["data"].Value;
                if (string.IsNullOrEmpty(configText))
                {
                    throw new Exception("string.IsNullOrEmpty(configText) == true");
                }

                // XmlDocument doc = new XmlDocument();
                // doc.LoadXml(configText);
            }
            catch (Exception e)
            {
                IW.Logger.Log("Error when parse ClientSupportDef json: " + e.Message);
                var error = string.Format(LocalizationService.Get("dlg.client.support.error.wrong.data"), 4);
                onComplete(error);
            }

            ProfileSlots.SafeReplaceCurrentProfile(configText, onComplete);
        }

        // todo: FIX
        private static bool ApplyChangesForFieldsMode(JSONNode json, out string error)
        {
            error = null;
            var result = false;
            // List<ClientSupportFieldDescription> fields = new List<ClientSupportFieldDescription>();
            //
            // string level = "0";
            // bool completeAllStages = false;
            // try
            // {
            //     var fieldsJson = json["configUpdate"]["data"]["fields"];
            //     
            //     level = json["configUpdate"]["setLevel"].Value;
            //     
            //     if (json["configUpdate"]["completeAllStages"] != null)
            //     {
            //         completeAllStages = json["configUpdate"]["completeAllStages"];
            //     }
            //
            //     foreach (var fieldJson in fieldsJson.Children)
            //     {
            //         var field = new ClientSupportFieldDescription(fieldJson["p"], fieldJson["v"], fieldJson["t"], fieldJson["a"]);
            //         fields.Add(field);
            //     }
            // }
            // catch (Exception e)
            // {
            //     IW.Logger.Log("Error when parse ClientSupportDef json: " + e.Message);
            //     error = string.Format(LocalizationService.Get("dlg.client.support.error.wrong.data"), 4);
            //     return false;
            // }
            //
            // // Апдейтаем конфиг
            // bool result = UserManagerInitializer.ModifyConfigWithFailSafe(() =>
            // {
            //     // Уровни
            //     CheatSheetLevelsController.UnlockLevelsAndProvideReward(level, completeAllStages);
            //
            //     foreach (var field in fields)
            //     {
            //         // Убедимся, что искомый путь существует
            //         if (!UserManager.Instance.IsValueExists(field.Path))
            //         {
            //             Container container = UserManager.Instance;
            //
            //             if (field.Path.Contains("/"))
            //             {
            //                 var steps = field.Path.Split('/').ToList();
            //                 //string lastStep = steps[steps.Count - 1];
            //                 steps.RemoveAt(steps.Count - 1);
            //
            //
            //                 for (int i = 0; i < steps.Count; i++)
            //                 {
            //                     string step = steps[i];
            //                     if (!container.IsValueExists(step))
            //                     {
            //                         container.SetValue(step, new Container());
            //                     }
            //                     container = container.GetValue<Container>(step);
            //                 }
            //             }
            //
            //             // Создаем несуществующие поля
            //             if (!UserManager.Instance.IsValueExists(field.Path))
            //             {
            //                 var fieldType = field.Value.GetType();
            //
            //                 // todo: it seems like a working code, but
            //                 // there is a magic exception
            //                 //
            //                 //var handledType = Factory.GetHandlerTypeForHandler(fieldType);
            //                 //var obj = handledType.IsValueType ? Activator.CreateInstance(handledType) : null;
            //                 //UserManager.Instance.SetValue(field.Path, obj);
            //
            //                 // hack
            //                 CreateEmptyRecord(field.Path, fieldType);
            //             }
            //         }
            //
            //         // Апдейтаем значения
            //         switch (field.Action)
            //         {
            //             case ClientSupportFieldAction.Add:
            //                 if (field.Type == typeof (ObscuredIntValue))
            //                 {
            //                     UserManager.Instance.ShiftValue(field.Path, (ObscuredInt) ((IGetValueAsObject) field.Value).GetValueAsObject());
            //                 }
            //                 else if (field.Type == typeof(int))
            //                 {
            //                     UserManager.Instance.ShiftValue(field.Path, (int)((IGetValueAsObject)field.Value).GetValueAsObject());
            //                 }
            //                 else
            //                 {
            //                     throw new ArgumentOutOfRangeException("Add action may be applied only for int and ObscuredInt types");
            //                 }
            //                 break;
            //             case ClientSupportFieldAction.Set:
            //                 UserManager.Instance.SetValue(field.Path, ((IGetValueAsObject)field.Value).GetValueAsObject());
            //                 break;
            //             default:
            //                 throw new ArgumentOutOfRangeException();
            //         }
            //         IW.Logger.Log("Field updated: " + field.Path);
            //     }
            // });

            return result;
        }

        private static void ApplyChangesToConfig(ClientSupportDef def, Action<string> onComplete)
        {
            JSONNode json = null;
            string method = "unknown";
            try
            {
                // Парсим инфу
                json = JSON.Parse(def.Data);
                method = json["configUpdate"]["method"];
                    
                IW.Logger.Log("Client support: ApplyChangesToConfig: method: " + method);

                // Check version
                string version;
                version = json["configUpdate"]["version"];
                if (version != "*")
                {
                    if (version != IWProjectVersionSettings.Instance.CurrentVersion)
                    {
                        var mask = LocalizationService.Get("dlg.client.support.incorrect.version.error");
                        var error = string.Format(mask, IWProjectVersionSettings.Instance.CurrentVersion, version);
                        onComplete(error);
                    }
                }
            }
            catch (Exception e)
            {
                IW.Logger.Log("Error when parse version: " + e.Message);
                var error = string.Format(LocalizationService.Get("dlg.client.support.error.wrong.data"), 2);
                onComplete(error);
            }

            switch (method)
            {
                // case "reset":
                //     result = ApplyChangesForResetMode(json, out error);
                //     break;
                case "full":
                    ApplyChangesForFullMode(json, onComplete);
                    break;
                // case "fields":
                //     result = ApplyChangesForFieldsMode(json, out error);
                //     break;
                default:
                    var mess = $"Unknown mode for update config: {method}";
                    IW.Logger.Log(mess);
                    onComplete(mess);
                    break;
            }
        }


        public delegate void ClientSupportGetCallback(ClientSupportDef def);

        public static void Get(string userId, ClientSupportGetCallback callback)
        {
            IW.Logger.Log("ClientSupport: Get: Request for ID: " + userId ?? "null");
            IW.Logger.Assert(callback != null, "callback == null!");
            if (callback == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(userId))
            {
                IW.Logger.LogWarning("ClientSupport: Get: IsNullOrEmpty(userId) == true, request SKIPPED");
                callback(null);
                return;
            }

            NetworkUtils.Instance.PostToBackend("client-support/get", new Dictionary<string, string>
            {
                {"user_id", userId}
            }, (result) =>
            {
                if (result.IsOk)
                {
                    try
                    {
                        if (result.ResultAsText == "ok empty")
                        {
                            callback(null);
                        }
                        else
                        {
                            ClientSupportDef def = new ClientSupportDef
                            {
                                Id = result.ResultAsJson["id"],
                                Data = result.ResultAsJson["data"].ToString(),
                                StartMessage = result.ResultAsJson["data"]["configUpdate"]["startMessage"],
                                EndMessage = result.ResultAsJson["data"]["configUpdate"]["endMessage"],
                            };
                            callback(def);
                        }
                    }
                    catch (Exception e)
                    {
                        IW.Logger.Log("ClientSupport: Error: " + e.GetType() + " " + e.Message);
                        callback(null);
                    }
                }
                else if (result.IsConnectionError)
                {
                    IW.Logger.Log("ClientSupport: Connection error");
                    callback(null);
                }
                else
                {
                    IW.Logger.Log("ClientSupport: Error: " + result.ErrorAsText);
                    callback(null);
                }
            });
        }

        public delegate void ClientSupportConsumeCallback(bool isOk);
        public static void Consume(ClientSupportDef def, ClientSupportConsumeCallback callback)
        {
            IW.Logger.Log("ClientSupport: Request...");
            IW.Logger.Assert(callback != null, "callback == null!");
            if (callback == null)
            {
                return;
            }

            IW.Logger.Assert(!string.IsNullOrEmpty(def.Id), "IsNullOrEmpty(def.Id) == true!");
            if (string.IsNullOrEmpty(def.Id))
            {
                return;
            }

            NetworkUtils.Instance.PostToBackend("client-support/consume", new Dictionary<string, string>
            {
                {"client_support_id", def.Id}
            }, (result) =>
            {
                if (result.IsOk)
                {
                    try
                    {
                        if (result.ResultAsText == "ok")
                        {
                            callback(true);
                        }
                    }
                    catch (Exception e)
                    {
                        IW.Logger.Log("ClientSupport: Consume: " + e.GetType() + " " + e.Message);
                        callback(false);
                    }
                }
                else if (result.IsConnectionError)
                {
                    IW.Logger.Log("ClientSupport: Consume: Connection error");
                    callback(false);
                }
                else
                {
                    IW.Logger.Log("ClientSupport: Consume: Error: " + result.ErrorAsText);
                    callback(false);
                }
            });
        }
    }
}