using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace UT
{
    public class UTFogs
    {
        [Test]
        public void UTFogCentersPasses()
        {
            // LogAssert.ignoreFailingMessages = true;

            var dataMapper = new ResourceConfigDataMapper<FogsDataManager>("configs/fogs.data", NSConfigsSettings.Instance.IsUseEncryption);
            
            dataMapper.LoadData((data, error)=> 
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Assert.Fail(error);
                    return;
                }
                
                StringBuilder sb = new StringBuilder();

                HashSet<string> ids = new HashSet<string>();
                
                foreach (var fog in data.Fogs)
                {
                    if (!ids.Add(fog.Uid))
                    {
                        sb.AppendLine($"Fog '{fog.Uid}' is separated to several independent areas");
                    }
                    
                    var center = fog.GetCenter();
                    if (center.Equals(BoardPosition.Default()))
                    {
                        sb.AppendLine($"Fog '{fog.Uid}': Center is not specified or specified multiply times");
                    }
                    else
                    {
                        bool isCenterOk = false;
                        foreach (var position in fog.Positions)
                        {
                            if (center.Equals(position))
                            {
                                isCenterOk = true;
                                break;
                            }
                        }

                        if (!isCenterOk)
                        {
                            sb.AppendLine($"Fog '{fog.Uid}': Center is outside of the fog area ({center})");
                        }
                    }
                }

                // LogAssert.ignoreFailingMessages = false;
            
                var message = sb.ToString();
                if (string.IsNullOrEmpty(message))
                {
                    Assert.Pass();
                }
                else
                {
                    Assert.Fail(message);
                }
            });
        }
    }
}