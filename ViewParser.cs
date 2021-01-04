using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebServer.ExtensionMethods;

namespace WebServer
{
    public class ViewParser
    {
        readonly string _viewDir = @"Views/";
        readonly string _viewExtension = ".evan";

        public string ParseFile<T>(string viewName, T model)
        {
            List<string> fileContent = File.ReadLines(_viewDir + viewName + _viewExtension).ToList();

            string modelType = fileContent.First().Trim();

            if (typeof(T).Name != modelType)
            {
                throw new Exception($"incorrect model passed to evan file. Was expecting type {typeof(T).Name} but received type {modelType}");
            }

            string parsedView = "";

            foreach (string line in fileContent)
            {
                string modelIndicator = "$" + modelType;

                if (line.Contains(modelIndicator))
                {
                    List<int> variables = line.AllIndexesOf(modelIndicator);

                    string parsedLine = null;

                    for(int i = 0; i < variables.Count; i++)
                    {
                        string lineToParse = parsedLine ?? line;

                        int idx = lineToParse.IndexOf(modelIndicator);

                        string propName = string.Join(string.Empty, lineToParse.Skip(idx + 1 + modelType.Length + 1)
                            .TakeWhile(x => Char.IsLetterOrDigit(x)));

                        string propValue = GetPropertyValueAsString(model, propName);

                        parsedLine = ReplaceAtIndex(lineToParse, propValue, idx, modelIndicator.Length + propName.Length + 1);
                    }

                    parsedView += parsedLine;
                }
            }

            return parsedView;
        }

        public string ParseFile(string viewName)
        {
            List<string> fileContent = File.ReadLines(_viewDir + viewName + _viewExtension).ToList();

            fileContent = fileContent.Skip(1).ToList();

            return string.Join(string.Empty, fileContent);
        }

        static string GetPropertyValueAsString(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null).ToString();
        }

        static string ReplaceAtIndex(string wholeString, string value, int index, int indicatorLength)
        {
            var sb = new StringBuilder(wholeString);
            sb.Remove(index, indicatorLength);
            sb.Insert(index, value);
            return sb.ToString();

        }

    }
}
