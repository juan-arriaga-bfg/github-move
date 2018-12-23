using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace UT
{
    public class UTUniqueTextureName
    {
        private List<string> pathToCheck = new List<string> {"Assets/Content/SourceContent"};
        private List<string> ignoredFileNames = new List<string> { };
        private string searchFilter = "t:Texture";

        [Test]
        public void UTUniqueTextureNameSimplePasses()
        {
            // Use the Assert class to test conditions.
            var dublicatedFiles = AssetsInfo.AssetsUsageFinderUtils.GetDublicatedFileNames(pathToCheck, ignoredFileNames, searchFilter);

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