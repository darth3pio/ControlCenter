using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Xml;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    public static class MedalDataParser
    {
        /// <summary>
        /// Loads the default medal data into the Award Cache
        /// </summary>
        static MedalDataParser()
        {
            // Fill the award cache with the original award conditions
            MatchCollection MedalsMatches, RankMatches;
            ParseMedalData(Utils.GetResourceAsString("BF2Statistics.MedalData.PyFiles.medal_data_xpack.py"), out MedalsMatches, out RankMatches);

            // Convert each medal match into an object, and add it to the award cache
            foreach (Match ArrayMatch in MedalsMatches)
            {
                AwardCache.AddDefaultAwardCondition(
                        ArrayMatch.Groups["MedalIntId"].Value,
                        ParseCondition(ArrayMatch.Groups["Conditions"].Value)
                );
            }

            // Convert ranks into objects, and also add them to the award cache
            foreach (Match ArrayMatch in RankMatches)
            {
                AwardCache.AddDefaultAwardCondition(
                        ArrayMatch.Groups["RankId"].Value,
                        ParseCondition(ArrayMatch.Groups["Conditions"].Value)
                );
            }
        }

        /// <summary>
        /// This method loads a Medal data python file, and parses it. The medal data is stored
        /// in the AwardCache object
        /// </summary>
        /// <param name="FilePath">The full file path to the python file</param>
        public static void LoadMedalDataFile(string FilePath)
        {
            // Get the medal data contents, and remove all spaces, comments, tabs, and line breaks
            MatchCollection MedalsMatches, RankMatches;
            ParseMedalData(ReadPythonFile(FilePath), out MedalsMatches, out RankMatches);
            BuildAwardCache(MedalsMatches, RankMatches);
        }

        /// <summary>
        /// This method takes a medal data string, and parses it. The parsed match
        /// collections are passed back out from referenced variables
        /// </summary>
        /// <param name="Data">The contents of the medal data file</param>
        public static void ParseMedalData(string Data, out MatchCollection MedalsMatches, out MatchCollection RanksMatches)
        {
            // Replace all Pythonic multi-line comments.
            Data = Regex.Replace(Data, "\"\"\"(.*?)\"\"\"", "", RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            //Replace all Pythonic single-line comments.
            Data = Regex.Replace(Data, "^#((!stop).)*$", "", RegexOptions.ExplicitCapture | RegexOptions.Multiline);
            // Replace all spaces, tabs, newlines and carriage returns
            Data = Data.Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");

            // Make sure this is a formated medal data file
            if(Data.Contains("player_score") || !Data.Contains("#stop"))
                throw new Exception("Improper Medal data file. Only medal data files generated with this program can be editied."
                    + " Click the \"New\" button to generate a new medal data file, compatible with this editor.");

            // Match and Capture awards and ranks
            Match Medals = Regex.Match(Data, @"medal_data=\((?<Array>.*?)\)rank_data=", RegexOptions.IgnoreCase);
            Match Ranks = Regex.Match(Data, @"rank_data=\((?<Array>.*?)\)#end", RegexOptions.IgnoreCase);

            // Make sure we have captures, otherwise its a parse error
            if (!Medals.Success || !Ranks.Success)
                throw new ParseException();

            // Caputre all awards
            string pattern = "\\(['|\"](?<MedalIntId>[0-9]{7}(_[0-3])?)['|\"],"
                + "['|\"](?<MedalStrId>.*?)['|\"],"
                + "(?<RewardType>[0-2]),"
                + "(?<Conditions>[a-z]+_[a-z]+\\(.*?\\))\\),#stop";
            MedalsMatches = Regex.Matches(Medals.Groups["Array"].Value, pattern, RegexOptions.ExplicitCapture);

            // Capture all Ranks
            pattern = "\\((?<RankId>[0-9]+),'rank',(?<Conditions>[a-z]+_[a-z]+\\(.*?\\))\\),#stop";
            RanksMatches = Regex.Matches(Ranks.Groups["Array"].Value, pattern, RegexOptions.ExplicitCapture | RegexOptions.Multiline);
        }

        /// <summary>
        /// This method builds the award cache with the given data from the medal data file.
        /// This method WILL CLEAR the award cache from any existing medals
        /// </summary>
        /// <param name="MedalsMatches"></param>
        /// <param name="RanksMatches"></param>
        public static void BuildAwardCache(MatchCollection MedalsMatches, MatchCollection RanksMatches)
        {
            // Clear out the award cache!
            AwardCache.Clear();

            // Convert each medal match into an object, and add it to the award cache
            foreach (Match ArrayMatch in MedalsMatches)
            {
                AwardCache.AddAward(
                    new Award(
                        ArrayMatch.Groups["MedalIntId"].Value,
                        ArrayMatch.Groups["MedalStrId"].Value,
                        ArrayMatch.Groups["RewardType"].Value,
                        ParseCondition(ArrayMatch.Groups["Conditions"].Value)
                    )
                );
            }

            // Convert ranks into objects, and also add them to the award cache
            foreach (Match ArrayMatch in RanksMatches)
            {
                AwardCache.AddRank(
                    new Rank(
                        Int32.Parse(ArrayMatch.Groups["RankId"].Value), 
                        ParseCondition(ArrayMatch.Groups["Conditions"].Value)
                    )
                );
            }
        }

        /// <summary>
        /// Breaks an input string into recognizable tokens
        /// </summary>
        /// <param name="source">The input string to break up</param>
        /// <returns>The set of tokens located within the string</returns>
        private static IEnumerable<Token> Tokenize(string source)
        {
            List<Token> tokens = new List<Token>();
            KeyValuePair<string, int>[]  sourceParts = new[] { new KeyValuePair<string, int>(source, 0) };

            tokens.AddRange(Tokenize(TokenType.OpenParen, "\\(", ref sourceParts));
            tokens.AddRange(Tokenize(TokenType.CloseParen, "\\)", ref sourceParts));
            tokens.AddRange(Tokenize(TokenType.Literal, "[A-Za-z0-9_-]+", ref sourceParts));

            return tokens.OrderBy(x => x.Position);
        }

        /// <summary>
        /// Performs tokenization of a collection of non-tokenized data parts with a specific pattern
        /// </summary>
        /// <param name="tokenKind">The name to give the located tokens</param>
        /// <param name="pattern">The pattern to use to match the tokens</param>
        /// <param name="untokenizedParts">The portions of the input that have yet to be tokenized (organized as text vs. position in source)</param>
        /// <returns>The set of tokens matching the given pattern located in the untokenized portions of the input, <paramref name="untokenizedParts"> is updated as a result of this call</returns>
        private static IEnumerable<Token> Tokenize(TokenType tokenKind, string pattern, ref KeyValuePair<string, int>[] untokenizedParts)
        {
            //Do a bit of setup
            var resultParts = new List<KeyValuePair<string, int>>();
            var resultTokens = new List<Token>();
            var regex = new Regex(pattern);

            //Look through all of our currently untokenized data
            foreach (var part in untokenizedParts)
            {
                //Find all of our available matches
                List<Match> matches = regex.Matches(part.Key).OfType<Match>().ToList();

                //If we don't have any, keep the data as untokenized and move to the next chunk
                if (matches.Count == 0)
                {
                    resultParts.Add(part);
                    continue;
                }

                //Store the untokenized data in a working copy and save the absolute index it reported itself at in the source file
                string workingPart = part.Key;
                int index = part.Value;

                //Look through each of the matches that were found within this untokenized segment
                foreach (Match match in matches)
                {
                    //Calculate the effective start of the match within the working copy of the data
                    int effectiveStart = match.Index - (part.Key.Length - workingPart.Length);
                    resultTokens.Add(Token.Create(tokenKind, match, part.Value));

                    //If we didn't match at the beginning, save off the first portion to the set of untokenized data we'll give back
                    if (effectiveStart > 0)
                    {
                        string value = workingPart.Substring(0, effectiveStart);
                        resultParts.Add(new KeyValuePair<string, int>(value, index));
                    }

                    //Get rid of the portion of the working copy we've already used
                    if (match.Index + match.Length < part.Key.Length)
                        workingPart = workingPart.Substring(effectiveStart + match.Length);
                    else
                        workingPart = string.Empty;

                    //Update the current absolute index in the source file we're reporting to be at
                    index += effectiveStart + match.Length;
                }

                //If we've got remaining data in the working copy, add it back to the untokenized data
                if (!string.IsNullOrEmpty(workingPart))
                    resultParts.Add(new KeyValuePair<string, int>(workingPart, index));
            }

            //Update the untokenized data to contain what we couldn't process with this pattern
            untokenizedParts = resultParts.ToArray();
            //Return the tokens we were able to extract
            return resultTokens;
        }

        /// <summary>
        /// Takes each literal token, and determines if its a StatFunction, or ConditionFunction
        /// </summary>
        /// <param name="parts">The list of tokens</param>
        /// <returns>Returns true if it was able to convert any literals to function names.</returns>
        private static bool ParseFunctions(ref List<Token> parts)
        {
            for (int i = 0; i < parts.Count; i++)
            {
                int Next = i + 1;
                if (parts.Count > Next && parts[Next].Kind == TokenType.OpenParen)
                    parts[i].Kind = (parts[i].Value.StartsWith("f_")) ? TokenType.ConditionFunction : TokenType.StatFunction;
            }
            return false;
        }

        /// <summary>
        /// Takes a node tree and converts it to a Condition
        /// </summary>
        /// <param name="Node"></param>
        /// <returns></returns>
        public static Condition ParseNodeConditions(TreeNode Node)
        {
            if (Node.Tag == null)
                return null;

            if (Node.Tag is ConditionList)
            {
                ConditionList C = (ConditionList)Node.Tag;
                List<Condition> Cs = C.GetConditions();

                // If a Plus / Div list has 3rd param, add the node
                if (Cs.Count == 3 && (C.Type == ConditionType.Plus || C.Type == ConditionType.Div))
                    Node.Nodes.Add(Cs[2].ToTree());

                // Remove old conditions
                C.Clear();

                foreach (TreeNode Sub in Node.Nodes)
                {
                    Condition SC = ParseNodeConditions(Sub);
                    if (SC == null)
                        continue;

                    C.Add(SC);
                }
                return C;
            }
            else
                return (Node.Tag is ConditionValue) ? (ConditionValue) Node.Tag : (Condition)Node.Tag;
        }

        /// <summary>
        /// Parses a string condition into condition objects
        /// </summary>
        /// <param name="ConditionString"></param>
        /// <returns></returns>
        private static Condition ParseCondition(string ConditionString)
        {
            int position = 0;
            return ParseCondition(Tokenize(ConditionString).ToList<Token>(), ref position);
        }

        /// <summary>
        /// Parses a string condition into condition objects
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static Condition ParseCondition(List<Token> parts, ref int i)
        {
            ParseFunctions(ref parts);

            ConditionList List;

            for(; i < parts.Count; i++)
            {
                if (parts[i].Kind == TokenType.ConditionFunction)
                {
                    // Default
                    ConditionType Type = ConditionType.And;

                    switch (parts[i].Value)
                    {
                        case "f_and":
                            Type = ConditionType.And;
                            break;
                        case "f_or":
                            Type = ConditionType.Or;
                            break;
                        case "f_not":
                            Type = ConditionType.Not;
                            break;
                        case "f_plus":
                            Type = ConditionType.Plus;
                            break;
                        case "f_div":
                            Type = ConditionType.Div;
                            break;
                    }

                    // Create the new condition list
                    List = new ConditionList(Type);

                    // Parse sub conditions
                    i++;
                    while(i < parts.Count && parts[i].Kind != TokenType.CloseParen)
                        List.Add(ParseCondition(parts, ref i));
                    i++;

                    // Return condition list
                    return List;
                }
                else if (parts[i].Kind == TokenType.StatFunction)
                {
                    List<string> Params = new List<string>();
                    string Name = parts[i].Value;

                    for (; i < parts.Count; i++)
                    {
                        if (parts[i].Kind == TokenType.CloseParen)
                            break;

                        if (parts[i].Kind == TokenType.OpenParen)
                            continue;

                        Params.Add(parts[i].Value);
                    }

                    i++;

                    // Create the condition
                    switch (Name)
                    {
                        case "object_stat":
                            return new ObjectStat(Params);
                        case "global_stat":
                        case "player_stat":
                            return new PlayerStat(Params);
                        case "has_medal":
                        case "has_rank":
                            return new MedalOrRankCondition(Params);
                        case "global_stat_multiple_times":
                            return new GlobalStatMultTimes(Params);
                    }
                }
                else if (parts[i].Kind == TokenType.Literal)
                {
                    ConditionValue V = new ConditionValue(parts[i].Value);
                    i++;
                    return V;
                }
            }

            return new ConditionList( ConditionType.And );
        }


        /// <summary>
        /// Simple easy way to get the contents of a file, excluding empty lines.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string ReadPythonFile(string file)
        {
            List<string> Lines = new List<string>();

            using (FileStream Fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader FsReader = new StreamReader(Fs))
            {
                while (FsReader.Peek() >= 0)
                {
                    string Line = FsReader.ReadLine().Trim('\t', '\r', '\n', ' ');

                    if (string.IsNullOrWhiteSpace(Line) || Line[0] == '#')
                        continue;

                    Lines.Add(Line);
                }
            }

            return string.Join("\r\n", Lines);
        }
    }
}
