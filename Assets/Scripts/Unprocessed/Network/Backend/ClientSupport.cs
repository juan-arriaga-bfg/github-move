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

    public static class ClientSupport
    {
        public delegate void ConsumeAndApplyCallback(string error);
        public static void ConsumeAndApplyDef(ClientSupportDef def, ConsumeAndApplyCallback callback)
        {
            IW.Logger.Log("ClientSupport: ConsumeAndApplyDef");
            
            ProfileService.Instance.Manager.UploadCurrentProfile(false);
                
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
            }
            catch (Exception e)
            {
                IW.Logger.Log("Error when parse ClientSupportDef json: " + e.Message);
                var error = string.Format(LocalizationService.Get("dlg.client.support.error.wrong.data"), 4);
                onComplete(error);
            }

            ProfileSlots.SafeReplaceCurrentProfile(configText, onComplete);
        }

        private static void ApplyChangesForFieldsMode(JSONNode json, Action<string> onComplete)
        {
            ProfileSlots.Load(ProfileSlots.ActiveSlot, (manager, dataExistsOnPath, error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    onComplete(error);
                    return;
                }

                try
                {
                    UserProfile userProfile = manager.CurrentProfile;
                    
                    var dm = new GameDataManager();
                    dm.SetupComponents(userProfile);

                    //     "data": {
                    //         "currency": [
                    //         {
                    //             "id": "10",
                    //             "action": "+",
                    //             "value": "12"
                    //         },
                    //         {
                    //             "id": "20",
                    //             "action": "+",
                    //             "value": "33"
                    //         }
                    //         ]
                    //     }
                    // }

                    var data = json["configUpdate"]["data"];
                    JSONNode currencies = data["currency"];
                    JSONNode airship = data["airship"];

                    if (currencies != null)
                    {
                        UserPurchasesComponent purchases = userProfile.Purchases;
                        
                        var arr = currencies.AsArray.Values;
                        foreach (JSONNode item in arr)
                        {
                            int id = item["id"].AsInt;
                            string action = item["action"].Value;
                            int value = item["value"].AsInt;

                            var def = Currency.GetCurrencyDef(id);
                            if (def == null)
                            {
                                throw new Exception($"Unknown currency id '{id}'");
                            }

                            var storageItem = purchases.GetStorageItem(def.Name);

                            // Hack for LEVEL
                            if (id == Currency.Level.Id)
                            {
                                var lvlManager = dm.LevelsManager;
                                int curLevel = storageItem.Amount;
                                int targetLevel = action == "+" ? curLevel + value : value;

                                int expToAdd = 0;
                                foreach (var lvlDef in lvlManager.Levels)
                                {
                                    if (lvlDef.Index > curLevel - 1 && lvlDef.Index < targetLevel)
                                    {
                                        expToAdd += lvlDef.Price.Amount;
                                    }
                                }

                                var expStorage = purchases.GetStorageItem(Currency.Experience.Name);
                                expStorage.Amount += expToAdd;
                                
                                continue;
                            }
                            //
                            
                            switch (action)
                            {
                                case "+" :
                                    storageItem.Amount += value;
                                    break;       
                                
                                case "=" :
                                    storageItem.Amount = value;
                                    break;
                                
                                default:
                                    throw new Exception($"Unknown action '{action}'");
                            }
                        }
                    }

                    if (airship != null)
                    {
                        string airshipValue = airship.Value.ToString();
                        if (!string.IsNullOrEmpty(airshipValue))
                        {
                            List<string> uids = airshipValue.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).ToList();
                        
                            var airShipSaveComponent = userProfile.GetComponent<AirShipSaveComponent>(AirShipSaveComponent.ComponentGuid);

                            Dictionary<int, int> payload = new Dictionary<int, int>();
                            foreach (var uid in uids)
                            {
                                int id = PieceType.Parse(uid);
                                if (id == PieceType.None.Id)
                                {
                                    throw new Exception($"Unknown piece uid '{uid}'");
                                }

                                if (!payload.ContainsKey(id))
                                {
                                    payload.Add(id, 0);
                                }

                                payload[id]++;
                            }
                            
                            AirShipSaveItem saveItem = new AirShipSaveItem();
                            saveItem.Position = Camera.main.transform.position;
                            saveItem.Payload = payload;

                            if (airShipSaveComponent.Items == null)
                            {
                                airShipSaveComponent.Items = new List<AirShipSaveItem>();
                            }
                            
                            airShipSaveComponent.Items.Add(saveItem);
                        }
                    }

                    IJsonDataMapper<UserProfile> dataMapper = new JsonStringDataMapper<UserProfile>(null);
                    manager.UploadCurrentProfile(dataMapper);
                    string fixedConfigText = dataMapper.GetJsonDataAsString();
                    ProfileSlots.SafeReplaceCurrentProfile(fixedConfigText, onComplete);
                }
                catch (Exception e)
                {
                    IW.Logger.Log("Error when parse ClientSupportDef json: " + e.Message);
                    onComplete(LocalizationService.Get("dlg.client.support.error.wrong.data"));
                }
            });
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
                case "fields":
                    ApplyChangesForFieldsMode(json, onComplete);
                    break;
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