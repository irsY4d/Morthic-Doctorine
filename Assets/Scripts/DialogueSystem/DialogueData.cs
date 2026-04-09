    using UnityEngine;

    [System.Serializable]
    public class DialogueCharacter
    {
        public string name;
        public Sprite icon;
    }

    [System.Serializable]
    public class DialogueLine
    {
        [TextArea(3, 10)]
        public string line;
    }

    [System.Serializable]
    public class DialogueData
    {
        public DialogueCharacter character;
        public DialogueLine[] lines;
    }