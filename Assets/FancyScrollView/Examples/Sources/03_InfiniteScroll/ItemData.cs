namespace FancyScrollView.Example03
{
    public class ItemData
    {
        public int StageIndex;
        public int Score;
        public SceneManager.StageState State;
        public string Message;
        public string MessageLarge;
        public int MessageLargeFontSize;

        public ItemData(int index, int score, SceneManager.StageState state)
        {
            StageIndex = index;
            Score = score;
            State = state;
            Message = (State != SceneManager.StageState.NOTEXIST)? $"Stage {index + 1}" : "Comming Soon...";

            switch (State)
            {
            case SceneManager.StageState.UNLOCKED:
                MessageLarge = (StageIndex + 1).ToString();
                MessageLargeFontSize = 200;
                break;
            case SceneManager.StageState.LOCKED:
                MessageLarge = "Locked";
                MessageLargeFontSize = 100;
                break;
            case SceneManager.StageState.NOTEXIST:
                MessageLarge = "Comming Soon...";
                MessageLargeFontSize = 100;
                break;
            }
        }
    }
}
