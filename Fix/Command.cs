using System.Collections.Generic;
using System.Linq;

namespace Fix
{
    public class Command
    {
        private List<string> _words;

        public Command(string line)
        {
            _words = line.Split(' ').ToList();
        }

        public int GetNumberOfLeftCommonWords(string other)
        {
            var otherWords = other.Split(' ');
            int i;
            for (i = 0; i < _words.Count; i++)
            {
                if(otherWords.Length < i || _words[i] != otherWords[i])
                {
                    break;
                }
            }
            return i;
        }

        public void ReplaceWord(string newWord, int index)
        {
            _words[index] = newWord;
        }

        public string Build()
        {
            return string.Join(' ', _words);
        }
    }
}
