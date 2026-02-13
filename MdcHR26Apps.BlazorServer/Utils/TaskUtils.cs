using MdcHR26Apps.BlazorServer.Models;

namespace MdcHR26Apps.BlazorServer.Utils
{
    public class TaskUtils
    {
        #region + [1] Levellist
        public List<TaskLevelModel> GetTaskLevels()
        {
            List<TaskLevelModel> taskLevels = new List<TaskLevelModel>
            {
                new TaskLevelModel(){ TaskLevelid = 1 , TaskLevelName = "5 - 매우 어려움", TaskLevelScore = 1.2 },
                new TaskLevelModel(){ TaskLevelid = 2 , TaskLevelName = "4 - 어려움", TaskLevelScore = 1.1 },
                new TaskLevelModel(){ TaskLevelid = 3 , TaskLevelName = "3 - 보통", TaskLevelScore = 1 },
                new TaskLevelModel(){ TaskLevelid = 4 , TaskLevelName = "2 - 쉬움", TaskLevelScore = 0.9 },
                new TaskLevelModel(){ TaskLevelid = 4 , TaskLevelName = "1 - 매우 쉬움 ", TaskLevelScore = 0.8 }
            };

            return taskLevels;
        }

        #endregion

        #region + [2] LeveltoName
        public string LeveltoName(double levelno)
        {
            string levelString = String.Empty;

            switch (levelno)
            {
                case 1.2:
                    levelString = "매우 어려움";
                    break;
                case 1.1:
                    levelString = "어려움";
                    break;
                case 1:
                    levelString = "보통";
                    break;
                case 0.9:
                    levelString = "쉬움";
                    break;
                case 0.8:
                    levelString = "매우 쉬움";
                    break;
                default:
                    break;
            }

            return levelString;
        }
        #endregion

        #region + [3] 상태창표시 : StatusString
        public string StatusString(int no)
        {
            string result = String.Empty;
            switch (no)
            {
                case 0:
                    result = "진행중";
                    break;
                case 1:
                    result = "종료";
                    break;
                case 2:
                    result = "보류";
                    break;
                case 3:
                    result = "취소";
                    break;
                default:
                    break;
            }

            return result;
        }
        #endregion

        #region + 참고
        //https://github.com/VisualAcademy/Dul/tree/master
        #endregion

        #region + [4].[1] 문자열자르기
        public string CutString(string cut, int length, string suffix = "...")
        {
            //if (cut.Length > (length - 3))
            //{
            //    return cut.Substring(0, length - 3) + "" + suffix;
            //}
            //return cut;

            if (length > 10)
            {
                if (cut.Length > (length - 3))
                {
                    return cut.Substring(0, length - 3) + "" + suffix;
                }
                return cut;
            }
            else
            {
                return !String.IsNullOrEmpty(cut) ? cut : String.Empty;
            }
        }
        #endregion

        #region + [4].[2] 문자열자르기유니코드
        public string CutStringUnicode(string cut, int length, string suffix = "...")
        {
            //if (cut.Length > (length - 3))
            //{
            //    return cut.Substring(0, length - 3) + "" + suffix;
            //}
            //return cut;

            string result = cut;

            if (length > 1)
            {
                var si = new System.Globalization.StringInfo(cut);
                var l = si.LengthInTextElements;

                if (l > (length - 3))
                {
                    result = si.SubstringByTextElements(0, length) + "" + suffix;
                }
                return result;
            }
            else
            {
                return result;
            }
        }
        #endregion
    }
}
