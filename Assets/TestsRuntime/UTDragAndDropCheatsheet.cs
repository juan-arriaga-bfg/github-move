using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace UTRuntime
{
    public class UTDragAndDropCheatsheet
    {

        [Test]
        public void UTDragAndDropCheatsheetSimplePasses()
        {
            // Use the Assert class to test conditions.
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator UTDragAndDropCheatsheetWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // yield to skip a frame
            yield return null;
            
            // reset progress in editor
            #if UNITY_EDITOR
            var progressPath = Application.dataPath + "/Resources/configs/profile.data.txt";
            File.Delete(progressPath);
            File.Delete(progressPath + ".meta");
            AssetDatabase.Refresh();
            #endif

            SceneManager.LoadScene("Assets/Scenes/Main.unity", LoadSceneMode.Single);

            while (BoardService.Current == null || BoardService.Current.FirstBoard == null || UIService.Get.IsComplete == false)
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(1f);

            var testIds = PieceType.GetIdsByFilter(PieceTypeFilter.Default);

            var piecesCheatSheetWindowModel = new UIPiecesCheatSheetWindowModel();
            testIds = piecesCheatSheetWindowModel.GetPieceIdsBy(PieceTypeFilter.Default);
            
            BoardPosition testPoint = new BoardPosition(21, 12, BoardLayer.Piece.Layer);
            var availablePoints = new List<BoardPosition>();
            BoardService.Current.FirstBoard.BoardLogic.EmptyCellsFinder.FindNearWithPointInCenter(testPoint, availablePoints, 1, 100);

            if (availablePoints.Count <= 0)
            {
                Assert.Fail("No free points for test");
            }
            else
            {
                testPoint = availablePoints[0];

                for (int i = 0; i < testIds.Count; i++)
                {
                    var testId = testIds[i];
                
                    Debug.LogWarning($"test piece:{PieceType.Parse(testId)}:{testId}");
                
                    var fakePiece = BoardService.Current.FirstBoard.BoardLogic.DragAndDrop.CreateFakePiece(testId, testPoint);
            
                    yield return new WaitForSeconds(0.02f);
            
                    BoardService.Current.FirstBoard.BoardLogic.DragAndDrop.DestroyFakePiece(fakePiece);

                    yield return new WaitForSeconds(0.02f);
                }
   
                Assert.Pass();
            }

            
        }
    }
}