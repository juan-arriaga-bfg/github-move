using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace UT
{
    public class UTUniqueResourceName
    {
        private List<string> pathToCheck = new List<string> {"Assets/Content/ContentResources"};
        private List<string> ignoredFileNames = new List<string> { };

        [Test]
        public void UTUniqueResourceNameSimplePasses()
        {
            var dublicatedFiles = AssetsUsageFinderUtils.GetDublicatedFileNames(pathToCheck, ignoredFileNames, "");

            if (dublicatedFiles.Count > 0)
            {
                var message = new StringBuilder();
                foreach (var dublicatedFile in dublicatedFiles)
                {
                    for (int i = 0; i < dublicatedFile.Value.Count; i++)
                    {
                        var path = dublicatedFile.Value[i];
                        message.AppendLine($"{dublicatedFile.Key} => {path}");
                    }
                }

                Assert.Fail(message.ToString());
            }
            else
            {
                Assert.Pass();
            }
        }
    }
}
