using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BirthdayAttackDemo
{
    public class BirthdayAttacker
    {
        private SHA256 _sha256 = SHA256.Create();
        private List<string> _targetFileContents = new List<string>();
        private Random _random = new Random();

        public int MaxEndLineSpaces { get; private set; }
        public string RealPath { get; private set; }
        public string FakePath { get; private set; }
        public string RealHash { get; private set; }
        public string FakeHash { get; private set; }
        public int HashEndCharsInCommon { get; private set; }

        public BirthdayAttacker(string realPath, string fakePath, int maxEndLineSpaces)
        {
            RealPath = realPath;
            FakePath = fakePath;
            MaxEndLineSpaces = maxEndLineSpaces;
            RealHash = Sha256HashOfFile(realPath);
        }

        public string Sha256HashOfFile(string path)
        {            
            using var stream = File.OpenRead(path);
            var hash = _sha256.ComputeHash(stream);
            return ByteArrayToString(hash);
        }

        private void ReadFakeFile()
        {
            var eof = false;
            _targetFileContents.Clear();
            using var textReader = new StreamReader(FakePath);
            string? nextLine;
            while (!eof)
            {
                nextLine = textReader.ReadLine();
                if (nextLine != null)
                    _targetFileContents.Add(nextLine);
                else
                    eof = true;
            }
        }

        public void MakeModifiedFile()
        {
            if (_targetFileContents.Count == 0) ReadFakeFile();
            var regex = new Regex(@"(\s){1,}$");
            var lineToChange = _random.Next(0, _targetFileContents.Count -1);
            var textLine = _targetFileContents.ElementAt(lineToChange);
            var match = regex.Match(textLine);
            if (match.Success)
            {
                if (match.Groups[0].Length >= MaxEndLineSpaces)
                    textLine = textLine.TrimEnd();
                else
                    textLine += ' ';            
            }
            else
             textLine += ' ';
            _targetFileContents[lineToChange] = textLine;
            WriteModifiedFile();
            FakeHash = Sha256HashOfFile(FakePath);
            HashEndCharsInCommon = EndHashCharsMatch();
        }

        private int EndHashCharsMatch()
        {
            int matchingChars = 0;
            for (var i = RealHash.Length-1; i>=0; i--)
            {
                if (RealHash[i] == FakeHash[i])
                    matchingChars++;
                else
                    break;
            }
            return matchingChars;
        }

        private void WriteModifiedFile()
        {
            using var textWriter = new StreamWriter(FakePath);
            _targetFileContents.ForEach(line => textWriter.WriteLine(line));
        }

        private static string ByteArrayToString(byte[] array)
        {
            var sb = new StringBuilder();
            foreach (var c in array)
                sb.Append(c.ToString("X2"));
            return sb.ToString();
        }
    }
}
