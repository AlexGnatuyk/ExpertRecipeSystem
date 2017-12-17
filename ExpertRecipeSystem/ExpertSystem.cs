using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExpertRecipeSystem
{
    public class ExpertSystem
    {
        private const int CurrentState = 0;
        private const int NextState = 1;
        private const int EndOfSearching = 2;
        private const int Answer = 3;
        private const int Output = 4;

        private readonly string _pathToStateTableFile;
        private readonly string _pathToStateQuestionMapFile;
        private List<string[]> _stateTableList;
        private readonly Dictionary<int, string> _questionList;
        private int _currentState = -1;
        private bool _isEndOfSearching;

        public string NextQuestion(string answerForPreviousQuestion, out bool isEndOfSearching)
        {
            isEndOfSearching = _isEndOfSearching;
            if (_currentState == -1)
            {
                _currentState = 0;
                var question = _questionList[_currentState];
                return question;
            }

            if (_stateTableList.FindIndex(tokens => tokens[Answer] == answerForPreviousQuestion) == -1)
                return null;
            
            var transition = _stateTableList
                .Where(tokens => int.Parse(tokens[CurrentState]) == _currentState)
                .FirstOrDefault(tokens => tokens[Answer] == answerForPreviousQuestion);

            _currentState = int.Parse(transition[NextState]);

            _isEndOfSearching = transition[EndOfSearching] != "0";
            isEndOfSearching = _isEndOfSearching;

            if (isEndOfSearching)
                return transition[Output];

            return _questionList[_currentState];
        }

        private void ParseFiles()
        {
            _stateTableList = File.ReadLines(_pathToStateTableFile, Encoding.GetEncoding(1251))
                .Skip(1)
                .Select(line => line.Split(';'))
                .ToList();


            var pairs = File.ReadLines(_pathToStateQuestionMapFile, Encoding.GetEncoding(1251))
                .Skip(1)
                .Select(line => line.Split(';'))
                .Select(tokens => new {Key = int.Parse(tokens[0]), Value = tokens[1]});
            foreach (var pair in pairs)
            {
                _questionList.Add(pair.Key, pair.Value);
            }
        }

        public ExpertSystem(string pathToStateTableFile, string pathToStateQuestionMapFile)
        {
            _pathToStateQuestionMapFile = pathToStateQuestionMapFile;
            _pathToStateTableFile = pathToStateTableFile;
            _stateTableList = new List<string[]>();
            _questionList = new Dictionary<int, string>();
            _isEndOfSearching = false;
            ParseFiles();
        }
    }
}