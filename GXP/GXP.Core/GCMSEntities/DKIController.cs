using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;
using Fareportal.GlobalCMS.DataStore;
using System.Collections.Specialized;
using Fareportal.GlobalCMS.Library;
using System.Web;
using GXP.Core.GCMSEntities;

namespace GXP.Core.Framework
{
    public class DKIController
    {
        private static readonly string DKI_REGEX_PATTERN = "\\[#.*?#\\]";
        public struct DKIExpression
        {
            public string key;
            public string defaultValue;
            public string stringFunction;
        }

        //******* Function to implement DKI functions on Query string and base data values
        public static string DoDKI(string content_, HttpContextBase context_, PagePublisherInput input_)
        {
            MatchCollection matchCollection = new Regex(DKI_REGEX_PATTERN).Matches(content_);
            RequestValueResolver requestValueResolver = new RequestValueResolver();
            requestValueResolver.CurrentContext = context_;

            QueryStringValueResolver queryStringValueResolver = new QueryStringValueResolver();
            queryStringValueResolver.CurrentContext = requestValueResolver.CurrentContext;
            string value = string.Empty;

            foreach (Match match in matchCollection)
            {
                string[] keyValuePair = null;
                keyValuePair = match.Groups[0].ToString().Split('|');

                DKIExpression DKIExp = ParseDKIExpression(match.Groups[0].ToString());

                // Check if it is a Request DKI
                if ((requestValueResolver.CanResolve(DKIExp.key)))
                {
                    value = requestValueResolver.Resolve(DKIExp.key);
                    if (!string.IsNullOrEmpty(value))
                    {
                        if ((IsValidKeyToReplace(value, (Int32)input_.ActiveTab.PortalID)))
                        {
                            content_ = Strings.Replace(content_, match.Groups[0].ToString(), ApplyStringFunction(value, DKIExp.stringFunction), 1, -1, CompareMethod.Text);
                        }
                    }
                    else if (!string.IsNullOrEmpty(DKIExp.defaultValue))
                    {
                        content_ = Strings.Replace(content_, match.Groups[0].ToString(), DKIExp.defaultValue, 1, -1, CompareMethod.Text);
                    }
                    else
                    {
                        content_ = Strings.Replace(content_, match.Groups[0].ToString(), string.Empty, 1, -1, CompareMethod.Text);
                    }
                    // Replace Request DKI with its value
                    // Check if its a QueryString DKI
                }
                else if ((queryStringValueResolver.CanResolve(DKIExp.key)))
                {
                    value = queryStringValueResolver.Resolve(DKIExp.key);
                    if (IsValidKeyToReplace(value, (Int32)input_.ActiveTab.PortalID))
                    {
                        content_ = Strings.Replace(content_, match.Groups[0].ToString(), ApplyStringFunction(value, DKIExp.stringFunction), 1, -1, CompareMethod.Text);
                        //check if it is a DKI then replace value here
                    }
                }
                else if (!string.IsNullOrEmpty(DKIExp.defaultValue))
                {
                    content_ = Strings.Replace(content_, match.Groups[0].ToString(), DKIExp.defaultValue, 1, -1, CompareMethod.Text);
                    // If it is not a DKI then replace "DefaultValue" value here
                }
                else
                {
                    content_ = Strings.Replace(content_, match.Groups[0].ToString(), string.Empty, 1, -1, CompareMethod.Text);
                    // else replace key with empty string.
                }

            }
            //Return content_
            return content_;
        }

        //*******Function to implemet DKI function on SQL Module Output Parameter values
        public static string DoDKI(string content_, DataRow dr_)
        {
            MatchCollection matchCollection = new Regex(DKI_REGEX_PATTERN).Matches(content_);

            foreach (Match match in matchCollection)
            {
                DKIExpression DKIExp = ParseDKIExpression(match.Groups[0].ToString());
                content_ = DoDKIDataRow(content_, match, DKIExp, dr_);
            }
            return content_;
        }

        public static string DoDKIQueryString(string content_, string matchedExpression_, DKIExpression dkiExp_, int portalId_)
        {
            QueryStringValueResolver queryStringValueResolver = new QueryStringValueResolver();
            string value = queryStringValueResolver.Resolve(dkiExp_.key);

            if (IsValidKeyToReplace(value, portalId_))
            {
                content_ = Strings.Replace(content_, matchedExpression_, ApplyStringFunction(value, dkiExp_.stringFunction), 1, -1, CompareMethod.Text);
                //check if it is a DKI then replace value here
            }

            return content_;
        }

        public static string DoDKIDataRow(string content_, Match match_, DKIExpression dkiExp_, DataRow dr_)
        {
            foreach (DataColumn Column in dr_.Table.Columns)
            {
                if (dkiExp_.key.ToUpper() == Column.ColumnName.ToUpper())
                {
                    content_ = Strings.Replace(content_, match_.ToString(), ApplyStringFunction(dr_[Column].ToString(), dkiExp_.stringFunction), 1, -1, CompareMethod.Text);
                    //check if it is a DKI then replace value here
                }
            }
            return content_;
        }

        private static DKIExpression ParseDKIExpression(string expression_)
        {
            string key = null;
            string defaultValue = null;
            string stringCase = null;
            string[] keyValuePair = null;
            DKIExpression dkiExp = default(DKIExpression);
            string startTag = "[#";
            string endTag = "#]";
            key = string.Empty;
            defaultValue = string.Empty;
            stringCase = string.Empty;

            keyValuePair = expression_.Split('|');

            if (keyValuePair.Length == 3)
            {
                key = keyValuePair[0].Insert(keyValuePair[0].Length, endTag);
                defaultValue = keyValuePair[1].Replace(endTag, string.Empty);
                stringCase = keyValuePair[2].Replace(endTag, string.Empty);

            }
            else if (keyValuePair.Length == 2)
            {
                key = keyValuePair[0].Insert(keyValuePair[0].Length, endTag);
                keyValuePair[1] = keyValuePair[1].Replace(endTag, string.Empty);
                //identify if it's string function or default value
                if (HasDKIFunctions(keyValuePair[1].ToLower()))
                {
                    stringCase = keyValuePair[1];
                }
                else
                {
                    defaultValue = keyValuePair[1];
                }
            }
            else
            {
                key = keyValuePair[0];
            }
            key = key.Replace(startTag, string.Empty).Replace(endTag, string.Empty);
            //remove tags from key before doing final replacement
            dkiExp.key = key;
            dkiExp.defaultValue = defaultValue;
            dkiExp.stringFunction = stringCase;
            return dkiExp;
        }

        private static bool HasDKIFunctions(string str_)
        {
            string stringCasingIdentifiers = "uc,lc,tc,sc,w(,lw,fw,ntow(,s2d,d2s,rq,ue";
            string[] splitted = null;

            splitted = str_.Split('#');

            foreach (string functn in splitted)
            {
                if (stringCasingIdentifiers.Contains(Strings.Left(functn, 2)))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsValidKeyToReplace(string value_, int portalId_)
        {
            //Get all invalid tokens
            string[] invalidTokens = GXP.Default.IsValidHTMLElement.Split(new char[] { ',' });
            bool isValidHTML = true;

            //Check for each invelid token and if found return true othrewise false
            foreach (string str in invalidTokens)
            {
                if ((value_.ToLower().IndexOf(str) > -1))
                {
                    isValidHTML = false;
                    break; // TODO: might not be correct. Was : Exit For
                }
            }

            return isValidHTML;
        }

        // Change the case of key according to defined case
        public static string ApplyStringFunction(string keyValue_, string keyCase_)
        {
            string upperCase = "uc";
            string lowerCase = "lc";
            string titleCase = "tc";
            string sentenceCase = "sc";
            string wordSplitter = "w(*";
            string firstWord = "fw";
            string lastWord = "lw";
            string numberToWord = "ntow";
            string spaceToDash = "s2d";
            string dashToSpace = "d2s";
            string removeQuerystring = "rq";
            string encodeURL = "ue";

            string finalKey = string.Empty;

            string[] Functions = null;
            Functions = keyCase_.Split('#');

            foreach (string Function_ in Functions)
            {
                switch (Function_.ToLower())
                {
                    case "upperCase":
                        finalKey = keyValue_.ToUpper();
                        break;
                    case "lowerCase":
                        finalKey = keyValue_.ToLower();
                        break;
                    case "titleCase":
                        finalKey = Strings.StrConv(keyValue_, VbStrConv.ProperCase);
                        break;
                    case "sentenceCase":
                        finalKey = Strings.UCase(keyValue_[0]) + keyValue_.Substring(1).ToLower();
                        break;
                    case "firstWord":
                        finalKey = GetFirstWord(keyValue_);
                        break;
                    case "lastWord":
                        finalKey = GetLastWord(keyValue_);
                        break;
                    case "numberToWord":
                        NumericToWord num = new NumericToWord();
                        finalKey = num.changeNumericToWords(keyValue_);
                        break;
                    case "spaceToDash":
                        finalKey = keyValue_.Replace(" ", "-");
                        break;
                    case "dashToSpace":
                        finalKey = keyValue_.Replace("-", " ");
                        break;
                    case "removeQuerystring":
                        if ((keyValue_.Contains("?")))
                        {
                            finalKey = keyValue_.Split('?')[0];
                        }
                        else
                        {
                            finalKey = keyValue_;
                        }
                        break;
                    case "encodeURL":
                        finalKey = HttpUtility.UrlEncode(keyValue_);
                        break;
                    default:
                        if (Function_.Contains(wordSplitter) )
                        {
                            finalKey = GetWord(keyValue_, Function_);
                        }
                        else
                        {
                            finalKey = keyValue_;
                            //Assigning default value if String Casing is  not defined
                        }
                        break;
                }
                keyValue_ = finalKey;
            }

            return finalKey;
        }

        private static string GetWord(string keyValue_, string function_)
        {
            string finalKey = keyValue_;
            string[] @params = null;
            int wordIndex = 0;
            //'1 based index no of word from the string
            char[] delimeter = { ' ' };
            Regex regex = null;
            Match match = null;
            string defaultWord = string.Empty;
            try
            {
                regex = new Regex("w\\(.*?\\)");
                match = regex.Match(function_);
                if (match.Success)
                {
                    function_ = function_.TrimStart('w');
                    function_ = function_.TrimStart('(');
                    function_ = function_.TrimEnd(')');
                    @params = function_.Split(',');
                    if (Int32.TryParse(@params[0], out wordIndex))
                    {
                        if (@params.Length == 2)
                        {
                            if (@params[1].Contains("fw") | @params[1].Contains("lw"))
                            {
                                defaultWord = @params[1].Trim('\'');
                            }
                            else
                            {
                                @params[1] = @params[1].Trim('\'') + " ";
                                delimeter[0] = @params[1][0];
                            }
                        }
                        else if (@params.Length == 3)
                        {
                            @params[1] = @params[1].Trim('\'') + " ";
                            delimeter[0] = @params[1][0];
                            defaultWord = @params[2].Trim('\'');
                        }
                        try
                        {
                            finalKey = keyValue_.Split(delimeter, StringSplitOptions.RemoveEmptyEntries)[wordIndex - 1];
                        }
                        catch (Exception ex)
                        {
                            switch (defaultWord)
                            {
                                case "fw":
                                    finalKey = GetFirstWord(keyValue_);
                                    break;
                                case "lw":
                                    finalKey = GetLastWord(keyValue_);
                                    break;
                                default:
                                    finalKey = keyValue_;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return finalKey;
        }

        private static string GetFirstWord(string str_)
        {
            int Index = 0;
            str_ = str_.Replace("-", " ");
            Index = str_.IndexOf(" ");
            if (Index != -1)
            {
                return str_.Substring(0, Index);
            }
            else
            {
                return str_;
            }
        }

        private static string GetLastWord(string str_)
        {
            str_ = Strings.StrReverse(str_);
            str_ = GetFirstWord(str_);
            return Strings.StrReverse(str_);
        }
        
        public static string DoDKIRequest(string content_, string matchedExpression_, DKIExpression dkiExp_, int portalId_)
        {
            RequestValueResolver requestValueResolver = new RequestValueResolver();
            string value = requestValueResolver.Resolve(dkiExp_.key);
            if (!string.IsNullOrEmpty(value))
            {
                if ((IsValidKeyToReplace(value, portalId_)))
                {
                    content_ = Strings.Replace(content_, matchedExpression_, ApplyStringFunction(value, dkiExp_.stringFunction), 1, -1, CompareMethod.Text);
                }
            }
            else if (!string.IsNullOrEmpty(dkiExp_.defaultValue))
            {
                content_ = Strings.Replace(content_, matchedExpression_, dkiExp_.defaultValue, 1, -1, CompareMethod.Text);
            }
            else
            {
                content_ = Strings.Replace(content_, matchedExpression_, string.Empty, 1, -1, CompareMethod.Text);
            }
            return content_;
        }

    }
}
